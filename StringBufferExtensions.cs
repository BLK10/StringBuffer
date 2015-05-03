using System;

namespace BLK10.Text
{
    public static class StringBufferExtensions
    {
        /// <summary>.</summary>
        public static StringBuffer AppendTabbedLine(this StringBuffer @this, int tabCount, char[] value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append('\t', tabCount).AppendLine(value));
        }
        
        /// <summary>.</summary>
        public static StringBuffer AppendTabbedLine(this StringBuffer @this, int tabCount, string value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append('\t', tabCount).AppendLine(value));            
        }
        
        /// <summary>.</summary>
        public static StringBuffer AppendTabbedLine(this StringBuffer @this, int tabCount, StringBuffer value)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append('\t', tabCount).AppendLine(value));
        }
        
        
        /// <summary>.</summary>
        public static StringBuffer AppendTabbedLineFormat(this StringBuffer @this, int tabCount, string value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append('\t', tabCount).AppendLineFormat(value, args));
        }
        
        /// <summary>.</summary>
        public static StringBuffer AppendTabbedLineFormat(this StringBuffer @this, int tabCount, StringBuffer value, params object[] args)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("StringBuffer could be null.");
            }

            return (@this.Append('\t', tabCount).AppendLineFormat(value, args));
        }
        
    }
}
