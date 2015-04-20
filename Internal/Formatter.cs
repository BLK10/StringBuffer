using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Threading;
using System.Runtime.CompilerServices;


namespace BLK10.Text
{    
    internal static class Formatter
    {

        internal static object[] ClassifyObjects(object[] args)
        {
            List<object> objIndex = new List<object>();
            object       objName  = null;

            foreach (var arg in args)
            {
                if (arg != null)
                {
                    if (Formatter.IsAnonymousType(arg.GetType()))
                    {
                        if (objName == null)
                        {
                            objName = arg;
                        }
                        else
                        {
                            objName = TypeMerger.MergeTypes(objName, arg);
                        }
                    }
                    else
                    {
                        objIndex.Add(arg);
                    }
                }
            }

            if (objName != null)
            {
                objIndex.Add(objName);
            }

            return (objIndex.ToArray());
        }
                        
        internal static string Eval(object[] source, StringBuffer expression, IFormatProvider provider)
        {
            if (StringBuffer.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Expression cannot be null or empty.");
            }
            
            StringBuffer strIndex = expression.Copy();
            object       current  = null;

            strIndex.SubstringBefore(',', ESearch.FirstOccurrence)
                    .Fail(x => { x.SubstringBefore(':', ESearch.FirstOccurrence); })
                    .Do(x =>
                    {
                        int intIndex;

                        if (int.TryParse(x.ToString(), out intIndex))
                        {
                            if ((intIndex >= 0) && (intIndex < source.Length))
                            {
                                current = source[intIndex];
                            }
                            else
                            { // IndexOutOfRangeException
                                throw new FormatException("Input string was not in a correct format.");
                            }
                        }
                    });

            string format = null;

            if (current != null)
            { // Index based
                if (expression.StartsWithAny(StringBuffer.Whitespaces, true, false) || expression.EndsWithAny(StringBuffer.Whitespaces, true, false))
                {
                    throw new FormatException("Input string was not in a correct format.");
                }

                format = expression.Copy()
                                   .SubstringAfter(',', ESearch.FirstOccurrence)
                                   .Fail(x =>
                                   {
                                       x.SubstringAfter(':', ESearch.FirstOccurrence)
                                        .Fail(y => y.Clear());
                                   })
                                   .Prepend("{0:")
                                   .Append('}')
                                   .ToString();
            }
            else
            { // Name based
                current = source[source.Length - 1];

                if (expression.ContainsAny(StringBuffer.Whitespaces, 0, expression.Length, false))
                {
                    throw new FormatException("Input string was not in a correct format.");
                }

                var expre = expression.Copy()
                                      .DoAtWhen(x => x.IndexOf(':'),
                                               (x, i) => (i > 0),
                                               (x, i) =>
                                               {
                                                   format = x.ToString(i + 1);
                                                   x.Crop(0, i);
                                               });

                StringBuffer prop;

                do
                {
                    prop = expre.Copy()
                                .SubstringRange(0, '.', ESearch.FirstOccurrence)
                                .Fail(x => expre.Clear())
                                .Succeed(x => expre.SubstringAfter('.', ESearch.FirstOccurrence))
                                .DoWhenElse(x => x.Contains('['),
                                            x => { current = Formatter.GetIndexedPropertyValue(current, x.ToString()); },
                                            x => { current = Formatter.GetPropertyValue(current, x.ToString()); });
                }
                while ((expre.Length > 0) && (current != null));

                if (current == null)
                {
                    return ("");
                }

                if (format == null)
                {
                    return (current.ToString());
                }

                format = string.Concat("{0:" + format + "}");                
            }

            if (provider != null)
            {
                return (string.Format(provider, format, current));
            }

            return (string.Format(format, current));
        }


        private static bool IsAnonymousType(Type type)
        {
            if ((type.IsGenericType) && (type.Namespace == null))
            {
                var d = type.GetGenericTypeDefinition();

                if ((d.IsClass) && (d.IsSealed) && ((d.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic))
                {
                    var attributes = d.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false);

                    if ((attributes != null) && (attributes.Length > 0))
                    {
                        return (true);
                    }
                }
            }

            return (false);
        }        
        
        private static object GetIndexedPropertyValue(object container, string expression)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if ((expression == null) || (expression.Length == 0))
            {
                throw new ArgumentNullException("expr");
            }

            int openIdx  = expression.IndexOf('[');
            int closeIdx = expression.IndexOf(']');

            if ((openIdx < 0) || (closeIdx < 0) || (closeIdx - openIdx <= 1))
            {
                throw new ArgumentException(expression + " is not a valid indexed expression.");
            }

            string val = expression.Substring(openIdx + 1, closeIdx - openIdx - 1).Trim();

