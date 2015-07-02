using System;

namespace BLK10.Text.Extensions
{
    public static class StringBufferExtensions
    {
        public static bool IsEmptyOrWhitespace(this StringBuffer @this)
        {
            return (StringBuffer.IsNullOrWhitespace(@this));
        }
        

        #region "APPEND | APPENDLINE | APPENDFORMAT | APPENDLINEFORMAT | APPENDTABLINE | APPENDTABLINEFORMAT"

        /// <summary>Appends a specified string buffer to this instance.</summary>
        public static StringBuffer Append(this StringBuffer @this, StringBuffer value)
        {            
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.Append(@this, value, 0, value.Length));
        }

        /// <summary>Appends a specified string buffer to this instance.</summary>
        public static StringBuffer Append(this StringBuffer @this, StringBuffer value, int startIndex)
        {            
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.Append(@this, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Appends a specified string buffer to this instance.</summary>
        public static StringBuffer Append(this StringBuffer @this, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Append(value.ToCharArray(), startIndex, length));
        }


        /// <summary>Appends a specified string buffer followed by the default line terminator to this instance.</summary>
        public static StringBuffer AppendLine(this StringBuffer @this, StringBuffer value)
        {            
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.AppendLine(@this, value, 0, value.Length));
        }

        /// <summary>Appends a specified string buffer followed by the default line terminator to this instance.</summary>
        public static StringBuffer AppendLine(this StringBuffer @this, StringBuffer value, int startIndex)
        {            
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.AppendLine(@this, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Appends a specified string buffer followed by the default line terminator to this instance.</summary>
        public static StringBuffer AppendLine(this StringBuffer @this, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.AppendLine(value.ToCharArray(), startIndex, length));
        }


        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public static StringBuffer AppendFormat(this StringBuffer @this, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.AppendFormat(value.ToString(), args));            
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public static StringBuffer AppendFormat(this StringBuffer @this, StringBuffer value, IFormatProvider provider, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.AppendFormat(value.ToString(), provider, args)); 
        }


        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public static StringBuffer AppendLineFormat(this StringBuffer @this, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.AppendLineFormat(value.ToString(), args));     
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public static StringBuffer AppendLineFormat(this StringBuffer @this, StringBuffer value, IFormatProvider provider, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.AppendLineFormat(value.ToString(), provider, args));            
        }
        

        /// <summary>.</summary>
        public static StringBuffer AppendTabLine(this StringBuffer @this, int tabCount, char[] value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append(tabCount, '\t').AppendLine(value));
        }
        
        /// <summary>.</summary>
        public static StringBuffer AppendTabLine(this StringBuffer @this, int tabCount, string value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append(tabCount, '\t').AppendLine(value));            
        }

        /// <summary>.</summary>
        public static StringBuffer AppendTabLine(this StringBuffer @this, int tabCount, StringBuffer value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Append(tabCount, '\t').AppendLine(value.ToCharArray()));
        }
        
        
        /// <summary>.</summary>
        public static StringBuffer AppendTabLineFormat(this StringBuffer @this, int tabCount, string value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append(tabCount, '\t').AppendLineFormat(value, args));
        }

        /// <summary>.</summary>
        public static StringBuffer AppendTabLineFormat(this StringBuffer @this, int tabCount, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Append(tabCount, '\t').AppendLineFormat(value.ToString(), args));
        }

        #endregion


        #region "PREPEND | PREPENDLINE | PREPENDFORMAT | PREPENDLINEFORMAT | PREPENDTABLINE | PREPENDTABLINEFORMAT"

        /// <summary>Prepends a specified string buffer to this instance.</summary>
        public static StringBuffer Prepend(this StringBuffer @this, StringBuffer value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.Prepend(@this, value, 0, value.Length));
        }

        /// <summary>Prepends a specified string buffer to this instance.</summary>
        public static StringBuffer Prepend(this StringBuffer @this, StringBuffer value, int startIndex)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.Prepend(@this, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Prepends a specified string buffer to this instance.</summary>
        public static StringBuffer Prepend(this StringBuffer @this, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Prepend(value.ToString(), startIndex, length));
        }


        /// <summary>Prepends a specified string buffer followed by the default line terminator to this instance.</summary>
        public static StringBuffer PrependLine(this StringBuffer @this, StringBuffer value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.PrependLine(@this, value, 0, value.Length));
        }

        /// <summary>Prepends a specified string buffer followed by the default line terminator to this instance.</summary>
        public static StringBuffer PrependLine(this StringBuffer @this, StringBuffer value, int startIndex)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.PrependLine(@this, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Prepends a specified string buffer followed by the default line terminator to this instance.</summary>
        public static StringBuffer PrependLine(this StringBuffer @this, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.PrependLine(value.ToString(), startIndex, length));
        }


        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public static StringBuffer PrependFormat(this StringBuffer @this, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.PrependFormat(value.ToString(), args));
        }

        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public static StringBuffer PrependFormat(this StringBuffer @this, StringBuffer value, IFormatProvider provider, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.PrependFormat(value.ToString(), provider, args));
        }


        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public static StringBuffer PrependLineFormat(this StringBuffer @this, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.PrependLineFormat(value.ToString(), args));            
        }

        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public static StringBuffer PrependLineFormat(this StringBuffer @this, StringBuffer value, IFormatProvider provider, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.PrependLineFormat(value.ToString(), provider, args));
        }


        /// <summary>.</summary>
        public static StringBuffer PrependTabLine(this StringBuffer @this, int tabCount, char[] value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Prepend('\t', tabCount).InsertLine(tabCount, value));
        }

        /// <summary>.</summary>
        public static StringBuffer PrependTabLine(this StringBuffer @this, int tabCount, string value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Prepend('\t', tabCount).InsertLine(tabCount, value));
        }

        /// <summary>.</summary>
        public static StringBuffer PrependTabLine(this StringBuffer @this, int tabCount, StringBuffer value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Prepend('\t', tabCount).InsertLine(tabCount, value.ToCharArray()));
        }


        /// <summary>.</summary>
        public static StringBuffer PrependTabLineFormat(this StringBuffer @this, int tabCount, string value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Prepend('\t', tabCount).InsertLineFormat(tabCount, value, args));
        }

        /// <summary>.</summary>
        public static StringBuffer PrependTabLineFormat(this StringBuffer @this, int tabCount, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Prepend('\t', tabCount).InsertLineFormat(tabCount, value.ToString(), args));
        }
        
        #endregion


        #region "INSERT | INSERTLINE | INSERTFORMAT | INSERTLINEFORMAT | INSERTTABLINE | INSERTTABLINEFORMAT"

        /// <summary>Inserts a specified string buffer into this instance at the specified character position.</summary>
        public static StringBuffer Insert(this StringBuffer @this, int index, StringBuffer value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.Insert(@this, index, value, 0, value.Length));
        }

        /// <summary>Inserts a specified string buffer into this instance at the specified character position.</summary>
        public static StringBuffer Insert(this StringBuffer @this, int index, StringBuffer value, int startIndex)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.Insert(@this, index, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Inserts a specified string buffer into this instance at the specified character position.</summary>
        public static StringBuffer Insert(this StringBuffer @this, int index, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Insert(index, value.ToCharArray(), startIndex, length));
        }


        /// <summary>Inserts a specified string buffer followed by the default line terminator into this instance at the specified character position.</summary>
        public static StringBuffer InsertLine(this StringBuffer @this, int index, StringBuffer value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.InsertLine(@this, index, value, 0, value.Length));
        }

        /// <summary>Inserts a specified string buffer followed by the default line terminator into this instance at the specified character position.</summary>
        public static StringBuffer InsertLine(this StringBuffer @this, int index, StringBuffer value, int startIndex)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.InsertLine(@this, index, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Inserts a specified string buffer followed by the default line terminator into this instance at the specified character position.</summary>
        public static StringBuffer InsertLine(this StringBuffer @this, int index, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.InsertLine(index, value.ToCharArray(), startIndex, length));
        }


        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public static StringBuffer InsertFormat(this StringBuffer @this, int index, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.InsertFormat(index, value.ToString(), args));
        }

        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public static StringBuffer InsertFormat(this StringBuffer @this, int index, StringBuffer value, IFormatProvider provider, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.InsertFormat(index, value.ToString(), provider, args));
        }


        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public static StringBuffer InsertLineFormat(this StringBuffer @this, int index, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.InsertLineFormat(index, value.ToString(), args));
        }

        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public static StringBuffer InsertLineFormat(this StringBuffer @this, int index, StringBuffer value, IFormatProvider provider, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.InsertLineFormat(index, value.ToString(), provider, args));
        }


        /// <summary>.</summary>
        public static StringBuffer InsertTabLine(this StringBuffer @this, int index, int tabCount, char[] value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Insert(index, '\t', tabCount).InsertLine(index + tabCount, value));
        }

        /// <summary>.</summary>
        public static StringBuffer InsertTabLine(this StringBuffer @this, int index, int tabCount, string value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Insert(index, '\t', tabCount).InsertLine(index + tabCount, value));
        }

        /// <summary>.</summary>
        public static StringBuffer InsertTabLine(this StringBuffer @this, int index, int tabCount, StringBuffer value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Insert(index, '\t', tabCount).InsertLine(index + tabCount, value.ToCharArray()));
        }


        /// <summary>.</summary>
        public static StringBuffer InsertTabLineFormat(this StringBuffer @this, int index, int tabCount, string value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Insert(index, '\t', tabCount).InsertLineFormat(index + tabCount, value, args));
        }

        /// <summary>.</summary>
        public static StringBuffer InsertTabLineFormat(this StringBuffer @this, int index, int tabCount, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.Insert(index, '\t', tabCount).InsertLineFormat(index + tabCount, value.ToString(), args));
        }
        
        #endregion


        #region "SUBSTITUTE"

        /// <summary>Substitutes all occurrences of a specified character with another specified string buffer in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, char oldValue, StringBuffer newValue)
        {
            return (StringBufferExtensions.Substitute(@this, oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified character with another specified string buffer in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, char oldValue, StringBuffer newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.Substitute(oldValue, newValue.ToString(), ignoreCase));            
        }

        /// <summary>Substitutes all occurrences of a specified string with another specified string buffer in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, string oldValue, StringBuffer newValue)
        {
            return (StringBufferExtensions.Substitute(@this, oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified string with another specified string buffer in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, string oldValue, StringBuffer newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.Substitute(oldValue, newValue.ToString(), ignoreCase));
        }
        
        /// <summary>Substitutes all occurrences of a specified string buffer with another specified character in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, StringBuffer oldValue, char newValue)
        {
            return (StringBufferExtensions.Substitute(@this, oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified string buffer with another specified character in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, StringBuffer oldValue, char newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            return (@this.Substitute(oldValue.ToString(), newValue, ignoreCase));
        }

        /// <summary>Substitutes all occurrences of a specified string buffer with another specified string in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, StringBuffer oldValue, string newValue)
        {
            return (StringBufferExtensions.Substitute(@this, oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified string buffer with another specified string in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, StringBuffer oldValue, string newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            return (@this.Substitute(oldValue.ToString(), newValue, ignoreCase));
        }

        /// <summary>Substitutes all occurrences of a specified string buffer with another specified string buffer in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, StringBuffer oldValue, StringBuffer newValue)
        {
            return (StringBufferExtensions.Substitute(@this, oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified string buffer with another specified string buffer in this instance.</summary>
        public static StringBuffer Substitute(this StringBuffer @this, StringBuffer oldValue, StringBuffer newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.Substitute(oldValue.ToString(), newValue.ToString(), ignoreCase));
        }

        #endregion


        #region "REPLACE | REPLACERANGE | REPLACEBEFORE | REPLACEAFTER | REPLACEINSIDE"

        /// <summary>Replaces the first or the last specified character by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, char oldValue, ESearch occurrence, StringBuffer newValue)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, 0, @this.Length, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, char oldValue, ESearch occurrence, StringBuffer newValue, int index)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, @this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, char oldValue, ESearch occurrence, StringBuffer newValue, int index, int length)
        {            
            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, char oldValue, ESearch occurrence, StringBuffer newValue, int index, int length, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.Replace(oldValue, occurrence, newValue.ToString(), index, length, ignoreCase));
        }


        /// <summary>Replaces the first or the last specified string by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, string oldValue, ESearch occurrence, StringBuffer newValue)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, 0, @this.Length, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, string oldValue, ESearch occurrence, StringBuffer newValue, int index)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, @this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, string oldValue, ESearch occurrence, StringBuffer newValue, int index, int length)
        {
            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, string oldValue, ESearch occurrence, StringBuffer newValue, int index, int length, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.Replace(oldValue, occurrence, newValue.ToString(), index, length, ignoreCase));
        }


        /// <summary>Replaces the first or the last specified string buffer by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, char newValue)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, 0, @this.Length, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, char newValue, int index)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, @this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, char newValue, int index, int length)
        {
            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, char newValue, int index, int length, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            return (@this.Replace(oldValue.ToString(), occurrence, newValue, index, length, ignoreCase));
        }


        /// <summary>Replaces the first or the last specified string buffer by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, string newValue)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, 0, @this.Length, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, string newValue, int index)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, @this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, string newValue, int index, int length)
        {
            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, string newValue, int index, int length, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            return (@this.Replace(oldValue.ToString(), occurrence, newValue, index, length, ignoreCase));
        }


        /// <summary>Replaces the first or the last specified string buffer by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, StringBuffer newValue)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, 0, @this.Length, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, StringBuffer newValue, int index)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, @this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, StringBuffer newValue, int index, int length)
        {
            return (StringBufferExtensions.Replace(@this, oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified string buffer by another specified string buffer in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer Replace(this StringBuffer @this, StringBuffer oldValue, ESearch occurrence, StringBuffer newValue, int index, int length, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.Replace(oldValue.ToString(), occurrence, newValue.ToString(), index, length, ignoreCase));
        }

        
        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, char value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceRange(@this, index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, char value, ESearch occurence, StringBuffer newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceRange(index, value, occurence, newValue.ToString(), ignoreCase));
        }


        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, string value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceRange(@this, index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, string value, ESearch occurence, StringBuffer newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceRange(index, value, occurence, newValue.ToString(), ignoreCase));
        }


        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, char newValue)
        {
            return (StringBufferExtensions.ReplaceRange(@this, index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, char newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.ReplaceRange(index, value.ToString(), occurence, newValue, ignoreCase));
        }


        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, string newValue)
        {
            return (StringBufferExtensions.ReplaceRange(@this, index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, string newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.ReplaceRange(index, value.ToString(), occurence, newValue, ignoreCase));
        }


        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceRange(@this, index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer ReplaceRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, StringBuffer newValue, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceRange(index, value.ToString(), occurence, newValue.ToString(), ignoreCase));
        }

        
        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, char value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, char value, ESearch occurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, char value, ESearch occurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceBefore(value, occurence, newValue.ToString(), index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, string value, ESearch occurence, StringBuffer newValue)
        {            
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, string value, ESearch occurence, StringBuffer newValue, int index)
        {            
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, string value, ESearch occurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceBefore(value, occurence, newValue.ToString(), index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, char newValue)
        {
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, char newValue, int index)
        {
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, char newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.ReplaceBefore(value.ToString(), occurence, newValue, index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, string newValue)
        {            
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, string newValue, int index)
        {            
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, string newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.ReplaceBefore(value.ToString(), occurence, newValue, index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceBefore(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceBefore(value.ToString(), occurence, newValue.ToString(), index, ignoreCase));
        }

        
        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, char value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, char value, ESearch occurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, char value, ESearch occurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceAfter(value, occurence, newValue.ToString(), index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, string value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, string value, ESearch occurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, string value, ESearch occurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceAfter(value, occurence, newValue.ToString(), index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, char newValue)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, char newValue, int index)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, char newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.ReplaceAfter(value.ToString(), occurence, newValue, index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, string newValue)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, string newValue, int index)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, string newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.ReplaceAfter(value.ToString(), occurence, newValue, index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceAfter(@this, value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceAfter(value.ToString(), occurence, newValue.ToString(), index, ignoreCase));
        }
        
        
        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, char prev, ESearch prevOccurence, char next, ESearch nextOccurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, char prev, ESearch prevOccurence, char next, ESearch nextOccurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, char prev, ESearch prevOccurence, char next, ESearch nextOccurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue.ToString(), index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, char newValue)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, char newValue, int index)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, char newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (prev == null)
            {
                throw new ArgumentNullException("prev");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            return (@this.ReplaceInside(prev.ToString(), prevOccurence, next.ToString(), nextOccurence, newValue, index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, string newValue)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, string newValue, int index)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, string newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (prev == null)
            {
                throw new ArgumentNullException("prev");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            return (@this.ReplaceInside(prev.ToString(), prevOccurence, next.ToString(), nextOccurence, newValue, index, ignoreCase));
        }


        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, StringBuffer newValue)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, StringBuffer newValue, int index)
        {
            return (StringBufferExtensions.ReplaceInside(@this, prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Between the first and the second occurence of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer ReplaceInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, StringBuffer newValue, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (prev == null)
            {
                throw new ArgumentNullException("prev");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            if (newValue == null)
            {
                throw new ArgumentNullException("newValue");
            }

            return (@this.ReplaceInside(prev.ToString(), prevOccurence, next.ToString(), nextOccurence, newValue.ToString(), index, ignoreCase));
        }
        
        #endregion


        #region "SUBSTRINGRANGE | SUBSTRINGBEFORE | SUBSTRINGAFTER | SUBSTRINGINSIDE | SUBSTRINGOUTSIDE"

        /// <summary>Retrieves a substring from this instance, from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer SubstringRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence)
        {
            return (StringBufferExtensions.SubstringRange(@this, index, value, occurence, false));
        }

        /// <summary>Retrieves a substring from this instance, from the specified index position to the first or last specified matching string buffer position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public static StringBuffer SubstringRange(this StringBuffer @this, int index, StringBuffer value, ESearch occurence, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.SubstringRange(index, value.ToString(), occurence, ignoreCase));
        }


        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringBefore(this StringBuffer @this, StringBuffer value, ESearch occurence)
        {
            return (StringBufferExtensions.SubstringBefore(@this, value, occurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, int index)
        {
            return (StringBufferExtensions.SubstringBefore(@this, value, occurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringBefore(this StringBuffer @this, StringBuffer value, ESearch occurence, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.SubstringBefore(value.ToString(), occurence, index, ignoreCase));
        }


        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified string buffer.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringAfter(this StringBuffer @this, StringBuffer value, ESearch occurence)
        {
            return (StringBufferExtensions.SubstringAfter(@this, value, occurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, int index)
        {
            return (StringBufferExtensions.SubstringAfter(@this, value, occurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified string buffer. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringAfter(this StringBuffer @this, StringBuffer value, ESearch occurence, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.SubstringAfter(value.ToString(), occurence, index, ignoreCase));
        }


        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified string buffers.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence)
        {
            return (StringBufferExtensions.SubstringInside(@this, prev, prevOccurence, next, nextOccurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, int index)
        {
            return (StringBufferExtensions.SubstringInside(@this, prev, prevOccurence, next, nextOccurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringInside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (prev == null)
            {
                throw new ArgumentNullException("prev");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            return (@this.SubstringInside(prev.ToString(), prevOccurence, next.ToString(), nextOccurence, index, ignoreCase));
        }


        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified string buffers.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringOutside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence)
        {
            return (StringBufferExtensions.SubstringOutside(@this, prev, prevOccurence, next, nextOccurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringOutside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, int index)
        {
            return (StringBufferExtensions.SubstringOutside(@this, prev, prevOccurence, next, nextOccurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified string buffers. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public static StringBuffer SubstringOutside(this StringBuffer @this, StringBuffer prev, ESearch prevOccurence, StringBuffer next, ESearch nextOccurence, int index, bool ignoreCase)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (prev == null)
            {
                throw new ArgumentNullException("prev");
            }

            if (next == null)
            {
                throw new ArgumentNullException("next");
            }

            return (@this.SubstringOutside(prev.ToString(), prevOccurence, next.ToString(), nextOccurence, index, ignoreCase));
        }
        
        #endregion


        #region "FROMSTRINGBUFFER"

        /// <summary>Clears this instance and append a copy of the string buffer.</summary>
        public static StringBuffer FromStringBuffer(this StringBuffer @this, StringBuffer value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.FromStringBuffer(@this, value, 0, value.Length));
        }

        /// <summary>Clears this instance and append a copy of the string buffer.</summary>
        public static StringBuffer FromStringBuffer(this StringBuffer @this, StringBuffer value, int startIndex)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (StringBufferExtensions.FromStringBuffer(@this, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Clears this instance and append a copy of the string buffer.</summary>
        public static StringBuffer FromStringBuffer(this StringBuffer @this, StringBuffer value, int startIndex, int length)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return (@this.FromCharArray(value.ToCharArray(), startIndex, length));
        }

        #endregion

    }
}
