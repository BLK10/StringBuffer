using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;


namespace BLK10.Text
{
    public abstract class StringBufferComparer : IComparer, IEqualityComparer, IComparer<StringBuffer>, IEqualityComparer<StringBuffer>
    {
        private static readonly StringBufferComparer kInvariantCulture           = new CultureAwareComparer(CultureInfo.InvariantCulture, false);
        private static readonly StringBufferComparer kInvariantCultureIgnoreCase = new CultureAwareComparer(CultureInfo.InvariantCulture, true);
        private static readonly StringBufferComparer kOrdinal                    = new OrdinalComparer(false);
        private static readonly StringBufferComparer kOrdinalIgnoreCase          = new OrdinalComparer(true);


        public static StringBufferComparer InvariantCulture
        {
            get { return (StringBufferComparer.kInvariantCulture); }
        }

        public static StringBufferComparer InvariantCultureIgnoreCase
        {
            get { return (StringBufferComparer.kInvariantCultureIgnoreCase); }
        }

        public static StringBufferComparer CurrentCulture
        {
            get { return (new CultureAwareComparer(CultureInfo.CurrentCulture, false)); }
        }

        public static StringBufferComparer CurrentCultureIgnoreCase
        {
            get { return (new CultureAwareComparer(CultureInfo.CurrentCulture, true)); }
        }

        public static StringBufferComparer Ordinal
        {
            get { return (StringBufferComparer.kOrdinal); }
        }

        public static StringBufferComparer OrdinalIgnoreCase
        {
            get { return (StringBufferComparer.kOrdinalIgnoreCase); }
        }

        public static StringBufferComparer Create(CultureInfo culture, bool ignoreCase)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            return (new CultureAwareComparer(culture, ignoreCase));
        }

       
        public int Compare(object x, object y)
        {
            if (x == y)    return (0);
            if (x == null) return (-1);
            if (y == null) return (1);

            StringBuffer sb1 = x as StringBuffer;
            if (sb1 != null)
            {
                StringBuffer sb2 = y as StringBuffer;
                if (sb2 != null)
                {
                    return Compare(sb1, sb2);
                }
            }

            IComparable ia = x as IComparable;
            if (ia != null)
            {
                return (ia.CompareTo(y));
            }

            throw new ArgumentException("Does not impement IComparable");
        }

        public new bool Equals(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;

            StringBuffer sb1 = x as StringBuffer;
            if (sb1 != null)
            {
                StringBuffer sb2 = y as StringBuffer;
                if (sb2 != null)
                {
                    return Equals(sb1, sb2);
                }
            }

            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            StringBuffer sb = obj as StringBuffer;
            
            if (sb != null)
            {
                return GetHashCode(sb);
            }

            return obj.GetHashCode();
        }
        