            if (val.Length == 0)
            {
                throw new ArgumentException(expression + " is not a valid indexed expression.");
            }

            bool is_string = false;

            // a quoted val means we have a string
            if (((val[0] == '\'') && (val[val.Length - 1] == '\'')) ||
                ((val[0] == '\"') && (val[val.Length - 1] == '\"')))
            {
                is_string = true;
                val = val.Substring(1, val.Length - 2);
            }
            else
            {
                // if all chars are digits, then we have an int
                for (int i = 0; i < val.Length; i++)
                {
                    if (!char.IsDigit(val[i]))
                    {
                        is_string = true;
                        break;
                    }
                }
            }

            int intVal = 0;

            if (!is_string)
            {
                try
                {
                    intVal = Int32.Parse(val);
                }
                catch
                {
                    throw new ArgumentException(expression + " is not a valid indexed expression.");
                }
            }

            string property = null;

            if (openIdx > 0)
            {
                property = expression.Substring(0, openIdx);
                if (property != null && property.Length > 0)
                {
                    container = GetPropertyValue(container, property);
                }
            }

            if (container == null)
            {
                return (null);
            }

            if (container is System.Collections.IList)
            {
                if (is_string)
                {
                    throw new ArgumentException(expression + " cannot be indexed with a string.");
                }

                IList l = (IList)container;

                return (l[intVal]);
            }

            Type t = container.GetType();

            object[] atts = t.GetCustomAttributes(typeof(DefaultMemberAttribute), false);

            if (atts.Length != 1)
            {
                property = "Item";
            }
            else
            {
                property = ((DefaultMemberAttribute)atts[0]).MemberName;
            }

            Type[]       argTypes = new Type[] { (is_string) ? typeof(string) : typeof(int) };
            PropertyInfo prop     = t.GetProperty(property, argTypes);

            if (prop == null)
            {
                throw new ArgumentException(expression + " indexer not found.");
            }

            object[] args = new object[1];

            if (is_string)
            {
                args[0] = val;
            }
            else
            {
                args[0] = intVal;
            }

            return (prop.GetValue(container, args));
        }

        private static object GetPropertyValue(object container, string propName)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if ((propName == null) || (propName.Length == 0))
            {
                throw new ArgumentNullException("propName");
            }

            PropertyDescriptor prop = TypeDescriptor.GetProperties(container).Find(propName, true);

            if (prop == null)
            {
                //return (null);                
                throw new NullReferenceException("Property " + propName + " not found in " + container.GetType());                
            }