        public abstract int Compare(StringBuffer x, StringBuffer y);
        public abstract bool Equals(StringBuffer x, StringBuffer y);
        public abstract int GetHashCode(StringBuffer obj);
    }

    internal sealed class OrdinalComparer : StringBufferComparer
    {
        private bool m_IgnoreCase;

        internal OrdinalComparer() : this(false) { }
        internal OrdinalComparer(bool ignoreCase)
        {
            this.m_IgnoreCase = ignoreCase;
        }


        public override int Compare(StringBuffer x, StringBuffer y)
        {
            if (Object.ReferenceEquals(x, y)) return (0);
            if (x == null) return (-1);
            if (y == null) return (1);

            if (this.m_IgnoreCase)
            {
                return (StringBuffer.Compare(x, y, StringComparison.OrdinalIgnoreCase));
            }

            return (StringBuffer.Compare(x, y, StringComparison.Ordinal));
        }

        public override bool Equals(StringBuffer x, StringBuffer y)
        {
            if (Object.ReferenceEquals(x, y)) return (true);
            if ((x == null) || (y == null))   return (false);

            if (this.m_IgnoreCase)
            {
                if (x.Length != y.Length)
                {
                    return (false);
                }

                return (StringBuffer.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0);                
            }

            return x.Equals(y);            
        }

        public override int GetHashCode(StringBuffer obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (this.m_IgnoreCase)
            {
                return (GetCaseInsensitiveHashCode(obj));                
            }

            return obj.GetHashCode();
        }


        public override bool Equals(Object obj)
        {            
            OrdinalComparer comparer = obj as OrdinalComparer;

            if (comparer == null)
            {
                return (false);
            }

            return (this.m_IgnoreCase == comparer.m_IgnoreCase);
        }

        public override int GetHashCode()
        {            
            int hashCode = ("OrdinalComparer").GetHashCode();

            return ((this.m_IgnoreCase) ? (~hashCode) : hashCode);
        }


        
        private int GetCaseInsensitiveHashCode(StringBuffer str)
        {            
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            // This code assumes that ASCII casing is safe for whatever context is passed in.
            // this is true today, because we only ever call these methods on Invariant.  It would be ideal to refactor
            // these methods so they were correct by construction and we could only ever use Invariant.

            uint hash = 5381;
            uint c;

            // Note: We assume that str contains only ASCII characters until
            // we hit a non-ASCII character to optimize the common case.
            for (int i = 0; i < str.Length; i++)
            {
                c = str[i];
                if (c >= 0x80)
                {
                    return GetCaseInsensitiveHashCodeSlow(str);
                }

                // If we have a lowercase character, ANDing off 0x20
                // will make it an uppercase character.
                if ((c - 'a') <= ('z' - 'a'))
                {
                    c = (uint)((int)c & ~0x20);
                }

                hash = ((hash << 5) + hash) ^ c;
            }

            return ((int)hash);
        }

        private int GetCaseInsensitiveHashCodeSlow(StringBuffer str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            StringBuffer upper = str.ToUpper();

            uint hash = 5381;
            uint c;

            for (int i = 0; i < upper.Length; i++)
            {
                c = upper[i];
                hash = ((hash << 5) + hash) ^ c;
            }

            return ((int)hash);
        }
    
    }

    internal sealed class CultureAwareComparer : StringBufferComparer
    {
        private bool m_IgnoreCase;
        private CompareInfo m_CompareInfo;

        internal CultureAwareComparer(CultureInfo culture) : this(culture, false) { }
        internal CultureAwareComparer(CultureInfo culture, bool ignoreCase) : this(culture.CompareInfo, false) { }
        internal CultureAwareComparer(CompareInfo compare, bool ignoreCase)
        {
            this.m_IgnoreCase = ignoreCase;
            this.m_CompareInfo = compare;

        }


        public override int Compare(StringBuffer x, StringBuffer y)
        {
            if (Object.ReferenceEquals(x, y)) return (0);            
            if (x == null) return (-1);
            if (y == null) return (1);

            return (this.m_CompareInfo.Compare(x.ToString(), y.ToString(), ((this.m_IgnoreCase) ? CompareOptions.IgnoreCase : CompareOptions.None)));
        }

        public override bool Equals(StringBuffer x, StringBuffer y)
        {
            if (Object.ReferenceEquals(x, y)) return (true);
            if ((x == null) || (y == null))   return (false);

            return (this.m_CompareInfo.Compare(x.ToString(), y.ToString(), ((this.m_IgnoreCase) ? CompareOptions.IgnoreCase : CompareOptions.None)) == 0);
        }

        public override int GetHashCode(StringBuffer obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            CompareOptions options = CompareOptions.None;

            if (this.m_IgnoreCase)
            {
                options |= CompareOptions.IgnoreCase;
            }

            return (this.m_CompareInfo.GetSortKey(obj.ToString(), options).GetHashCode());
        }


        public override bool Equals(Object obj)
        {
            CultureAwareComparer comparer = obj as CultureAwareComparer;

            if (comparer == null)
            {
                return (false);
            }

            return ((this.m_IgnoreCase == comparer.m_IgnoreCase) && (this.m_CompareInfo.Equals(comparer.m_CompareInfo)));
        }

        public override int GetHashCode()
        {
            int hashCode = this.m_CompareInfo.GetHashCode();

            return ((this.m_IgnoreCase) ? (~hashCode) : hashCode);
        }

    }

}