            return (prop.GetValue(container));
        }

        /// <summary>
        /// A Utility class used to merge the properties of heterogenious objects. Original Source by Mark J. Miller.
        /// <para/>see url: www.developmentalmadness.com/archive/2008/02/12/extend-anonymous-types-using.aspx
        /// <para/>Extended by Kyle Finley (TypeMergerPolicy)
        /// <para/>see url: kylefinley.net/typemerger
        /// </summary>
        private static class TypeMerger
        {
            // assembly / module builders
            private static AssemblyBuilder  kAsmBuilder       = null;
            private static ModuleBuilder    kModBuilder       = null;
            private static TypeMergerPolicy kTypeMergerPolicy = null;

            // object type cache
            private static IDictionary<String, Type> kAnonymousTypes = new Dictionary<String, Type>();

            // used for thread-safe access to Type Dictionary
            private static object _syncLock = new object();

            /// <summary>.</summary>
            public static object MergeTypes(object values1, object values2, TypeMergerPolicy policy)
            {
                TypeMerger.kTypeMergerPolicy = policy;

                return (TypeMerger.MergeTypes(values1, values2));
            }

            /// <summary>Merge two different object instances into a single object which is a super-set of the properties of both objects.</summary>
            public static object MergeTypes(object values1, object values2)
            {
                // create a name from the names of both Types
                string name1 = String.Format("{0}_{1}", values1.GetType(), values2.GetType());
                string name2 = String.Format("{0}_{1}", values2.GetType(), values1.GetType());

                object newValues = TypeMerger.CreateInstance(name1, values1, values2);

                if (newValues != null)
                {
                    return (newValues);
                }

                newValues = TypeMerger.CreateInstance(name2, values2, values1);

                if (newValues != null)
                {
                    return (newValues);
                }

                // lock for thread safe writing
                lock (_syncLock)
                {
                    // now that we're inside the lock - check one more time
                    newValues = TypeMerger.CreateInstance(name1, values1, values2);
                    
                    if (newValues != null)
                    {
                        return (newValues);
                    }

                    // merge list of PropertyDescriptors for both objects
                    PropertyDescriptor[] pdc = TypeMerger.GetProperties(values1, values2);

                    // make sure static properties are properly initialized
                    TypeMerger.InitializeAssembly();

                    // create the type definition
                    Type newType = TypeMerger.CreateType(name1, pdc);

                    // add it to the cache
                    TypeMerger.kAnonymousTypes.Add(name1, newType);

                    // return an instance of the new Type
                    return (TypeMerger.CreateInstance(name1, values1, values2));
                }
            }
                        

            /// <summary>Instantiates an instance of an existing Type from cache.</summary>
            private static object CreateInstance(string name, object values1, object values2)
            {
                object newValues = null;

                // merge all values together into an array
                object[] allValues = TypeMerger.MergeValues(values1, values2);

                // check to see if type exists
                if (TypeMerger.kAnonymousTypes.ContainsKey(name))
                {
                    // get type
                    Type type = TypeMerger.kAnonymousTypes[name];

                    // make sure it isn't null for some reason
                    if (type != null)
                    { // create a new instance                        
                        newValues = Activator.CreateInstance(type, allValues);
                    }
                    else
                    { // remove null type entry                        
                        lock (_syncLock)
                        {
                            TypeMerger.kAnonymousTypes.Remove(name);
                        }
                    }
                }

                // return values (if any)
                return (newValues);
            }

            /// <summary>Merge PropertyDescriptors for both objects.</summary>
            private static PropertyDescriptor[] GetProperties(object values1, object values2)
            {
                // dynamic list to hold merged list of properties
                List<PropertyDescriptor> properties = new List<PropertyDescriptor>();

                // get the properties from both objects
                PropertyDescriptorCollection pdc1 = TypeDescriptor.GetProperties(values1);
                PropertyDescriptorCollection pdc2 = TypeDescriptor.GetProperties(values2);

                // add properties from values1
                for (int i = 0; i < pdc1.Count; i++)
                {
                    if ((TypeMerger.kTypeMergerPolicy == null) ||
                        (!TypeMerger.kTypeMergerPolicy.Ignored.Contains(pdc1[i].GetValue(values1))))
                    {
                        properties.Add(pdc1[i]);
                    }
                }

                // add properties from values2
                for (int i = 0; i < pdc2.Count; i++)
                {
                    if ((TypeMerger.kTypeMergerPolicy == null) ||
                        (!TypeMerger.kTypeMergerPolicy.Ignored.Contains(pdc2[i].GetValue(values2))))
                    {
                        properties.Add(pdc2[i]);
                    }
                }

                // return array
                return (properties.ToArray());
            }

            /// <summary>Get the type of each property.</summary>
            private static Type[] GetTypes(PropertyDescriptor[] pdc)
            {
                List<Type> types = new List<Type>();

                for (int i = 0; i < pdc.Length; i++)
                {
                    types.Add(pdc[i].PropertyType);
                }

                return (types.ToArray());
            }

            /// <summary>Merge the values of the two types into an object array.</summary>
            private static object[] MergeValues(object values1, object values2)
            {
                PropertyDescriptorCollection pdc1 = TypeDescriptor.GetProperties(values1);
                PropertyDescriptorCollection pdc2 = TypeDescriptor.GetProperties(values2);

                List<object> values = new List<object>();

                for (int i = 0; i < pdc1.Count; i++)
                {
                    if ((TypeMerger.kTypeMergerPolicy == null) ||
                        (!TypeMerger.kTypeMergerPolicy.Ignored.Contains(pdc1[i].GetValue(values1))))
                    {
                        values.Add(pdc1[i].GetValue(values1));
                    }
                }

                for (int i = 0; i < pdc2.Count; i++)
                {
                    if ((TypeMerger.kTypeMergerPolicy == null) ||
                        (!TypeMerger.kTypeMergerPolicy.Ignored.Contains(pdc2[i].GetValue(values2))))
                    {
                        values.Add(pdc2[i].GetValue(values2));
                    }
                }

                return (values.ToArray());
            }

            /// <summary>Initialize static objects.</summary>
            private static void InitializeAssembly()
            {
                // check to see if we've already instantiated the static objects
                if (TypeMerger.kAsmBuilder == null)
                {
                    // create a new dynamic assembly
                    AssemblyName assembly = new AssemblyName();
                    assembly.Name = "AnonymousTypeExtensions";

                    // get the current application domain
                    AppDomain domain = Thread.GetDomain();

                    // get a module builder object
                    TypeMerger.kAsmBuilder = domain.DefineDynamicAssembly(assembly, AssemblyBuilderAccess.Run);
                    TypeMerger.kModBuilder = TypeMerger.kAsmBuilder.DefineDynamicModule(TypeMerger.kAsmBuilder.GetName().Name, false);
                }
            }

            /// <summary>Create a new Type definition from the list of PropertyDescriptors.</summary>
            private static Type CreateType(string name, PropertyDescriptor[] pdc)
            {
                // create TypeBuilder
                TypeBuilder typeBuilder = TypeMerger.CreateTypeBuilder(name);

                // get list of types for ctor definition
                Type[] types = TypeMerger.GetTypes(pdc);

                // create priate fields for use w/in the ctor body and properties
                FieldBuilder[] fields = TypeMerger.BuildFields(typeBuilder, pdc);

                // define / emit the Ctor
                TypeMerger.BuildCtor(typeBuilder, fields, types);

                // define / emit the properties
                TypeMerger.BuildProperties(typeBuilder, fields);

                // return Type definition
                return (typeBuilder.CreateType());
            }

            /// <summary>Create a type builder with the specified name.</summary>
            private static TypeBuilder CreateTypeBuilder(string typeName)
            {                
                // return new type builder
                return (TypeMerger.kModBuilder.DefineType(typeName, TypeAttributes.Public, typeof(object)));
            }

            /// <summary>Define/emit the ctor and ctor body.</summary>
            private static void BuildCtor(TypeBuilder typeBuilder, FieldBuilder[] fields, Type[] types)
            {
                // define ctor()
                ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, types);

                // build ctor()
                ILGenerator ctorGen = ctor.GetILGenerator();

                // create ctor that will assign to private fields
                for (int i = 0; i < fields.Length; i++)
                {
                    // load argument (parameter)
                    ctorGen.Emit(OpCodes.Ldarg_0);
                    ctorGen.Emit(OpCodes.Ldarg, (i + 1));

                    // store argument in field
                    ctorGen.Emit(OpCodes.Stfld, fields[i]);
                }

                // return from ctor()
                ctorGen.Emit(OpCodes.Ret);
            }

            /// <summary>Define fields based on the list of PropertyDescriptors.</summary>
            private static FieldBuilder[] BuildFields(TypeBuilder typeBuilder, PropertyDescriptor[] pdc)
            {
                List<FieldBuilder> fields = new List<FieldBuilder>();

                // build / define fields
                for (int i = 0; i < pdc.Length; i++)
                {
                    PropertyDescriptor pd = pdc[i];

                    // define field as '_[Name]' with the object's Type
                    FieldBuilder field = typeBuilder.DefineField(string.Format("_{0}", pd.Name), pd.PropertyType, FieldAttributes.Private);

                    // add to list of FieldBuilder objects
                    fields.Add(field);
                }

                return (fields.ToArray());
            }

            /// <summary>Build a list of Properties to match the list of private fields.</summary>
            private static void BuildProperties(TypeBuilder typeBuilder, FieldBuilder[] fields)
            {
                // build properties
                for (int i = 0; i < fields.Length; i++)
                {
                    // remove '_' from name for public property name
                    String propertyName = fields[i].Name.Substring(1);

                    // define the property
                    PropertyBuilder property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, fields[i].FieldType, null);
                    
                    // define 'Get' method only (anonymous types are read-only)
                    // MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig
                    MethodBuilder getMethod = typeBuilder.DefineMethod(string.Format("Get_{0}", propertyName), MethodAttributes.Public, fields[i].FieldType, Type.EmptyTypes);

                    // build 'Get' method
                    ILGenerator methGen = getMethod.GetILGenerator();

                    // method body
                    methGen.Emit(OpCodes.Ldarg_0);
                    // load value of corresponding field
                    methGen.Emit(OpCodes.Ldfld, fields[i]);
                    // return from 'Get' method
                    methGen.Emit(OpCodes.Ret);

                    // assign method to property 'Get'
                    property.SetGetMethod(getMethod);
                }
            }

            /// <summary>.</summary>
            public static TypeMergerPolicy Ignore(object value)
            {
                return (new TypeMergerPolicy(value));
            }

        }
        
        /// <summary>TypeMergerPolicy for ignoring properties.</summary>
        internal class TypeMergerPolicy
        {
            private IList m_Ignored;

            public IList Ignored
            {
                get { return (this.m_Ignored); }
            }

            public TypeMergerPolicy(IList ignored)
            {
                this.m_Ignored = ignored;
            }

            public TypeMergerPolicy(object ignoreValue)
            {
                this.m_Ignored = new List<object>();
                this.m_Ignored.Add(ignoreValue);
            }

            public TypeMergerPolicy Ignore(object value)
            {
                this.m_Ignored.Add(value);
                return (this);
            }

            public object MergeTypes(object values1, object values2)
            {
                return (TypeMerger.MergeTypes(values1, values2, this));
            }
        }
        
    }    
}

