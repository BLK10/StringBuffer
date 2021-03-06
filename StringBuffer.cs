﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using Object = System.Object;


namespace BLK10.Text
{    
    /// <summary>The type of search to perform.</summary>
    public enum ESearch
    {
        FirstOccurrence = 0,
        LastOccurrence
    }

    
    public class StringBuffer : IEnumerator, IEnumerable, IComparable, IComparable<StringBuffer>, IEquatable<StringBuffer>
    {
        // To keep the chunk arrays out of the large object heap set the MAX_CHUNK_SIZE lesser than 40K chars(85 K-bytes) .
        // Making the MAX_CHUNK_SIZE big means less allocation, but also more waste in unused characters and slower Inserts/Removes.    
        private const int MAX_CHUNK_SIZE   = 8000;
        private const int DEFAULT_CAPACITY = 32;
        
        // Whitespaces category, see url: msdn.microsoft.com/en-us/library/system.char.iswhitespace.aspx
        private static readonly char[] Whitespace = new char[25]
        {
            // SpaceSeparator category
            '\u0020', // SPACE
            '\u1680', // OGHAM SPACE MARK
            '\u2000', // EN QUAD
            '\u2001', // EM QUAD
            '\u2002', // EN SPACE
            '\u2003', // EM SPACE
            '\u2004', // THREE-PER-EM SPACE
            '\u2005', // FOUR-PER-EM SPACE
            '\u2006', // SIX-PER-EM SPACE
            '\u2007', // FIGURE SPACE
            '\u2008', // PUNCTUATION SPACE
            '\u2009', // THIN SPACE
            '\u200A', // HAIR SPACE
            '\u202F', // NARROW NO-BREAK SPACE
            '\u205F', // MEDIUM MATHEMATICAL SPACE
            '\u3000', // IDEOGRAPHIC SPACE

            // LineSeparator category
            '\u2028', // LINE SEPARATOR

            // ParagraphSeparator category
            '\u2029', // PARAGRAPH SEPARATOR

            // Others...
            '\u0009', // CHARACTER TABULATION
            '\u000A', // LINE FEED
            '\u000B', // LINE TABULATION
            '\u000C', // FORM FEED
            '\u000D', // CARRIAGE RETURN
            '\u0085', // NEXT LINE
            '\u00A0'  // NO-BREAK SPACE
        };

        internal static readonly char[] Crlf = new char[] { '\r', '\n' };
        
        public readonly int _maxCapacity;
        private StringNode  _chunkHead;
        private StringNode  _chunkTail;
        private string      _cachedStr;
        private int         _position;        
        private bool        _failed;

        private enum FormatState
        {
            OutsideExpression = 0,
            OpenBracket,
            InsideExpression,
            CloseBracket
        }
                

        #region "CTOR"

        /// <summary>Initializes a new instance of the StringBuffer class.</summary>
        public StringBuffer()
            : this(new char[0], 0, 0, DEFAULT_CAPACITY, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified capacity. </summary>
        public StringBuffer(int capacity)
            : this(new char[0], 0, 0, capacity, int.MaxValue) { }
        

        /// <summary>Initializes a new instance of the StringBuffer class using the specified string.</summary>    
        public StringBuffer(string value)            
            : this((value != null) ? value.ToCharArray() : null, 0, ((value != null) ? value.Length : 0), DEFAULT_CAPACITY, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified string.</summary>     
        public StringBuffer(string value, int capacity)
            : this((value != null) ? value.ToCharArray() : null, 0, ((value != null) ? value.Length : 0), capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified string.</summary>    
        public StringBuffer(string value, int capacity, int maxCapacity)
            : this((value != null) ? value.ToCharArray() : null, 0, ((value != null) ? value.Length : 0), capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified string.</summary>     
        public StringBuffer(string value, int startIndex, int length, int capacity)
            : this((value != null) ? value.ToCharArray() : null, startIndex, length, capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified string.</summary> 
        public StringBuffer(string value, int startIndex, int length, int capacity, int maxCapacity)
            : this((value != null) ? value.ToCharArray() : null, startIndex, length, capacity, maxCapacity) { }


        /// <summary>Initializes a new instance of the StringBuffer class using the specified bytes array.</summary>
        public StringBuffer(byte[] bytes)
            : this((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes) : null, 0, ((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes).Length : 0), DEFAULT_CAPACITY, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified bytes array.</summary>    
        public StringBuffer(byte[] bytes, int capacity)
            : this((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes) : null, 0, ((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes).Length : 0), capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified bytes array.</summary>    
        public StringBuffer(byte[] bytes, int capacity, int maxCapacity)
            : this((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes) : null, 0, ((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes).Length : 0), capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified bytes array.</summary>    
        public StringBuffer(byte[] bytes, int startIndex, int length, int capacity)
            : this((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes) : null, startIndex, length, capacity, int.MaxValue) { }
        
        /// <summary>Initializes a new instance of the StringBuffer class using the specified bytes array.</summary>
        public StringBuffer(byte[] bytes, int startIndex, int length, int capacity, int maxCapacity)
            : this((bytes != null) ? System.Text.Encoding.Default.GetChars(bytes) : null, startIndex, length, capacity, maxCapacity) { }


        /// <summary>Initializes a new instance of the StringBuffer class using the specified character.</summary>
        public StringBuffer(char c)            
            : this(new char[1] { c }, 0, 1, DEFAULT_CAPACITY, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified characters array.</summary>
        public StringBuffer(char[] chars)
            : this(chars, 0, ((chars != null) ? chars.Length : 0), DEFAULT_CAPACITY, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified characters array.</summary>    
        public StringBuffer(char[] chars, int capacity)
            : this(chars, 0, ((chars != null) ? chars.Length : 0), capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class using the specified characters array.</summary>    
        public StringBuffer(char[] chars, int capacity, int maxCapacity)
            : this(chars, 0, ((chars != null) ? chars.Length : 0), capacity, int.MaxValue) { }

        /// <summary>Initializes a new instance of the StringBuffer class from the specified characters array.</summary>    
        public StringBuffer(char[] chars, int startIndex, int length, int capacity)
            : this(chars, startIndex, length, capacity, int.MaxValue) { }


        /// <summary>Initializes a new instance of the StringBuffer class from the specified characters array.</summary>
        public StringBuffer(char[] chars, int startIndex, int length, int capacity, int maxCapacity)
        {
            if (maxCapacity < 0)
                throw new ArgumentOutOfRangeException("maxCapacity");

            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            if (chars == null)
                chars = new char[0];

            if ((startIndex + length) > chars.Length)
                throw new ArgumentOutOfRangeException("length");


            if (capacity == 0)
            {
                capacity = DEFAULT_CAPACITY;
            }
            else if (capacity > MAX_CHUNK_SIZE)
            {
                capacity = MAX_CHUNK_SIZE;
            }
            else if (capacity < length)
            {
                capacity = DEFAULT_CAPACITY;
                while (length > capacity)
                {
                    capacity <<= 1;

                    if ((capacity > MAX_CHUNK_SIZE) || (capacity > int.MaxValue))
                    {
                        capacity = Math.Min(MAX_CHUNK_SIZE, int.MaxValue);
                        break;
                    }
                }
            }
            else
            {
                capacity = length;
            }

            this._maxCapacity = Math.Min(Math.Max(capacity, maxCapacity), int.MaxValue);
            this._cachedStr   = null;
            this._chunkHead   = StringNode.CreateInstance(0, 0, capacity, null);
            this._chunkTail   = this._chunkHead;
            this._position    = -1;            
            this._failed      = false;

            this.InternalAppend(chars, startIndex, length);
        }
                
        #endregion


        #region "PROPERTIES"

        /// <summary>Gets the maximum capacity of this instance.</summary>
        public int MaxCapacity
        {
            get { return (this._maxCapacity); }
        }

        /// <summary>Gets or sets the maximum number of characters that can be contained in the memory allocated by this instance.</summary>
        public int Capacity
        {
            get { return (this._chunkTail._offset + this._chunkTail._chars.Length); }
            set
            {
                if ((value < 0) || (value > this._maxCapacity))
                    throw new ArgumentOutOfRangeException("value");
                
                // shrink capacity not allowed
                if (value < this.Length)
                    return;

                if (value != this.Capacity)
                {
                    int chunkCapacity = value - this._chunkTail._offset;
                    if (value > this.Capacity)
                        chunkCapacity = ((MAX_CHUNK_SIZE - chunkCapacity) < 0) ? MAX_CHUNK_SIZE : chunkCapacity;

                    Array.Resize(ref this._chunkTail._chars, chunkCapacity);
                }
            }
        }

        /// <summary>Gets or Sets the length of this instance.</summary>
        public int Length
        {
            get { return (this._chunkTail._offset + this._chunkTail._length); }
            set
            {
                if ((value < 0) || (value > this._maxCapacity))
                    throw new ArgumentOutOfRangeException("value");

                this.CheckEnumeration();

                if (value == this.Length)
                    return;

                if (value == 0)
                {
                    this.InternalClear();
                }
                else if (value > this.Length)
                {                    
                    int diff = value - this.Length;                    
                    this.InternalAppend(diff);
                }
                else
                {
                    this.InternalShrink(value);                    
                }
            }
        }

        /// <summary>Gets or sets the character at the specified character position in this instance.</summary>    
        public char this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.Length))
                    throw new IndexOutOfRangeException("index");

                var chunk = this.FindChunkForIndex(index);
                int local = index - chunk._offset;

                if (local >= chunk._length)
                    throw new Exception("local index out of range");

                return (chunk._chars[local]);
            }
            set
            {
                if ((index < 0) || (index >= this.Length))
                    throw new IndexOutOfRangeException("index");

                this.CheckEnumeration();
                this.InternalAssign(index, value);
            }
        }

        /// <summary>Gets a string at the specified character position with the specified length in this instance.</summary>    
        public string this[int index, int length]
        {
            get
            {
                if ((index < 0) || (index >= this.Length))
                    throw new IndexOutOfRangeException("index");

                if ((length < 0) || ((index + length) > this.Length))
                    throw new IndexOutOfRangeException("length");

                if (this._cachedStr != null)
                    return (this._cachedStr.Substring(index, length));

                char[] result = new char[this.Length];
                var    chunk  = this._chunkHead;

                int i = 0;
                while (chunk != null)
                {
                    int len = Math.Min(chunk._chars.Length, chunk._length);
                    Array.Copy(chunk._chars, 0, result, i, len);
                    i += len;
                    chunk = chunk._next;
                }

                this._cachedStr = new string(result);

                return (this._cachedStr.Substring(index, length));
            }            
        }
        
        /// <summary>Gets the state of the previous Replace or Substring methods call.</summary>
        public bool Failed
        {
            get { return (this._failed); }
        }

        /// <summary>.</summary>
        public bool IsEmpty
        {
            get { return ((this._chunkTail._offset + this._chunkTail._length) == 0); }
        }
        
        #endregion


        #region "CLEAR"

        /// <summary>Removes all the characters from the current StringBuffer instance.</summary>    
        public StringBuffer Clear()
        {
            return (this.Clear(DEFAULT_CAPACITY));
        }
        
        /// <summary>Removes all the characters from the current StringBuffer instance, and set the new capacity.</summary>
        public StringBuffer Clear(int newCapacity)
        {
            this.CheckEnumeration();
            this.InternalClear(newCapacity);
            return (this);
        }

        #endregion


        #region "APPEND | APPENDLINE | APPENDFORMAT | APPENDLINEFORMAT"

        /// <summary>Appends a specified character to this instance.</summary>
        public StringBuffer Append(char value)
        {            
            this.CheckEnumeration();
            this._failed = false;
            this.InternalAppend(value);
            return (this);
        }

        /// <summary>Appends a specified number of copies of a character to this instance.</summary>
        public StringBuffer Append(int repeatCount, char value)
        {// forward
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException("repeatCount");
            }
            else if (repeatCount == 0)
            {
                return (this);
            }
            else if (repeatCount == 1)
            {                
                return (this.Append(value));
            }
            else
            {
                char[] chars = new char[repeatCount];

                for (int i = 0; i < repeatCount; ++i)
                {
                    chars[i] = value;
                }

                return (this.Append(chars, 0, chars.Length));
            }
        }


        /// <summary>Appends a specified array of characters to this instance.</summary>
        public StringBuffer Append(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Append(value, 0, value.Length));
        }

        /// <summary>Appends a specified array of characters to this instance.</summary>
        public StringBuffer Append(char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            return (this.Append(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Appends a specified array of characters to this instance.</summary>
        public StringBuffer Append(char[] value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
                        
            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (length == 0)
                return (this);

            this.InternalAppend(value, startIndex, length);
            return (this);
        }


        /// <summary>Appends a specified string to this instance.</summary>
        public StringBuffer Append(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Append(value, 0, value.Length));
        }
        
        /// <summary>Appends a specified number of copies of a string to this instance.</summary>
        public StringBuffer Append(int repeatCount, string value)
        {
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException("repeatCount");
            }
            else if (repeatCount == 0)
            {
                return (this);
            }
            else if (repeatCount == 1)
            {
                return (this.Append(value));
            }
            else
            {
                char[] chars = new char[repeatCount * value.Length];

                for (int i = 0, j = 0; i < chars.Length; ++i)
                {
                    chars[i] = value[j++];

                    if (j >= value.Length)
                        j = 0;
                }

                return (this.Append(chars, 0, chars.Length));                
            }
        }
        
        /// <summary>Appends a specified string to this instance.</summary>
        public StringBuffer Append(string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            return (this.Append(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Appends a specified string to this instance.</summary>
        public StringBuffer Append(string value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (length == 0)
                return (this);

            this.InternalAppend(value, startIndex, length);
            return (this);
        }


        /// <summary>Appends the default line terminator to this instance.</summary>
        public StringBuffer AppendLine()
        {            
            this.CheckEnumeration();
            this._failed = false;
            this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            return (this);
        }


        /// <summary>Appends a specified array of characters followed by the default line terminator to this instance.</summary>
        public StringBuffer AppendLine(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.AppendLine(value, 0, value.Length));            
        }

        /// <summary>Appends a specified array of characters followed by the default line terminator to this instance.</summary>
        public StringBuffer AppendLine(char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.AppendLine(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Appends a specified array of characters followed by the default line terminator to this instance.</summary>
        public StringBuffer AppendLine(char[] value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (length != 0)
                this.InternalAppend(value, startIndex, length);
            
            this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            return (this);
        }


        /// <summary>Appends a specified string followed by the default line terminator to this instance.</summary>
        public StringBuffer AppendLine(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.AppendLine(value, 0, value.Length));
        }

        /// <summary>Appends a specified string followed by the default line terminator to this instance.</summary>
        public StringBuffer AppendLine(string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            return (this.AppendLine(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Appends a specified string followed by the default line terminator to this instance.</summary>
        public StringBuffer AppendLine(string value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (length != 0)
                this.InternalAppend(value, startIndex, length);
            
            this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            return (this);
        }
        
        
        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public StringBuffer AppendFormat(string value, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            int index = this.Length;
            this.InternalAppend(value, 0, value.Length);
            this.InternalFormat(args, null, index, value.Length);
            
            return (this);
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public StringBuffer AppendFormat(string value, IFormatProvider provider, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            int index = this.Length;            
            this.InternalAppend(value, 0, value.Length);
            this.InternalFormat(args, provider, index, value.Length);

            return (this);
        }
                

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public StringBuffer AppendLineFormat(string value, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;            
            int index = this.Length;
            this.InternalAppend(value, 0, value.Length);
            this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            this.InternalFormat(args, null, index, value.Length);

            return (this);
        }

        /// <summary>Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public StringBuffer AppendLineFormat(string value, IFormatProvider provider, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;
            int index = this.Length;
            this.InternalAppend(value, 0, value.Length);
            this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            this.InternalFormat(args, provider, index, value.Length);

            return (this);
        }

        #endregion


        #region "PREPEND | PREPENDLINE | PREPENDFORMAT | PREPENDLINEFORMAT"

        /// <summary>Prepends a specified character to this instance.</summary>
        public StringBuffer Prepend(char value)
        {            
            this.CheckEnumeration();
            this._failed = false;

            if (this.Length == 0)
            {
                this.InternalAppend(value);
            }
            else
            {
                this.InternalExpand(0, 1);
                this.InternalAssign(0, value);
            }

            return (this);
        }

        /// <summary>Prepends a specified number of copies of a character to this instance.</summary>
        public StringBuffer Prepend(char value, int repeatCount)
        { // forward
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException("repeatCount");
            }
            else if (repeatCount == 0)
            {
                return (this);
            }
            else if (repeatCount == 1)
            {                
                return (this.Prepend(value));
            }
            else
            {                
                char[] chars = new char[repeatCount];

                for (int i = 0; i < repeatCount; ++i)
                {
                    chars[i] = value;
                }

                return (this.Prepend(chars, 0, chars.Length));
            }
        }


        /// <summary>Prepends a specified array of characters to this instance.</summary>
        public StringBuffer Prepend(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Prepend(value, 0, value.Length));
        }

        /// <summary>Prepends a specified array of characters to this instance.</summary>
        public StringBuffer Prepend(char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Prepend(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Prepends a specified array of characters to this instance.</summary>
        public StringBuffer Prepend(char[] value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (length == 0)
                return (this);

            if (this.Length == 0)
            {
                this.InternalAppend(value, startIndex, length);
            }
            else
            {
                this.InternalExpand(0, length);
                this.InternalAssign(0, value, startIndex, length);
            }

            return (this);
        }


        /// <summary>Prepends a specified string to this instance.</summary>
        public StringBuffer Prepend(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Prepend(value, 0, value.Length));
        }

        /// <summary>Prepends a specified string to this instance.</summary>
        public StringBuffer Prepend(string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            return (this.Prepend(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Prepends a specified string to this instance.</summary>
        public StringBuffer Prepend(string value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (length == 0)
                return (this);

            if (this.Length == 0)
            {
                this.InternalAppend(value, startIndex, length);
            }
            else
            {
                this.InternalExpand(0, length);
                this.InternalAssign(0, value, startIndex, length);
            }

            return (this);
        }


        /// <summary>Prepends the default line terminator to this instance.</summary>
        public StringBuffer PrependLine()
        {            
            this.CheckEnumeration();
            this._failed = false;

            if (this.Length == 0)
            {
                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(0, Environment.NewLine.Length);
                this.InternalAssign(0, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            return (this);
        }


        /// <summary>Prepends a specified array of characters followed by the default line terminator to this instance.</summary>
        public StringBuffer PrependLine(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.PrependLine(value, 0, value.Length));            
        }

        /// <summary>Prepends a specified array of characters followed by the default line terminator to this instance.</summary>
        public StringBuffer PrependLine(char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.PrependLine(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Prepends a specified array of characters followed by the default line terminator to this instance.</summary>
        public StringBuffer PrependLine(char[] value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;
                        
            if (this.Length == 0)
            {
                if (length != 0)
                    this.InternalAppend(value, startIndex, length);

                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(0, length + Environment.NewLine.Length);
                
                if (length != 0)
                    this.InternalAssign(0, value, startIndex, length);

                this.InternalAssign(length, Environment.NewLine, 0, Environment.NewLine.Length);
            }
            
            return (this);
        }


        /// <summary>Prepends a specified string followed by the default line terminator to this instance.</summary>
        public StringBuffer PrependLine(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.PrependLine(value, 0, value.Length));
        }

        /// <summary>Prepends a specified string followed by the default line terminator to this instance.</summary>
        public StringBuffer PrependLine(string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.PrependLine(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Prepends a specified string followed by the default line terminator to this instance.</summary>
        public StringBuffer PrependLine(string value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");
            
            this.CheckEnumeration();
            this._failed = false;
  
            if (this.Length == 0)
            {
                if (length != 0)
                    this.InternalAppend(value, startIndex, length);

                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(0, length + Environment.NewLine.Length);

                if (length != 0)
                    this.InternalAssign(0, value, startIndex, length);

                this.InternalAssign(length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            return (this);
        }
        

        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public StringBuffer PrependFormat(string value, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;
            
            if (this.Length == 0)
            {
                this.InternalAppend(value, 0, value.Length);
            }
            else
            {
                this.InternalExpand(0, value.Length);
                this.InternalAssign(0, value, 0, value.Length);
            }

            this.InternalFormat(args, null, 0, value.Length);

            return (this);
        }

        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public StringBuffer PrependFormat(string value, IFormatProvider provider, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;            
            
            if (this.Length == 0)
            {
                this.InternalAppend(value, 0, value.Length);
            }
            else
            {
                this.InternalExpand(0, value.Length);
                this.InternalAssign(0, value, 0, value.Length);
            }

            this.InternalFormat(args, provider, 0, value.Length);

            return (this);
        }


        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public StringBuffer PrependLineFormat(string value, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;
                        
            if (this.Length == 0)
            {
                this.InternalAppend(value, 0, value.Length);
                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(0, value.Length + Environment.NewLine.Length);
                this.InternalAssign(0, value, 0, value.Length);
                this.InternalAssign(value.Length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            this.InternalFormat(args, null, 0, value.Length);

            return (this);
        }

        /// <summary>Prepends the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public StringBuffer PrependLineFormat(string value, IFormatProvider provider, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;

            if (this.Length == 0)
            {
                this.InternalAppend(value, 0, value.Length);
                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(0, value.Length + Environment.NewLine.Length);
                this.InternalAssign(0, value, 0, value.Length);
                this.InternalAssign(value.Length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            this.InternalFormat(args, provider, 0, value.Length);

            return (this);
        }

        #endregion


        #region "INSERT | INSERTLINE | INSERTFORMAT | INSERTLINEFORMAT"

        /// <summary>Inserts a specified character into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, char value)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (index == this.Length)
            {
                this.InternalAppend(value);
            }
            else
            {
                this.InternalExpand(index, 1);
                this.InternalAssign(index, value);
            }

            return (this);
        }

        /// <summary>Inserts a specified number of copies of a character into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, char value, int repeatCount)
        { // forward
            if (repeatCount < 0)
            {
                throw new ArgumentOutOfRangeException("repeatCount");
            }
            else if (repeatCount == 0)
            {
                return (this);
            }
            else if (repeatCount == 1)
            {                
                return (this.Insert(index, value));
            }
            else
            {
                char[] chars = new char[repeatCount];

                for (int i = 0; i < repeatCount; ++i)
                {
                    chars[i] = value;
                }

                return (this.Insert(index, chars, 0, chars.Length));
            }
        }


        /// <summary>Inserts a specified array of characters into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Insert(index, value, 0, value.Length));
        }

        /// <summary>Inserts a specified array of characters into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Insert(index, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Inserts a specified array of characters into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, char[] value, int startIndex, int length)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (value == null)
                throw new ArgumentNullException("value");
                        
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;

            if (length == 0)
                return (this);

            if (index == this.Length)
            {
                this.InternalAppend(value, startIndex, length);
            }
            else
            {                
                this.InternalExpand(index, length);
                this.InternalAssign(index, value, startIndex, length);
            }

            return (this);
        }


        /// <summary>Inserts a specified string into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Insert(index, value, 0, value.Length));
        }

        /// <summary>Inserts a specified string into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.Insert(index, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Inserts a specified string into this instance at the specified character position.</summary>
        public StringBuffer Insert(int index, string value, int startIndex, int length)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;

            if (length == 0)
                return (this);
            
            if (index == this.Length)
            {
                this.InternalAppend(value, startIndex, length);
            }
            else
            {                
                this.InternalExpand(index, length);
                this.InternalAssign(index, value, startIndex, length);
            }

            return (this);
        }

        
        /// <summary>Inserts the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");
                        
            this.CheckEnumeration();
            this._failed = false;

            if (index == this.Length)
            {
                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(index, Environment.NewLine.Length);
                this.InternalAssign(index, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            return (this);
        }


        /// <summary>Inserts a specified array of characters followed by the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index, char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.InsertLine(index, value, 0, value.Length));
        }

        /// <summary>Inserts a specified array of characters followed by the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index, char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.InsertLine(index, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Inserts a specified array of characters followed by the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index, char[] value, int startIndex, int length)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;
            
            if (index == this.Length)
            {
                if (length != 0)
                    this.InternalAppend(value, startIndex, length);

                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);                
            }
            else
            {
                this.InternalExpand(index, length + Environment.NewLine.Length);

                if (length != 0)
                    this.InternalAssign(index, value, startIndex, length);

                this.InternalAssign(index + length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            return (this);
        }


        /// <summary>Inserts a specified string followed by the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.InsertLine(index, value, 0, value.Length));
        }

        /// <summary>Inserts a specified string followed by the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index, string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.InsertLine(index, value, startIndex, value.Length - startIndex));
        }

        /// <summary>Inserts a specified string followed by the default line terminator into this instance at the specified character position.</summary>
        public StringBuffer InsertLine(int index, string value, int startIndex, int length)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;
                        
            if (index == this.Length)
            {
                if (length != 0)
                    this.InternalAppend(value, startIndex, length);

                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {                
                this.InternalExpand(index, length + Environment.NewLine.Length);

                if (length != 0)
                    this.InternalAssign(index, value, startIndex, length);

                this.InternalAssign(index + length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            return (this);
        }


        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public StringBuffer InsertFormat(int index, string value, params object[] args)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;
                        
            if (index == this.Length)
            {
                this.InternalAppend(value, 0, value.Length);
            }
            else
            {
                this.InternalExpand(index, value.Length);
                this.InternalAssign(index, value, 0, value.Length);
            }

            this.InternalFormat(args, null, 0, value.Length);

            return (this);
        }

        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public StringBuffer InsertFormat(int index, string value, IFormatProvider provider, params object[] args)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;

            if (index == this.Length)
            {
                this.InternalAppend(value, 0, value.Length);
            }
            else
            {
                this.InternalExpand(index, value.Length);
                this.InternalAssign(index, value, 0, value.Length);
            }

            this.InternalFormat(args, provider, index, value.Length);

            return (this);
        }
                

        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array.</summary>
        public StringBuffer InsertLineFormat(int index, string value, params object[] args)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;
                        
            if (index == this.Length)
            {
                this.InternalAppend(value, 0, value.Length);
                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(index, value.Length + Environment.NewLine.Length);
                this.InternalAssign(index, value, 0, value.Length);
                this.InternalAssign(index + value.Length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            this.InternalFormat(args, null, index, value.Length);

            return (this);
        }

        /// <summary>Inserts the string returned by processing a composite format string, which contains zero or more format items, to this instance.
        /// Each format item is replaced by the string representation of a corresponding argument in a parameter array using a specified format provider.</summary>
        public StringBuffer InsertLineFormat(int index, string value, IFormatProvider provider, params object[] args)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (args == null)
                throw new ArgumentNullException("args");

            this.CheckEnumeration();
            this._failed = false;

            if (index == this.Length)
            {
                this.InternalAppend(value, 0, value.Length);
                this.InternalAppend(Environment.NewLine, 0, Environment.NewLine.Length);
            }
            else
            {
                this.InternalExpand(index, value.Length + Environment.NewLine.Length);
                this.InternalAssign(index, value, 0, value.Length);
                this.InternalAssign(index + value.Length, Environment.NewLine, 0, Environment.NewLine.Length);
            }

            this.InternalFormat(args, provider, index, value.Length);

            return (this);
        }
        
        #endregion
        
        
        #region "CROP | REMOVE | SUBSTITUTE"

        /// <summary>Retrieves a substring from specified character position to the end of this instance.</summary> 
        public StringBuffer Crop(int index)
        {
            return (this.Crop(index, this.Length - index));
        }

        /// <summary>Retrieves a substring from specified character position with the specified length from this instance.</summary> 
        public StringBuffer Crop(int index, int length)
        {
            if ((index < 0) || (index > this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            this.CheckEnumeration();
            this._failed = false;

            if ((index == 0) && (length == this.Length))
                return (this);

            this.InternalCrop(index, length);
            return (this);
        }


        /// <summary>Removes a range of characters from the specified character position to the end of this instance.</summary>    
        public StringBuffer Remove(int index)
        {
            return (this.Remove(index, this.Length - index));
        }

        /// <summary>Removes a range of characters from the specified character position with the specified length from this instance.</summary>   
        public StringBuffer Remove(int index, int length)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            this.CheckEnumeration();
            this._failed = false;

            this.InternalShrink(index, length);
            return (this);
        }


        /// <summary>Substitutes all occurrences of a specified character with another specified character in this instance.</summary>
        public StringBuffer Substitute(char oldValue, char newValue)
        {
            return (this.Substitute(oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified character with another specified character in this instance.</summary>
        public StringBuffer Substitute(char oldValue, char newValue, bool ignoreCase)
        {
            this.CheckEnumeration();
            this._failed = false;

            if (this.Length != 0)
                this.InternalFindMultipleAndReplace(oldValue, newValue, 0, this.Length, ignoreCase);

            return (this);
        }

        /// <summary>Substitutes all occurrences of a specified character with another specified string in this instance.</summary>
        public StringBuffer Substitute(char oldValue, string newValue)
        {
            return (this.Substitute(oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified character with another specified string in this instance.</summary>
        public StringBuffer Substitute(char oldValue, string newValue, bool ignoreCase)
        {
            if (newValue == null)
                throw new ArgumentNullException("newValue");

            this.CheckEnumeration();
            this._failed = false;

            if (this.Length != 0)
                this.InternalFindMultipleAndReplace(oldValue, newValue, 0, this.Length, ignoreCase);

            return (this);
        }
                

        /// <summary>Substitutes all occurrences of a specified string with another specified character in this instance.</summary>
        public StringBuffer Substitute(string oldValue, char newValue)
        {
            return (this.Substitute(oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified string with another specified character in this instance.</summary>
        public StringBuffer Substitute(string oldValue, char newValue, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException("oldValue");

            this.CheckEnumeration();
            this._failed = false;

            if (oldValue.Length > this.Length)
                return (this);

            this.InternalFindMultipleAndReplace(oldValue, newValue, 0, this.Length, ignoreCase);

            return (this);
        }

        /// <summary>Substitutes all occurrences of a specified string with another specified string in this instance.</summary>
        public StringBuffer Substitute(string oldValue, string newValue)
        {
            return (this.Substitute(oldValue, newValue, false));
        }

        /// <summary>Substitutes all occurrences of a specified string with another specified string in this instance.</summary>
        public StringBuffer Substitute(string oldValue, string newValue, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException("oldValue");

            if (newValue == null)
                throw new ArgumentNullException("newValue");

            this.CheckEnumeration();
            this._failed = false;

            if (oldValue.Length > this.Length)
                return (this);

            this.InternalFindMultipleAndReplace(oldValue, newValue, 0, this.Length, ignoreCase);

            return (this);
        }
                
        #endregion
                
        
        #region "REPLACE | REPLACERANGE | REPLACEBEFORE | REPLACEAFTER | REPLACEINSIDE"

        /// <summary>Replaces the first or the last specified character by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, char newValue)
        {
            return (this.Replace(oldValue, occurrence, newValue, 0, this.Length, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, char newValue, int index)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, char newValue, int index, int length)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, char newValue, int index, int length, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            this.CheckEnumeration();
            
            if (length == 0)
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurrence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(oldValue, index, length, ignoreCase);
            else
                pos = this.InternalLastIndexOf(oldValue, index, length, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, pos, 1);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces the first or the last specified character by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, string newValue)
        {
            return (this.Replace(oldValue, occurrence, newValue, 0, this.Length, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, string newValue, int index)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, string newValue, int index, int length)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified character by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(char oldValue, ESearch occurrence, string newValue, int index, int length, bool ignoreCase)
        {            
            if (newValue == null)
                throw new ArgumentNullException("newValue");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");
                        
            this.CheckEnumeration();
            
            if (length == 0)
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurrence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(oldValue, index, length, ignoreCase);
            else
                pos = this.InternalLastIndexOf(oldValue, index, length, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, pos, 1);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces the first or the last specified string by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, char newValue)
        {
            return (this.Replace(oldValue, occurrence, newValue, 0, this.Length, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, char newValue, int index)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, char newValue, int index, int length)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, length, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified character in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, char newValue, int index, int length, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException("oldValue");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            this.CheckEnumeration();            

            if ((length == 0) || (oldValue.Length > length))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurrence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(oldValue, index, length, ignoreCase);
            else
                pos = this.InternalLastIndexOf(oldValue, index, length, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, pos, oldValue.Length);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces the first or the last specified string by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, string newValue)
        {
            return (this.Replace(oldValue, occurrence, newValue, 0, this.Length, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, string newValue, int index)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, this.Length - index, false));
        }

        /// <summary>Replaces the first or the last specified string by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, string newValue, int index, int length)
        {
            return (this.Replace(oldValue, occurrence, newValue, index, length, false));
        }
        
        /// <summary>Replaces the first or the last specified string by another specified string in this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer Replace(string oldValue, ESearch occurrence, string newValue, int index, int length, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(oldValue))
                throw new ArgumentNullException("oldValue");

            if (newValue == null)
                throw new ArgumentNullException("newValue");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            this.CheckEnumeration();            

            if ((length == 0) || (oldValue.Length > length))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurrence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(oldValue, index, length, ignoreCase);
            else
                pos = this.InternalLastIndexOf(oldValue, index, length, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, pos, oldValue.Length);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, char value, ESearch occurence, char newValue)
        {
            return (this.ReplaceRange(index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, char value, ESearch occurence, char newValue, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, index, (pos + 1) - index);
            else
                this._failed = true;

            return (this);
        }


        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, char value, ESearch occurence, string newValue)
        {
            return (this.ReplaceRange(index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, char value, ESearch occurence, string newValue, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (newValue == null)
                throw new ArgumentNullException("newValue");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, index, (pos + 1) - index);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, string value, ESearch occurence, char newValue)
        {
            return (this.ReplaceRange(index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, string value, ESearch occurence, char newValue, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, index, (pos + value.Length) - index);
            else
                this._failed = true;

            return (this);
        }


        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, string value, ESearch occurence, string newValue)
        {
            return (this.ReplaceRange(index, value, occurence, newValue, false));
        }

        /// <summary>Replaces a range of characters from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer ReplaceRange(int index, string value, ESearch occurence, string newValue, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (newValue == null)
                throw new ArgumentNullException("newValue");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);
                        
            if (pos != -1)
                this.InternalReplace(newValue, index, (pos + value.Length) - index);
            else
                this._failed = true;

            return (this);
        }
        
        
        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(char value, ESearch occurence, char newValue)
        {
            return (this.ReplaceBefore(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(char value, ESearch occurence, char newValue, int index)
        {
            return (this.ReplaceBefore(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(char value, ESearch occurence, char newValue, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos > 0)
                this.InternalReplace(newValue, 0, pos);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(char value, ESearch occurence, string newValue)
        {
            return (this.ReplaceBefore(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(char value, ESearch occurence, string newValue, int index)
        {
            return (this.ReplaceBefore(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(char value, ESearch occurence, string newValue, int index, bool ignoreCase)
        {
            if (newValue == null)
                throw new ArgumentNullException();

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos > 0)
                this.InternalReplace(newValue, 0, pos);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(string value, ESearch occurence, char newValue)
        {
            return (this.ReplaceBefore(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(string value, ESearch occurence, char newValue, int index)
        {
            return (this.ReplaceBefore(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(string value, ESearch occurence, char newValue, int index, bool ignoreCase)
        {            
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos > 0)
                this.InternalReplace(newValue, 0, pos);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(string value, ESearch occurence, string newValue)
        {
            return (this.ReplaceBefore(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(string value, ESearch occurence, string newValue, int index)
        {
            return (this.ReplaceBefore(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceBefore(string value, ESearch occurence, string newValue, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (newValue == null)
                throw new ArgumentNullException();

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();            

            if (value.Length > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos > 0)
                this.InternalReplace(newValue, 0, pos);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(char value, ESearch occurence, char newValue)
        {
            return (this.ReplaceAfter(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(char value, ESearch occurence, char newValue, int index)
        {
            return (this.ReplaceAfter(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(char value, ESearch occurence, char newValue, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();
            
            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if ((pos != -1) && ((pos + 1) != this.Length))
                this.InternalReplace(newValue, pos + 1, this.Length - (pos + 1));
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(char value, ESearch occurence, string newValue)
        {
            return (this.ReplaceAfter(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(char value, ESearch occurence, string newValue, int index)
        {
            return (this.ReplaceAfter(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(char value, ESearch occurence, string newValue, int index, bool ignoreCase)
        {
            if (newValue == null)
                throw new ArgumentNullException();

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;           
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);
                        
            if ((pos != -1) && ((pos + 1) != this.Length))
                this.InternalReplace(newValue, pos + 1, this.Length - (pos + 1));
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(string value, ESearch occurence, char newValue)
        {
            return (this.ReplaceAfter(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(string value, ESearch occurence, char newValue, int index)
        {
            return (this.ReplaceAfter(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(string value, ESearch occurence, char newValue, int index, bool ignoreCase)
        {            
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if (value.Length > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if ((pos != -1) && ((pos + value.Length) != this.Length))
                this.InternalReplace(newValue, pos + value.Length, this.Length - (pos + value.Length));
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(string value, ESearch occurence, string newValue)
        {
            return (this.ReplaceAfter(value, occurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(string value, ESearch occurence, string newValue, int index)
        {
            return (this.ReplaceAfter(value, occurence, newValue, index, false));
        }

        /// <summary>Replaces a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceAfter(string value, ESearch occurence, string newValue, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (newValue == null)
                throw new ArgumentNullException();

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if (value.Length > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if ((pos != -1) && ((pos + value.Length) != this.Length))
                this.InternalReplace(newValue, pos + value.Length, this.Length - (pos + value.Length));
            else
                this._failed = true;

            return (this);
        }
                

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, char newValue)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, char newValue, int index)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, char newValue, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();
            
            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + 1) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);

                if ((pos2 != -1) && ((pos1 + 1) <= pos2))
                    this.InternalReplace(newValue, pos1 + 1, pos2 - (pos1 + 1));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }
        

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, string newValue)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, string newValue, int index)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, string newValue, int index, bool ignoreCase)
        {
            if (newValue == null)
                throw new ArgumentNullException("newValue");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + 1) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);

                if ((pos2 != -1) && ((pos1 + 1) <= pos2))
                    this.InternalReplace(newValue, pos1 + 1, pos2 - (pos1 + 1));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }
        
        
        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified strings.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, char newValue)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, char newValue, int index)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, char newValue, int index, bool ignoreCase)
        {            
            if (string.IsNullOrEmpty(prev))
                throw new ArgumentNullException("prev");

            if (string.IsNullOrEmpty(next))
                throw new ArgumentNullException("next");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if ((prev.Length + next.Length) > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + prev.Length) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);

                if ((pos2 != -1) && ((pos1 + prev.Length) <= pos2))
                    this.InternalReplace(newValue, pos1 + prev.Length, pos2 - (pos1 + prev.Length));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }
        

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified strings.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, string newValue)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, 0, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, string newValue, int index)
        {
            return (this.ReplaceInside(prev, prevOccurence, next, nextOccurence, newValue, index, false));
        }

        /// <summary>Replaces a substring in this instance. Between the first and the second occurence of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer ReplaceInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, string newValue, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(prev))
                throw new ArgumentNullException("prev");

            if (string.IsNullOrEmpty(next))
                throw new ArgumentNullException("next");

            if (newValue == null)
                throw new ArgumentNullException("newValue");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if ((prev.Length + next.Length) > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + prev.Length) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);

                if ((pos2 != -1) && ((pos1 + prev.Length) <= pos2))
                    this.InternalReplace(newValue, pos1 + prev.Length, pos2 - (pos1 + prev.Length));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }
               
        #endregion

        
        #region "SUBSTRINGRANGE | SUBSTRINGBEFORE | SUBSTRINGAFTER | SUBSTRINGINSIDE | SUBSTRINGOUTSIDE"
                
        /// <summary>Retrieves a substring from this instance, from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringRange(int index, char value, ESearch occurence)
        {
            return (this.SubstringRange(index, value, occurence, false));
        }

        /// <summary>Retrieves a substring from this instance, from the specified index position to the first or last specified matching character position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer SubstringRange(int index, char value, ESearch occurence, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos != -1)
                this.InternalCrop(index, (pos + 1) - index);
            else
                this._failed = true;

            return (this);
        }


        /// <summary>Retrieves a substring from this instance, from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer SubstringRange(int index, string value, ESearch occurence)
        {
            return (this.SubstringRange(index, value, occurence, false));
        }

        /// <summary>Retrieves a substring from this instance, from the specified index position to the first or last specified matching string position (included) from this instance.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>         
        public StringBuffer SubstringRange(int index, string value, ESearch occurence, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");
            
            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos != -1)
                this.InternalCrop(index, (pos + value.Length) - index);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringBefore(char value, ESearch occurence)
        {
            return (this.SubstringBefore(value, occurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringBefore(char value, ESearch occurence, int index)
        {
            return (this.SubstringBefore(value, occurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringBefore(char value, ESearch occurence, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos == 0)
                this.InternalClear();
            else if (pos != -1)
                this.InternalCrop(0, pos);
            else
                this._failed = true;

            return (this);
        }


        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringBefore(string value, ESearch occurence)
        {
            return (this.SubstringBefore(value, occurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringBefore(string value, ESearch occurence, int index)
        {
            return (this.SubstringBefore(value, occurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringBefore(string value, ESearch occurence, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if (value.Length > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if (pos == 0)
                this.InternalClear();
            else if (pos != -1)
                this.InternalCrop(0, pos);
            else
                this._failed = true;

            return (this);
        }
        

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified character.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringAfter(char value, ESearch occurence)
        {
            return (this.SubstringAfter(value, occurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringAfter(char value, ESearch occurence, int index)
        {
            return (this.SubstringAfter(value, occurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified character. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringAfter(char value, ESearch occurence, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if ((pos + 1) == this.Length)
                this.InternalClear();
            else if (pos != -1)
                this.InternalCrop(pos + 1, this.Length - (pos + 1));
            else
                this._failed = true;

            return (this);
        }


        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified string.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringAfter(string value, ESearch occurence)
        {
            return (this.SubstringAfter(value, occurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringAfter(string value, ESearch occurence, int index)
        {
            return (this.SubstringAfter(value, occurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. After the first or the last occurence of the specified string. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringAfter(string value, ESearch occurence, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if (value.Length > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos;

            if (occurence == ESearch.FirstOccurrence)
                pos = this.InternalFirstIndexOf(value, index, this.Length - index, ignoreCase);
            else
                pos = this.InternalLastIndexOf(value, index, this.Length - index, ignoreCase);

            if ((pos + value.Length) == this.Length)
                this.InternalClear();
            else if (pos != -1)
                this.InternalCrop(pos + value.Length, this.Length - (pos + value.Length));
            else
                this._failed = true;

            return (this);
        }

        
        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified characters.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence)
        {
            return (this.SubstringInside(prev, prevOccurence, next, nextOccurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, int index)
        {
            return (this.SubstringInside(prev, prevOccurence, next, nextOccurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringInside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + 1) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);

                if ((pos2 != -1) && ((pos1 + 1) <= pos2))
                    this.InternalCrop(pos1 + 1, pos2 - (pos1 + 1));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }


        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified strings.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence)
        {
            return (this.SubstringInside(prev, prevOccurence, next, nextOccurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, int index)
        {
            return (this.SubstringInside(prev, prevOccurence, next, nextOccurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Between the first and the second occurences of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringInside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(prev))
                throw new ArgumentNullException("prev");

            if (string.IsNullOrEmpty(next))
                throw new ArgumentNullException("next");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if ((prev.Length + next.Length) > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + prev.Length) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);

                if ((pos2 != -1) && ((pos1 + prev.Length) <= pos2))
                    this.InternalCrop(pos1 + prev.Length, pos2 - (pos1 + prev.Length));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }
        

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified characters.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringOutside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence)
        {
            return (this.SubstringOutside(prev, prevOccurence, next, nextOccurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringOutside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, int index)
        {
            return (this.SubstringOutside(prev, prevOccurence, next, nextOccurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified characters. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringOutside(char prev, ESearch prevOccurence, char next, ESearch nextOccurence, int index, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + 1) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + 1, this.Length - (pos1 + 1), ignoreCase);

                if ((pos2 != -1) && ((pos1 + 1) <= pos2))
                    this.InternalShrink(pos1 + 1, pos2 - (pos1 + 1));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }


        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified strings.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringOutside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence)
        {
            return (this.SubstringOutside(prev, prevOccurence, next, nextOccurence, 0, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringOutside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, int index)
        {
            return (this.SubstringOutside(prev, prevOccurence, next, nextOccurence, index, false));
        }

        /// <summary>Retrieves a substring from this instance. Before the first and after the second occurences of the specified strings. Perform searching from the specified index.
        /// <para/>This operation may fail, check it with the Fail/Succeed methods.</summary>
        public StringBuffer SubstringOutside(string prev, ESearch prevOccurence, string next, ESearch nextOccurence, int index, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(prev))
                throw new ArgumentNullException("prev");

            if (string.IsNullOrEmpty(next))
                throw new ArgumentNullException("next");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            this.CheckEnumeration();

            if ((prev.Length + next.Length) > (this.Length - index))
            {
                this._failed = true;
                return (this);
            }

            this._failed = false;
            int pos1;
            int pos2;

            if (prevOccurence == ESearch.FirstOccurrence)
                pos1 = this.InternalFirstIndexOf(prev, index, this.Length - index, ignoreCase);
            else
                pos1 = this.InternalLastIndexOf(prev, index, this.Length - index, ignoreCase);

            if ((pos1 != -1) && ((pos1 + prev.Length) < this.Length))
            {
                if (nextOccurence == ESearch.FirstOccurrence)
                    pos2 = this.InternalFirstIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);
                else
                    pos2 = this.InternalLastIndexOf(next, pos1 + prev.Length, this.Length - (pos1 + prev.Length), ignoreCase);

                if ((pos2 != -1) && ((pos1 + prev.Length) <= pos2))
                    this.InternalShrink(pos1 + prev.Length, pos2 - (pos1 + prev.Length));
                else
                    this._failed = true;
            }
            else
            {
                this._failed = true;
            }

            return (this);
        }
        
        #endregion
                        

        #region "TRIM | TRIMSTART | TRIMEND"

        /// <summary>Removes all leading and trailing white-space characters from this instance.</summary>    
        public StringBuffer Trim()
        {
            return (this.Trim(StringBuffer.Whitespace, false));
        }
        
        /// <summary>Removes all leading and trailing occurrences of a specified character from this instance.</summary>
        public StringBuffer Trim(char value, bool ignoreCase)
        {
            return (this.Trim(new char[1] { value }, ignoreCase));
        }
        

        /// <summary>Removes all leading and trailing occurrences of a set of characters specified in an array from this instance.</summary>
        public StringBuffer Trim(params char[] value)
        {            
            return (this.Trim(value, false));
        }

        /// <summary>Removes all leading and trailing occurrences of a set of characters specified in an array from this instance.</summary>
        public StringBuffer Trim(char[] value, bool ignoreCase)
        {
            this.CheckEnumeration();
            this._failed = false;

            if (value == null)
                value = StringBuffer.Whitespace;

            if (value.Length == 0)
                value = StringBuffer.Whitespace;

            this.InternalTrim(value, ignoreCase, true, true);

            return (this);
        }


        /// <summary>Removes all leading white-space characters from this instance.</summary>
        public StringBuffer TrimStart()
        {
            return (this.TrimStart(StringBuffer.Whitespace, false));
        }
                
        /// <summary>Removes all leading occurrences of a specified character from this instance.</summary>
        public StringBuffer TrimStart(char value, bool ignoreCase)
        {
            return (this.TrimStart(new char[1] { value }, ignoreCase));
        }


        /// <summary>Removes all leading occurrences of a set of characters specified in an array from this instance.</summary>
        public StringBuffer TrimStart(params char[] value)
        {
            return (this.TrimStart(value, false));
        }

        /// <summary>Removes all leading occurrences of a set of characters specified in an array from this instance.</summary>    
        public StringBuffer TrimStart(char[] value, bool ignoreCase)
        {
            this.CheckEnumeration();
            this._failed = false;

            if (value == null)
                value = StringBuffer.Whitespace;

            if (value.Length == 0)
                value = StringBuffer.Whitespace;

            this.InternalTrim(value, ignoreCase, true, false);

            return (this);
        }


        /// <summary>Removes all trailing white-space characters from this instance.</summary>
        public StringBuffer TrimEnd()
        {
            return (this.TrimEnd(StringBuffer.Whitespace, false));
        }

        /// <summary>Removes all trailing occurrences of a specified character from this instance.</summary>
        public StringBuffer TrimEnd(char value, bool ignoreCase)
        {
            return (this.TrimEnd(new char[1] { value }, ignoreCase));
        }


        /// <summary>Removes all trailing occurrences of a set of characters specified in an array from this instance.</summary>
        public StringBuffer TrimEnd(params char[] value)
        {
            return (this.TrimEnd(value, false));
        }

        /// <summary>Removes all trailing occurrences of a set of characters specified in an array from this instance.</summary>    
        public StringBuffer TrimEnd(char[] value, bool ignoreCase)
        {
            this.CheckEnumeration();
            this._failed = false;

            if (value == null)
                value = StringBuffer.Whitespace;

            if (value.Length == 0)
                value = StringBuffer.Whitespace;

            this.InternalTrim(value, ignoreCase, false, true);

            return (this);
        }

        #endregion


        #region "PADLEFT | PADRIGHT"

        /// <summary>Right-aligns the characters in this instance by padding them with spaces on the left, for a specified total length.</summary>        
        public StringBuffer PadLeft(int totalWidth)
        {
            return (this.PadLeft(totalWidth, ' '));
        }

        /// <summary>Right-aligns the characters in this instance by padding them with spaces on the left, for a specified total length.</summary> 
        public StringBuffer PadLeft(int totalWidth, char paddingChar)
        {
            if (totalWidth < 0)
                throw new ArgumentOutOfRangeException("totalWidth");

            this.CheckEnumeration();
            this._failed = false;
            
            if (totalWidth > this.Length)
            {
                int cnt = totalWidth - this.Length;

                char[] chars = new char[cnt];

                for (int i = 0; i < cnt; ++i)
                {
                    chars[i] = paddingChar;
                }

                if (this.Length == 0)
                {
                    this.InternalAppend(chars, 0, cnt);
                }
                else
                {
                    this.InternalExpand(0, cnt);
                    this.InternalAssign(0, chars, 0, cnt);
                }
            }
            
            return (this);
        }


        /// <summary>Left-aligns the characters in this string by padding them with spaces on the right, for a specified total length.</summary> 
        public StringBuffer PadRight(int totalWidth)
        {
            return (this.PadRight(totalWidth, ' '));
        }

        /// <summary>Left-aligns the characters in this string by padding them with spaces on the right, for a specified total length.</summary> 
        public StringBuffer PadRight(int totalWidth, char paddingChar)
        {
            if (totalWidth < 0)
                throw new ArgumentOutOfRangeException("totalWidth");

            this.CheckEnumeration();
            this._failed = false;
            
            if (totalWidth > this.Length)
            {
                int cnt = totalWidth - this.Length;
                char[] chars = new char[cnt];

                for (int i = 0; i < cnt; ++i)
                {
                    chars[i] = paddingChar;
                }

                this.InternalAppend(chars, cnt);
            }
           
            return (this);
        }

        #endregion


        #region "TOLOWER | TOLOWERINVARIANT | TOUPPER | TOUPPERINVARIANT"

        /// <summary>LowerCase the string in this instance.</summary>
        public StringBuffer ToLower()
        {            
            return (this.ToLower(CultureInfo.CurrentCulture));
        }

        /// <summary>LowerCase the string in this instance.</summary>
        public StringBuffer ToLowerInvariant()
        {
            return (this.ToLower(CultureInfo.InvariantCulture));
        }

        /// <summary>LowerCase the string in this instance.</summary>
        public StringBuffer ToLower(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException("culture");

            this.CheckEnumeration();
            this._failed = false;
            this.ToLowerCase(culture);
            return (this);
        }


        /// <summary>UpperCase the string in this instance.</summary>
        public StringBuffer ToUpper()
        {
            return (this.ToUpper(CultureInfo.CurrentCulture));
        }
        
        /// <summary>UpperCase the string in this instance.</summary>
        public StringBuffer ToUpperInvariant()
        {           
            return (this.ToUpper(CultureInfo.InvariantCulture));
        }

        /// <summary>UpperCase the string in this instance.</summary>
        public StringBuffer ToUpper(CultureInfo culture)
        {
            this.CheckEnumeration();
            this._failed = false;
            this.ToUpperCase(culture);
            return (this);
        }

        #endregion
        

        #region "FROMCHAR | FROMCHARARRAY | FROMSTRING"

        /// <summary>Clears this instance and append a character.</summary>
        public StringBuffer FromChar(char value)
        {
            this.CheckEnumeration();
            this._failed = false;
                        
            if (this.Length != 0)
                this.InternalClear(DEFAULT_CAPACITY);

            this.InternalAppend(value);
            return (this);
        }


        /// <summary>Clears this instance and append the character's array.</summary>
        public StringBuffer FromCharArray(char[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.FromCharArray(value, 0, value.Length));
        }

        /// <summary>Clears this instance and append the character's array.</summary>
        public StringBuffer FromCharArray(char[] value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.FromCharArray(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Clears this instance and append the character's array.</summary>
        public StringBuffer FromCharArray(char[] value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;

            if (this.Length != 0)
                this.InternalClear(length);

            if (length == 0)
                return (this);

            this.InternalAppend(value, startIndex, length);

            return (this);
        }


        /// <summary>Clears this instance and append the string.</summary>
        public StringBuffer FromString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.FromString(value, 0, value.Length));
        }

        /// <summary>Clears this instance and append the string.</summary>
        public StringBuffer FromString(string value, int startIndex)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return (this.FromString(value, startIndex, value.Length - startIndex));
        }

        /// <summary>Clears this instance and append the string.</summary>
        public StringBuffer FromString(string value, int startIndex, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");

            if ((startIndex < 0) || ((startIndex + length) > value.Length))
                throw new ArgumentOutOfRangeException("startIndex");

            this.CheckEnumeration();
            this._failed = false;
            
            if (this.Length != 0)
                this.InternalClear(length);

            if (length == 0)
                return (this);

            this.InternalAppend(value, startIndex, length);
            return (this);
        }
        
        #endregion


        #region "FORMATWITH"

        /// <summary>Format with the specified objects, there is two type of format:
        /// <para/>based on index position equivalent to the string format or based on name.        
        /// </summary>
        public StringBuffer FormatWith(IFormatProvider provider, params object[] args)
        {
            return (this.FormatWith(0, this.Length, provider, args));
        }

        /// <summary>Format with the specified objects, there is two type of format:
        /// <para/>based on index position equivalent to the string format or based on name.        
        /// </summary>
        public StringBuffer FormatWith(int index, int length, IFormatProvider provider, params object[] args)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (args == null)
                throw new ArgumentNullException("args");
            
            this.CheckEnumeration();
            this._failed = false;

            if (this.Length == 0)
                return (this);

            this.InternalFormat(args, provider, index, this.Length);

            return (this);
        }

        #endregion


        #region "FLUENT"

        /// <summary>Execute action if the previous 'Replace' or 'Substring' methods call failed.</summary>
        public StringBuffer Fail(Action<StringBuffer> action)
        {
            if (this._failed && (action != null))
            {                
                action(this);
                this._failed = true;
            }

            return (this);
        }

        /// <summary>Execute action if the previous 'Replace' or 'Substring' methods call succeeded.</summary>
        public StringBuffer Succeed(Action<StringBuffer> action)
        {
            if ((!this._failed) && (action != null))
            {
                action(this);
                this._failed = false;
            }
            
            return (this);
        }

        /// <summary>Set the 'failed' field to false, 'failed' is only used in Fail() / Succeed() methods.</summary>
        public StringBuffer Unfail()
        {
            this._failed = false;
            return (this);
        }
        

        /// <summary>.</summary>        
        public StringBuffer Do(Action<StringBuffer> action)
        {
            if (action != null)
            {
                action(this);
            }

            return (this);
        }
                
        /// <summary>.</summary>        
        public StringBuffer Do<T>(T on, Action<StringBuffer, T> action)
        {
            if (action != null)
            {
                action(this, on);
            }

            return (this);
        }
                
        /// <summary>.</summary>        
        public StringBuffer While(Func<StringBuffer, bool> condition)
        {
            if (condition != null)
            {
                while (condition(this)) { };
            }

            return (this);
        }        
        
        /// <summary>.</summary>
        public StringBuffer DoWhen(Func<StringBuffer, bool> condition, Action<StringBuffer> action)
        {
            if (condition != null)
            {                
                if (condition(this))
                {
                    if (action != null)
                    {
                        action(this);
                    }
                }
            }

            return (this);
        }

        /// <summary>.</summary>
        public StringBuffer DoAtWhen(Func<StringBuffer, int> grabIndex, Func<StringBuffer, int, bool> condition, Action<StringBuffer, int> action)
        {
            if (grabIndex != null)
            {
                int index = grabIndex(this);

                if ((condition != null) && (condition(this, index)))
                {
                    if (action != null)
                    {
                        action(this, index);
                    }
                }               
            }

            return (this);
        }

        /// <summary>.</summary>
        public StringBuffer DoAtWhenElse(Func<StringBuffer, int> grabIndex, Func<StringBuffer, int, bool> condition, Action<StringBuffer, int> trueAction, Action<StringBuffer, int> falseAction)
        {
            if (grabIndex != null)
            {
                int index = grabIndex(this);

                if (condition != null)                   
                {
                    if (condition(this, index))
                    {
                        if (trueAction != null)
                        {
                            trueAction(this, index);
                        }
                    }
                    else
                    {
                        if (falseAction != null)
                        {
                            falseAction(this, index);
                        }
                    }                    
                }
            }

            return (this);
        }
        
        /// <summary>.</summary>
        public StringBuffer DoWhenElse(Func<StringBuffer, bool> condition, Action<StringBuffer> trueAction, Action<StringBuffer> falseAction)
        {
            if (condition != null)
            {
                if (condition(this))
                {
                    if (trueAction != null)
                    {
                        trueAction(this);
                    }
                }
                else
                {
                    if (falseAction != null)
                    {
                        falseAction(this);
                    }
                }
            }

            return (this);
        }
                
        /// <summary>.</summary>
        public StringBuffer DoWhile(Func<StringBuffer, bool> condition, Action<StringBuffer> action)
        {            
            if (condition != null)
            {               
                while (condition(this))
                {
                    if (action != null)
                    {
                        action(this);
                    }
                }                
            }

            return (this);
        }
        
        #endregion


        #region "INDEXOF | INDEXOFANY"

        /// <summary>Reports the index of the first occurrence of a specified character within this instance.</summary>    
        public int IndexOf(char value)
        {
            return (this.IndexOf(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified character within this instance.</summary>   
        public int IndexOf(char value, int index)
        {
            return (this.IndexOf(value, index, this.Length - index, false));
        }
        
        /// <summary>Reports the index of the first occurrence of a specified character within this instance.</summary>
        public int IndexOf(char value, int index, int length)
        {
            return (this.IndexOf(value, index, length, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified character within this instance.</summary>  
        public int IndexOf(char value, int index, int length, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);
                        
            return (this.InternalFirstIndexOf(value, index, length, ignoreCase));
        }


        /// <summary>Reports the index of the first occurrence of a specified string within this instance.</summary>  
        public int IndexOf(string value)
        {
            return (this.IndexOf(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified string within this instance.</summary>   
        public int IndexOf(string value, int index)
        {
            return (this.IndexOf(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified string within this instance.</summary>   
        public int IndexOf(string value, int index, int length)
        {
            return (this.IndexOf(value, index, length, false));
        }
        
        /// <summary>Reports the index of the first occurrence of a specified  string within this instance.</summary>  
        public int IndexOf(string value, int index, int length, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if ((length == 0) || (value.Length > length))
                return (-1);

            return (this.InternalFirstIndexOf(value, index, length, ignoreCase));            
        }


        /// <summary>Reports the index of the first occurrence of a specified string within this instance.</summary>  
        public int IndexOf(StringBuffer value)
        {
            return (this.IndexOf(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified string within this instance.</summary>   
        public int IndexOf(StringBuffer value, int index)
        {
            return (this.IndexOf(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified string within this instance.</summary>   
        public int IndexOf(StringBuffer value, int index, int length)
        {
            return (this.IndexOf(value, index, length, false));
        }

        /// <summary>Reports the index of the first occurrence of a specified  string within this instance.</summary>  
        public int IndexOf(StringBuffer value, int index, int length, bool ignoreCase)
        {
            if (StringBuffer.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if ((length == 0) || (value.Length > length))
                return (-1);

            return (this.InternalFirstIndexOf(value, index, length, ignoreCase));
        }


        /// <summary>Reports the index of the first occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int IndexOfAny(char[] value)
        {
            return (this.IndexOfAny(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int IndexOfAny(char[] value, int index)
        {
            return (this.IndexOfAny(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int IndexOfAny(char[] value, int index, int length)
        {
            return (this.IndexOfAny(value, index, length, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int IndexOfAny(char[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);
            
            for (int i = 0; i < value.Length; ++i)
            {                
                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                
                if (pos != -1)
                    return (pos);
            }

            return (-1);
        }


        /// <summary>Reports the index of the first occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int IndexOfAny(string[] value)
        {
            return (this.IndexOfAny(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int IndexOfAny(string[] value, int index)
        {
            return (this.IndexOfAny(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int IndexOfAny(string[] value, int index, int length)
        {
            return (this.IndexOfAny(value, index, length, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int IndexOfAny(string[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);
                        
            for (int i = 0; i < value.Length; ++i)
            {
                if (string.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                
                if (pos != -1)
                    return (pos);
            }

            return (-1);
        }


        /// <summary>Reports the index of the first occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int IndexOfAny(StringBuffer[] value)
        {
            return (this.IndexOfAny(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int IndexOfAny(StringBuffer[] value, int index)
        {
            return (this.IndexOfAny(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int IndexOfAny(StringBuffer[] value, int index, int length)
        {
            return (this.IndexOfAny(value, index, length, false));
        }

        /// <summary>Reports the index of the first occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int IndexOfAny(StringBuffer[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);

            for (int i = 0; i < value.Length; ++i)
            {
                if (StringBuffer.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);

                if (pos != -1)
                    return (pos);
            }

            return (-1);
        }
        
        #endregion


        #region "LASTINDEXOF | LASTINDEXOFANY"

        /// <summary>Reports the index of the last occurrence of a specified character within this instance.</summary>    
        public int LastIndexOf(char value)
        {
            return (this.LastIndexOf(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified character within this instance.</summary>   
        public int LastIndexOf(char value, int index)
        {
            return (this.LastIndexOf(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified character within this instance.</summary>    
        public int LastIndexOf(char value, int index, int length)
        {
            return (this.LastIndexOf(value, index, length, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified character within this instance.</summary>   
        public int LastIndexOf(char value, int index, int length, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);

            return (this.InternalLastIndexOf(value, index, length, ignoreCase));
        }


        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary>    
        public int LastIndexOf(string value)
        {
            return (this.LastIndexOf(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary> 
        public int LastIndexOf(string value, int index)
        {
            return (this.LastIndexOf(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary>    
        public int LastIndexOf(string value, int index, int length)
        {
            return (this.LastIndexOf(value, index, length, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary>   
        public int LastIndexOf(string value, int index, int length, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if ((length == 0) || (value.Length > length))
                return (-1);

            return (this.InternalLastIndexOf(value, index, length, ignoreCase));
        }


        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary>    
        public int LastIndexOf(StringBuffer value)
        {
            return (this.LastIndexOf(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary> 
        public int LastIndexOf(StringBuffer value, int index)
        {
            return (this.LastIndexOf(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary>    
        public int LastIndexOf(StringBuffer value, int index, int length)
        {
            return (this.LastIndexOf(value, index, length, false));
        }

        /// <summary>Reports the index of the last occurrence of a specified string within this instance.</summary>   
        public int LastIndexOf(StringBuffer value, int index, int length, bool ignoreCase)
        {
            if (StringBuffer.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if ((length == 0) || (value.Length > length))
                return (-1);

            return (this.InternalLastIndexOf(value, index, length, ignoreCase));
        }


        /// <summary>Reports the index of the last occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int LastIndexOfAny(char[] value)
        {
            return (this.LastIndexOfAny(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int LastIndexOfAny(char[] value, int index)
        {
            return (this.LastIndexOfAny(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int LastIndexOfAny(char[] value, int index, int length)
        {
            return (this.LastIndexOfAny(value, index, length, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY character in a specified array of characters.</summary>
        public int LastIndexOfAny(char[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);

            for (int i = 0; i < value.Length; ++i)
            {
                int pos = this.InternalLastIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (pos);
            }

            return (-1);
        }


        /// <summary>Reports the index of the last occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int LastIndexOfAny(string[] value)
        {
            return (this.LastIndexOfAny(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int LastIndexOfAny(string[] value, int index)
        {
            return (this.LastIndexOfAny(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int LastIndexOfAny(string[] value, int index, int length)
        {
            return (this.LastIndexOfAny(value, index, length, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY string in a specified array of strings.</summary>
        public int LastIndexOfAny(string[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);

            for (int i = 0; i < value.Length; ++i)
            {
                if (string.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalLastIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (pos);
            }

            return (-1);
        }


        /// <summary>Reports the index of the last occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int LastIndexOfAny(StringBuffer[] value)
        {
            return (this.LastIndexOfAny(value, 0, this.Length, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int LastIndexOfAny(StringBuffer[] value, int index)
        {
            return (this.LastIndexOfAny(value, index, this.Length - index, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int LastIndexOfAny(StringBuffer[] value, int index, int length)
        {
            return (this.LastIndexOfAny(value, index, length, false));
        }

        /// <summary>Reports the index of the last occurrence in this instance of ANY string buffer in a specified array of strings buffer.</summary>
        public int LastIndexOfAny(StringBuffer[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (-1);

            for (int i = 0; i < value.Length; ++i)
            {
                if (StringBuffer.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalLastIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (pos);
            }

            return (-1);
        }

        #endregion

        
        #region "CONTAINS | CONTAINSSEQUENCE | CONTAINSALL | CONTAINSANY"

        /// <summary>Returns a value indicating whether the specified character occurs within this instance.</summary>        
        public bool Contains(char value)
        {
            return (this.Contains(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether the specified character occurs within this instance.</summary>        
        public bool Contains(char value, int index)
        {
            return (this.Contains(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether the specified character occurs within this instance.</summary>        
        public bool Contains(char value, int index, int length)
        {
            return (this.Contains(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether the specified character occurs within this instance.</summary> 
        public bool Contains(char value, int index, int length, bool ignoreCase)
        {
            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            if (this.InternalFirstIndexOf(value, index, length, ignoreCase) == -1)
                return (false);

            return (true);
        }


        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>
        public bool Contains(string value)
        {
            return (this.Contains(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>   
        public bool Contains(string value, int index)
        {
            return (this.Contains(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>   
        public bool Contains(string value, int index, int length)
        {
            return (this.Contains(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>
        public bool Contains(string value, int index, int length, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if ((length == 0) || (value.Length > length))
                return (false);

            if (this.InternalFirstIndexOf(value, index, length, ignoreCase) == -1)
                return (false);

            return (true);
        }


        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>
        public bool Contains(StringBuffer value)
        {
            return (this.Contains(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>   
        public bool Contains(StringBuffer value, int index)
        {
            return (this.Contains(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>   
        public bool Contains(StringBuffer value, int index, int length)
        {
            return (this.Contains(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether the specified string occurs within this instance.</summary>
        public bool Contains(StringBuffer value, int index, int length, bool ignoreCase)
        {
            if (StringBuffer.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if ((length == 0) || (value.Length > length))
                return (false);

            if (this.InternalFirstIndexOf(value, index, length, ignoreCase) == -1)
                return (false);

            return (true);
        }


        /// <summary>Determines whether some characters in this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool ContainsSequence(char[] value)
        {
            return (this.ContainsSequence(value, 0, this.Length, false, false));
        }

        /// <summary>Determines whether some characters in this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool ContainsSequence(char[] value, int index)
        {
            return (this.ContainsSequence(value, index, this.Length - index, false, false));
        }

        /// <summary>Determines whether some characters in this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool ContainsSequence(char[] value, int index, int length)
        {
            return (this.ContainsSequence(value, index, length, false, false));
        }

        /// <summary>Determines whether some characters in this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool ContainsSequence(char[] value, int index, int length, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            int pos = index;
            
            while (pos != -1)
            {
                pos = this.InternalFirstIndexOf(value[0], pos, index + length - pos, ignoreCase);                
                if (pos == -1)
                    return (false);
                
                bool fnd = true;
                int  idx = ++pos;
                
                for (int i = 1; i < value.Length; ++i)
                {
                    if (idx >= (index + length))
                    {
                        fnd = false;
                        break;
                    }

                    char c = value[i];

                    if (ignoreWhitespace)
                    {
                        if (char.IsWhiteSpace(c))
                            throw new Exception("You couldn't ignore whitespace and check if this instance contains a whitespace character.");

                        while (this.ContainsAnyWhitespace(idx))
                        {
                            idx++;
                            if (idx >= (index + length))
                            {
                                fnd = false;
                                break;
                            }
                        }
                    }

                    if (!this.InternalContains(c, idx, ignoreCase))
                    {
                        fnd = false;
                        break;
                    }

                    idx++;
                }

                if (fnd)
                {
                    return (true);
                }
            }

            return (false);
        }


        /// <summary>Determines whether some strings in this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool ContainsSequence(string[] value)
        {
            return (this.ContainsSequence(value, 0, this.Length, false, false));
        }

        /// <summary>Determines whether some strings in this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool ContainsSequence(string[] value, int index)
        {
            return (this.ContainsSequence(value, index, this.Length - index, false, false));
        }

        /// <summary>Determines whether some strings in this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool ContainsSequence(string[] value, int index, int length)
        {
            return (this.ContainsSequence(value, index, length, false, false));
        }

        /// <summary>Determines whether some strings in this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool ContainsSequence(string[] value, int index, int length, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            if (string.IsNullOrEmpty(value[0]))
                throw new ArgumentNullException("value");

            int pos = index;
            
            while (pos != -1)
            {
                pos = this.InternalFirstIndexOf(value[0], pos, index + length - pos, ignoreCase);

                if (pos == -1)
                    return (false);

                pos += value[0].Length;

                bool fnd = true;
                int  idx = pos;
                
                for (int i = 1; i < value.Length; ++i)
                {
                    string str = value[i];

                    if (string.IsNullOrEmpty(str))
                        throw new ArgumentNullException("value");

                    if ((idx >= (index + length)) || ((index + length - idx) < str.Length))
                    {                        
                        fnd = false;
                        break;
                    }

                    if (ignoreWhitespace)
                    {
                        if (str.IndexOfAny(StringBuffer.Whitespace) != -1)
                            throw new Exception("You couldn't ignore whitespace and check if this instance contains a string with a whitespace.");

                        while (this.ContainsAnyWhitespace(idx))
                        {
                            idx++;
                            if (idx >= (index + length))
                            {                                
                                fnd = false;
                                break;
                            }
                        }

                        if ((index + length - idx) < str.Length)
                        {                            
                            fnd = false;
                            break;
                        }
                    }

                    if (!this.InternalContains(str, idx, str.Length, ignoreCase))
                    {                        
                        fnd = false;
                        break;
                    }

                    idx += str.Length;
                }

                if (fnd)
                {
                    return (true);
                }
            }

            return (false);
        }


        /// <summary>Determines whether some string buffers in this instance matches sequentially ALL the set of string  buffer specified in an array of string buffer in their respective order.</summary>
        public bool ContainsSequence(StringBuffer[] value)
        {
            return (this.ContainsSequence(value, 0, this.Length, false, false));
        }

        /// <summary>Determines whether some string buffers in this instance matches sequentially ALL the set of string  buffer specified in an array of string buffer in their respective order.</summary>
        public bool ContainsSequence(StringBuffer[] value, int index)
        {
            return (this.ContainsSequence(value, index, this.Length - index, false, false));
        }

        /// <summary>Determines whether some string buffers in this instance matches sequentially ALL the set of string  buffer specified in an array of string buffer in their respective order.</summary>
        public bool ContainsSequence(StringBuffer[] value, int index, int length)
        {
            return (this.ContainsSequence(value, index, length, false, false));
        }

        /// <summary>Determines whether some string buffers in this instance matches sequentially ALL the set of string  buffer specified in an array of string buffer in their respective order.</summary>
        public bool ContainsSequence(StringBuffer[] value, int index, int length, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            if (StringBuffer.IsNullOrEmpty(value[0]))
                throw new ArgumentNullException("value");

            int pos = index;

            while (pos != -1)
            {
                pos = this.InternalFirstIndexOf(value[0], pos, index + length - pos, ignoreCase);
                if (pos == -1)
                    return (false);

                pos += value[0].Length;

                bool fnd = true;
                int  idx = pos;

                for (int i = 1; i < value.Length; ++i)
                {
                    StringBuffer str = value[i];

                    if (StringBuffer.IsNullOrEmpty(str))
                        throw new ArgumentNullException("value");

                    if ((idx >= (index + length)) || ((index + length - idx) < str.Length))
                    {
                        fnd = false;
                        break;
                    }

                    if (ignoreWhitespace)
                    {
                        if (str.IndexOfAny(StringBuffer.Whitespace) != -1)
                            throw new Exception("You couldn't ignore whitespace and check if this instance contains a string buffer with a whitespace.");

                        while (this.ContainsAnyWhitespace(idx))
                        {
                            idx++;
                            if (idx >= (index + length))
                            {
                                fnd = false;
                                break;
                            }
                        }

                        if ((index + length - idx) < str.Length)
                        {
                            fnd = false;
                            break;
                        }
                    }

                    if (!this.InternalContains(str, idx, str.Length, ignoreCase))
                    {
                        fnd = false;
                        break;
                    }

                    idx += str.Length;
                }

                if (fnd)
                {
                    return (true);
                }
            }

            return (false);
        }
        

        /// <summary>Returns a value indicating whether ALL the characters in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAll(char[] value)
        {
            return (this.ContainsAll(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether ALL the characters in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAll(char[] value, int index)
        {
            return (this.ContainsAll(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether ALL the characters in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAll(char[] value, int index, int length)
        {
            return (this.ContainsAll(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether ALL the characters in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAll(char[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            for (int i = 0; i < value.Length; ++i)
            {
                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                if (pos == -1)
                    return (false);
            }

            return (true);
        }


        /// <summary>Returns a value indicating whether ALL the strings in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAll(string[] value)
        {
            return (this.ContainsAll(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether ALL the strings in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAll(string[] value, int index)
        {
            return (this.ContainsAll(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether ALL the strings in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAll(string[] value, int index, int length)
        {
            return (this.ContainsAll(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether ALL the strings in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAll(string[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            for (int i = 0; i < value.Length; ++i)
            {
                if (string.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (true);
            }

            return (false);
        }


        /// <summary>Returns a value indicating whether ALL the strings buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAll(StringBuffer[] value)
        {
            return (this.ContainsAll(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether ALL the strings buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAll(StringBuffer[] value, int index)
        {
            return (this.ContainsAll(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether ALL the strings buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAll(StringBuffer[] value, int index, int length)
        {
            return (this.ContainsAll(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether ALL the strings buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAll(StringBuffer[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            for (int i = 0; i < value.Length; ++i)
            {
                if (StringBuffer.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (true);
            }

            return (false);
        }
        
        
        /// <summary>Returns a value indicating whether ANY character in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAny(char[] value)
        {
            return (this.ContainsAny(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether ANY character in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAny(char[] value, int index)
        {
            return (this.ContainsAny(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether ANY character in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAny(char[] value, int index, int length)
        {
            return (this.ContainsAny(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether ANY character in a specified array of characters occurs in this instance.</summary>
        public bool ContainsAny(char[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            for (int i = 0; i < value.Length; ++i)
            {
                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (true);
            }

            return (false);
        }


        /// <summary>Returns a value indicating whether ANY string in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAny(string[] value)
        {
            return (this.ContainsAny(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether ANY string in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAny(string[] value, int index)
        {
            return (this.ContainsAny(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether ANY string in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAny(string[] value, int index, int length)
        {
            return (this.ContainsAny(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether ANY string in a specified array of strings occurs in this instance.</summary>
        public bool ContainsAny(string[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            for (int i = 0; i < value.Length; ++i)
            {
                if (string.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (true);
            }

            return (false);
        }


        /// <summary>Returns a value indicating whether ANY string buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAny(StringBuffer[] value)
        {
            return (this.ContainsAny(value, 0, this.Length, false));
        }

        /// <summary>Returns a value indicating whether ANY string buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAny(StringBuffer[] value, int index)
        {
            return (this.ContainsAny(value, index, this.Length - index, false));
        }

        /// <summary>Returns a value indicating whether ANY string buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAny(StringBuffer[] value, int index, int length)
        {
            return (this.ContainsAny(value, index, length, false));
        }

        /// <summary>Returns a value indicating whether ANY string buffer in a specified array of strings buffer occurs in this instance.</summary>
        public bool ContainsAny(StringBuffer[] value, int index, int length, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if ((index < 0) || (index >= this.Length))
                throw new ArgumentOutOfRangeException("index");

            if ((length < 0) || ((index + length) > this.Length))
                throw new ArgumentOutOfRangeException("length");

            if (length == 0)
                return (false);

            for (int i = 0; i < value.Length; ++i)
            {
                if (StringBuffer.IsNullOrEmpty(value[i]))
                    throw new ArgumentNullException("value");

                if ((value[i].Length == 0) || (value[i].Length > length))
                    continue;

                int pos = this.InternalFirstIndexOf(value[i], index, length, ignoreCase);
                if (pos != -1)
                    return (true);
            }

            return (false);
        }
        
        #endregion


        #region "STARTSWITH | STARTSWITHSEQUENCE | STARTSWITHANY"

        /// <summary>Determines whether the beginning of this instance matches the specified character.</summary>        
        public bool StartsWith(char value)
        {
            return (this.StartsWith(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches the specified character.</summary>
        public bool StartsWith(char value, bool ignoreCase)
        {
            return (this.StartsWith(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches the specified character.</summary>
        public bool StartsWith(char value, bool ignoreCase, bool ignoreWhitespace)
        {            
            if ((this.Length == 0))
                return (false);

            int idx = 0;

            if (ignoreWhitespace)
            {
                if (char.IsWhiteSpace(value))
                    throw new Exception("You couldn't ignore whitespace and check if this instance starts with a whitespace character.");

                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (++idx >= this.Length)
                        return (false);
                }
            }

            if (this.InternalContains(value, idx, ignoreCase))
                return (true);

            return (false);
        }


        /// <summary>Determines whether the beginning of this instance matches the specified string.</summary>        
        public bool StartsWith(string value)
        {
            return (this.StartsWith(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches the specified string.</summary>
        public bool StartsWith(string value, bool ignoreCase)
        {
            return (this.StartsWith(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches the specified string.</summary>
        public bool StartsWith(string value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((this.Length == 0) || (value.Length > this.Length))
                return (false);

            int idx = 0;

            if (ignoreWhitespace)
            {
                if (value.IndexOfAny(StringBuffer.Whitespace) != -1) 
                    throw new Exception("You couldn't ignore whitespace and check if this instance starts with a string containing a whitespace.");
                                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (++idx >= this.Length)
                        return (false);
                }

                if ((this.Length - idx) < value.Length)
                    return (false);
            }            

            if (this.InternalContains(value, idx, value.Length, ignoreCase))
                return (true);

            return (false);
        }


        /// <summary>Determines whether the beginning of this instance matches the specified string buffer.</summary>        
        public bool StartsWith(StringBuffer value)
        {
            return (this.StartsWith(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches the specified string buffer.</summary>
        public bool StartsWith(StringBuffer value, bool ignoreCase)
        {
            return (this.StartsWith(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches the specified string buffer.</summary>
        public bool StartsWith(StringBuffer value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (StringBuffer.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((this.Length == 0) || (value.Length > this.Length))
                return (false);

            int idx = 0;

            if (ignoreWhitespace)
            {
                if (value.ContainsAny(StringBuffer.Whitespace))
                    throw new Exception("You couldn't ignore whitespace and check if this instance starts with a string buffer containing a whitespace.");
                                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (++idx >= this.Length)
                        return (false);
                }

                if ((this.Length - idx) < value.Length)
                    return (false);
            }

            if (this.InternalContains(value, idx, value.Length, ignoreCase))
                return (true);

            return (false);
        }


        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool StartsWithSequence(char[] value)
        {
            return (this.StartsWithSequence(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool StartsWithSequence(char[] value, bool ignoreCase)
        {
            return (this.StartsWithSequence(value, ignoreCase, false));
        }

        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool StartsWithSequence(char[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                if (idx >= this.Length)
                    return (false);

                char c = value[i];
                
                if (ignoreWhitespace)
                {
                    if (char.IsWhiteSpace(c))
                        throw new Exception("You couldn't ignore whitespace and check if this instance start with a whitespace character.");
                                                           
                    while (this.ContainsAnyWhitespace(idx))
                    {                        
                        if (++idx >= this.Length)
                            return (false);
                    }                    
                }
                                
                if (!this.InternalContains(c, idx, ignoreCase))
                    return (false);

                idx++;
            }

            return (true);
        }


        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool StartsWithSequence(string[] value)
        {
            return (this.StartsWithSequence(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool StartsWithSequence(string[] value, bool ignoreCase)
        {
            return (this.StartsWithSequence(value, ignoreCase, false));
        }

        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool StartsWithSequence(string[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                string str = value[i];

                if (string.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((idx >= this.Length) || ((this.Length - idx) < str.Length)) 
                    return (false);
                             
                if (ignoreWhitespace)
                {
                    if (str.IndexOfAny(StringBuffer.Whitespace) != -1)
                        throw new Exception("You couldn't ignore whitespace and check if this instance starts with a string containing a whitespace.");
                                        
                    while (this.ContainsAnyWhitespace(idx))
                    {                        
                        if (++idx >= this.Length)
                            return (false);
                    }

                    if ((this.Length - idx) < str.Length)
                        return (false);
                }

                if (!this.InternalContains(str, idx, str.Length, ignoreCase))
                    return (false);

                idx += str.Length;
            }

            return (true);
        }


        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of string buffer specified in an array of strings buffer in their respective order.</summary>
        public bool StartsWithSequence(StringBuffer[] value)
        {
            return (this.StartsWithSequence(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of string buffer specified in an array of strings buffer in their respective order.</summary>
        public bool StartsWithSequence(StringBuffer[] value, bool ignoreCase)
        {
            return (this.StartsWithSequence(value, ignoreCase, false));
        }

        /// <summary>Determines whether the beginning of this instance matches sequentially ALL the set of string buffer specified in an array of strings buffer in their respective order.</summary>
        public bool StartsWithSequence(StringBuffer[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                StringBuffer str = value[i];

                if (StringBuffer.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");
                
                if ((idx >= this.Length) || ((this.Length - idx) < str.Length))
                    return (false);

                if (ignoreWhitespace)
                {
                    if (str.IndexOfAny(StringBuffer.Whitespace) != -1)
                        throw new Exception("You couldn't ignore whitespace and check if this instance starts with a string buffer containing a whitespace.");
                                        
                    while (this.ContainsAnyWhitespace(idx))
                    {                        
                        if (++idx >= this.Length)
                            return (false);
                    }

                    if ((this.Length - idx) < str.Length)
                        return (false);
                }

                if (!this.InternalContains(str, idx, str.Length, ignoreCase))
                    return (false);

                idx += str.Length;
            }

            return (true);
        }


        /// <summary>Determines whether the beginning of this instance matches ANY set of character specified in an array of characters.</summary>
        public bool StartsWithAny(char[] value)
        {
            return (this.StartsWithAny(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches ANY set of character specified in an array of characters.</summary>
        public bool StartsWithAny(char[] value, bool ignoreCase)
        {
            return (this.StartsWithAny(value, ignoreCase, false));
        }

        /// <summary>Determines whether the beginning of this instance matches ANY set of character specified in an array of characters.</summary>
        public bool StartsWithAny(char[] value, bool ignoreCase, bool ignoreWhitespace)
        {            
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = 0;

            if (ignoreWhitespace)
            {                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (++idx >= this.Length)
                        return (false);
                }
            }

            for (int i = 0; i < value.Length; ++i)
            {                
                if (ignoreWhitespace && (char.IsWhiteSpace(value[i])))
                    throw new Exception("You couldn't ignore whitespace and check if this instance start with a whitespace character.");

                if (this.InternalContains(value[i], idx, ignoreCase))
                    return (true);
            }

            return (false);
        }


        /// <summary>Determines whether the beginning of this instance matches ANY set of string specified in an array of strings.</summary>
        public bool StartsWithAny(string[] value)
        {
            return (this.StartsWithAny(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches ANY set of string specified in an array of strings.</summary>
        public bool StartsWithAny(string[] value, bool ignoreCase)
        {
            return (this.StartsWithAny(value, ignoreCase, false));
        }

        /// <summary>Determines whether the beginning of this instance matches ANY set of string specified in an array of strings.</summary>
        public bool StartsWithAny(string[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = 0;

            if (ignoreWhitespace)
            {                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (++idx >= this.Length)
                        return (false);
                }
            }

            for (int i = 0; i < value.Length; ++i)
            {                
                string str = value[i];

                if (string.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((this.Length - idx) < str.Length)
                    continue;

                if (ignoreWhitespace && (str.IndexOfAny(StringBuffer.Whitespace) != -1))
                    throw new Exception("You couldn't ignore whitespace and check if this instance starts with a string containing a whitespace.");
                
                if (this.InternalContains(str, idx, str.Length, ignoreCase))
                    return (true);
            }

            return (false);
        }


        /// <summary>Determines whether the beginning of this instance matches ANY set of string buffer specified in an array of strings buffer.</summary>
        public bool StartsWithAny(StringBuffer[] value)
        {
            return (this.StartsWithAny(value, false, false));
        }

        /// <summary>Determines whether the beginning of this instance matches ANY set of string buffer specified in an array of strings buffer.</summary>
        public bool StartsWithAny(StringBuffer[] value, bool ignoreCase)
        {
            return (this.StartsWithAny(value, ignoreCase, false));
        }

        /// <summary>Determines whether the beginning of this instance matches ANY set of string buffer specified in an array of strings buffer.</summary>
        public bool StartsWithAny(StringBuffer[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = 0;

            if (ignoreWhitespace)
            {                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (++idx >= this.Length)
                        return (false);
                }
            }

            for (int i = 0; i < value.Length; ++i)
            {
                StringBuffer str = value[i];

                if (StringBuffer.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((this.Length - idx) < str.Length)
                    continue;

                if (ignoreWhitespace && (str.IndexOfAny(StringBuffer.Whitespace) != -1))
                    throw new Exception("You couldn't ignore whitespace and check if this instance starts with a string buffer containing a whitespace.");

                if (this.InternalContains(str, idx, str.Length, ignoreCase))
                    return (true);
            }

            return (false);
        }
        
        #endregion


        #region "ENDSWITH | ENDSWITHSEQUENCE | ENDSWITHANY

        /// <summary>Determines whether the end of this instance matches the specified character.</summary>        
        public bool EndsWith(char value)
        {
            return (this.EndsWith(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches the specified character.</summary>
        public bool EndsWith(char value, bool ignoreCase)
        {
            return (this.EndsWith(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches the specified character.</summary>
        public bool EndsWith(char value, bool ignoreCase, bool ignoreWhitespace)
        {            
            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            if (ignoreWhitespace)
            {
                if (char.IsWhiteSpace(value))
                    throw new Exception("You couldn't ignore whitespace and check if this instance ends with a whitespace character.");

                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (--idx < 0)
                        return (false);
                }                
            }

            if (this.InternalContains(value, idx, ignoreCase))
                return (true);

            return (false);
        }


        /// <summary>Determines whether the end of this instance matches the specified string.</summary>        
        public bool EndsWith(string value)
        {
            return (this.EndsWith(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches the specified string.</summary>
        public bool EndsWith(string value, bool ignoreCase)
        {
            return (this.EndsWith(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches the specified string.</summary>
        public bool EndsWith(string value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((this.Length == 0) || (value.Length > this.Length))
                return (false);

            int idx = this.Length - 1;

            if (ignoreWhitespace)
            {
                if (value.IndexOfAny(StringBuffer.Whitespace) != -1)
                    throw new Exception("You couldn't ignore whitespace and check if this instance ends with a string containing a whitespace.");
                                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (--idx < 0)
                        return (false);
                }

                if ((idx + 1) < value.Length)
                    return (false);
            }

            if (this.InternalContains(value, idx + 1 - value.Length, value.Length, ignoreCase))
                return (true);

            return (false);
        }


        /// <summary>Determines whether the end of this instance matches the specified string buffer.</summary>        
        public bool EndsWith(StringBuffer value)
        {
            return (this.EndsWith(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches the specified string buffer.</summary>
        public bool EndsWith(StringBuffer value, bool ignoreCase)
        {
            return (this.EndsWith(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches the specified string buffer.</summary>
        public bool EndsWith(StringBuffer value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (StringBuffer.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if ((this.Length == 0) || (value.Length > this.Length))
                return (false);

            int idx = this.Length - 1;

            if (ignoreWhitespace)
            {
                if (value.ContainsAny(StringBuffer.Whitespace))
                    throw new Exception("You couldn't ignore whitespace and check if this instance ends with a string buffer containing a whitespace.");
                                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (--idx < 0)
                        return (false);
                }

                if ((idx + 1) < value.Length)
                    return (false);
            }

            if (this.InternalContains(value, idx + 1 - value.Length, value.Length, ignoreCase))
                return (true);

            return (false);
        }


        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool EndsWithSequence(char[] value)
        {
            return (this.EndsWithSequence(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool EndsWithSequence(char[] value, bool ignoreCase)
        {
            return (this.EndsWithSequence(value, ignoreCase, false));
        }

        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of character specified in an array of characters in their respective order.</summary>
        public bool EndsWithSequence(char[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            for (int i = value.Length - 1; i >= 0; --i)
            {
                if (idx < 0)
                    return (false);

                char c = value[i];

                if (ignoreWhitespace)
                {
                    if (char.IsWhiteSpace(c))
                        throw new Exception("You couldn't ignore whitespace and check if this instance ends with a whitespace character.");
                                        
                    while (this.ContainsAnyWhitespace(idx))
                    {                        
                        if (--idx < 0)
                            return (false);
                    }
                }                

                if (!this.InternalContains(c, idx, ignoreCase))
                    return (false);

                idx--;
            }

            return (true);
        }


        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool EndsWithSequence(string[] value)
        {
            return (this.EndsWithSequence(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool EndsWithSequence(string[] value, bool ignoreCase)
        {
            return (this.EndsWithSequence(value, ignoreCase, false));
        }

        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of string specified in an array of strings in their respective order.</summary>
        public bool EndsWithSequence(string[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            for (int i = value.Length - 1; i >= 0; --i)
            {
                string str = value[i];

                if (string.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((idx < 0) || ((idx + 1) < str.Length))
                    return (false);

                if (ignoreWhitespace)
                {
                    if (str.IndexOfAny(StringBuffer.Whitespace) != -1)
                        throw new Exception("You couldn't ignore whitespace and check if this instance end with a string containing a whitespace.");
                                        
                    while (this.ContainsAnyWhitespace(idx))
                    {                        
                        if (--idx < 0)
                            return (false);
                    }

                    if ((idx + 1) < str.Length)
                        return (false);
                }
                
                if (!this.InternalContains(str, idx + 1 - str.Length, str.Length, ignoreCase))
                    return (false);

                idx -= str.Length;
            }

            return (true);
        }


        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of string buffer specified in an array of strings buffer in their respective order.</summary>
        public bool EndsWithSequence(StringBuffer[] value)
        {
            return (this.EndsWithSequence(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of string buffer specified in an array of strings buffer in their respective order.</summary>
        public bool EndsWithSequence(StringBuffer[] value, bool ignoreCase)
        {
            return (this.EndsWithSequence(value, ignoreCase, false));
        }

        /// <summary>Determines whether the end of this instance matches sequentially ALL the set of string buffer specified in an array of strings buffer in their respective order.</summary>
        public bool EndsWithSequence(StringBuffer[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            for (int i = value.Length - 1; i >= 0; --i)
            {
                StringBuffer str = value[i];

                if (StringBuffer.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((idx < 0) || ((idx + 1) < str.Length))
                    return (false);

                if (ignoreWhitespace)
                {
                    if (str.IndexOfAny(StringBuffer.Whitespace) != -1)
                        throw new Exception("You couldn't ignore whitespace and check if this instance end with a string buffer containing a whitespace.");
                                        
                    while (this.ContainsAnyWhitespace(idx))
                    {                        
                        if (--idx < 0)
                            return (false);
                    }

                    if ((idx + 1) < str.Length)
                        return (false);
                }

                if (!this.InternalContains(str, idx + 1 - str.Length, str.Length, ignoreCase))
                    return (false);

                idx -= str.Length;
            }

            return (true);
        }
                

        /// <summary>Determines whether the end of this instance matches ANY set of character specified in an array of characters.</summary>
        public bool EndsWithAny(char[] value)
        {
            return (this.EndsWithAny(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches ANY set of character specified in an array of characters.</summary>
        public bool EndsWithAny(char[] value, bool ignoreCase)
        {
            return (this.EndsWithAny(value, ignoreCase, false));
        }

        /// <summary>Determines whether the end of this instance matches ANY set of character specified in an array of characters.</summary>
        public bool EndsWithAny(char[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            if (ignoreWhitespace)
            {                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (--idx < 0)
                        return (false);
                }
            }

            for (int i = value.Length - 1; i >= 0; --i)
            {                
                if (ignoreWhitespace && (char.IsWhiteSpace(value[i])))
                    throw new Exception("You couldn't ignore whitespace and check if this instance ends with a whitespace character.");

                if (this.InternalContains(value[i], idx, ignoreCase))
                    return (true);
            }

            return (false);
        }


        /// <summary>Determines whether the end of this instance matches ANY set of string specified in an array of strings.</summary>
        public bool EndsWithAny(string[] value)
        {
            return (this.StartsWithAny(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches ANY set of string specified in an array of strings.</summary>
        public bool EndsWithAny(string[] value, bool ignoreCase)
        {
            return (this.StartsWithAny(value, ignoreCase, false));
        }

        /// <summary>Determines whether the end of this instance matches ANY set of string specified in an array of strings.</summary>
        public bool EndsWithAny(string[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            if (ignoreWhitespace)
            {                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (--idx < 0)
                        return (false);
                }
            }

            for (int i = 0; i < value.Length; ++i)
            {
                string str = value[i];

                if (string.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((idx + 1) < str.Length)
                    continue;

                if (ignoreWhitespace && (str.IndexOfAny(StringBuffer.Whitespace) != -1))
                    throw new Exception("You couldn't ignore whitespace and check if this instance ends with a string containing a whitespace.");

                if (this.InternalContains(str, idx + 1 - str.Length, str.Length, ignoreCase))
                    return (true);
            }

            return (false);
        }


        /// <summary>Determines whether the end of this instance matches ANY set of string buffer specified in an array of strings buffer.</summary>
        public bool EndsWithAny(StringBuffer[] value)
        {
            return (this.EndsWithAny(value, false, false));
        }

        /// <summary>Determines whether the end of this instance matches ANY set of string buffer specified in an array of strings buffer.</summary>
        public bool EndsWithAny(StringBuffer[] value, bool ignoreCase)
        {
            return (this.StartsWithAny(value, ignoreCase, false));
        }

        /// <summary>Determines whether the end of this instance matches ANY set of string buffer specified in an array of strings buffer.</summary>
        public bool EndsWithAny(StringBuffer[] value, bool ignoreCase, bool ignoreWhitespace)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length == 0)
                throw new ArgumentException("value could not be empty.");

            if (this.Length == 0)
                return (false);

            int idx = this.Length - 1;

            if (ignoreWhitespace)
            {                
                while (this.ContainsAnyWhitespace(idx))
                {                    
                    if (--idx < 0)
                        return (false);
                }
            }

            for (int i = 0; i < value.Length; ++i)
            {
                StringBuffer str = value[i];

                if (StringBuffer.IsNullOrEmpty(str))
                    throw new ArgumentNullException("value");

                if ((idx + 1) < str.Length)
                    continue;

                if (ignoreWhitespace && (str.IndexOfAny(StringBuffer.Whitespace) != -1))
                    throw new Exception("You couldn't ignore whitespace and check if this instance ends with a string buffer containing a whitespace.");

                if (this.InternalContains(str, idx + 1 - str.Length, str.Length, ignoreCase))
                    return (true);
            }

            return (false);
        }
        
        #endregion
        


        #region "SPLIT | SPLITTOBUFFER"

        public string[] Split(StringSplitOptions options)
        {
            return (this.Split(new char[0], int.MaxValue, options));
        }

        public string[] Split(int maxString, StringSplitOptions options)
        {
            return (this.Split(new char[0], maxString, options));
        }

        
        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified character array.</summary>    
        public string[] Split(char[] delimiter)
        {
            return (this.Split(delimiter, int.MaxValue, StringSplitOptions.None));
        }

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified character array.
        /// A parameter specify the maximum number of string to return.</summary>    
        public string[] Split(char[] delimiter, int maxString)
        {
            return (this.Split(delimiter, maxString, StringSplitOptions.None));
        }

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified character array.
        /// A parameter specify whether to return empty array elements.</summary>    
        public string[] Split(char[] delimiter, StringSplitOptions options)
        {
            return (this.Split(delimiter, int.MaxValue, options));
        }

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified character array.
        /// Parameters specify the maximum number of string to return and whether to return empty array elements.</summary>    
        public string[] Split(char[] delimiter, int maxString, StringSplitOptions options)
        {
            if (delimiter == null)
                delimiter = StringBuffer.Whitespace;
            else if (delimiter.Length == 0)
                delimiter = StringBuffer.Whitespace;

            if (maxString < 0)
                throw new ArgumentOutOfRangeException("maxString");

            bool omitEmptyEntries = (options == StringSplitOptions.RemoveEmptyEntries);

            if ((maxString == 0) || (omitEmptyEntries && (this.Length == 0)))
                return (new string[0]);

            int[] idxDelimiter;
            int replaceCount = this.Delimit(delimiter, out idxDelimiter);

            // Handle the special case of no replaces and special count.
            if ((replaceCount == 0) || (maxString == 1))
                return (new string[1] { this.ToString() });

            int[] lengthList = null;

            if (omitEmptyEntries)
                return (this.SplitOmitEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
            else
                return (this.SplitKeepEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
        }
        

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array.</summary>
        public string[] Split(string[] delimiter)
        {
            return (this.Split(delimiter, int.MaxValue, StringSplitOptions.None));
        }

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// A Parameter specify the maximum number of string to return</summary>
        public string[] Split(string[] delimiter, int maxString)
        {
            return (this.Split(delimiter, maxString, StringSplitOptions.None));
        }

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// A parameter specify whether to return empty array elements.</summary>    
        public string[] Split(string[] delimiter, StringSplitOptions options)
        {
            return (this.Split(delimiter, int.MaxValue, options));
        }

        /// <summary>Returns a string array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// Parameters specify the maximum number of string to return and whether to return empty array elements.</summary>   
        public string[] Split(string[] delimiter, int maxString, StringSplitOptions options)
        {
            if (delimiter == null)
                return (this.Split(StringBuffer.Whitespace, maxString, options));
            else if (delimiter.Length == 0)
                return (this.Split(StringBuffer.Whitespace, maxString, options));

            if (maxString < 0)
                throw new ArgumentOutOfRangeException("maxString");

            bool omitEmptyEntries = (options == StringSplitOptions.RemoveEmptyEntries);

            if ((maxString == 0) || (omitEmptyEntries && (this.Length == 0)))
                return (new string[0]);

            int[] lengthList;
            int[] idxDelimiter;
            int replaceCount = this.Delimit(delimiter, out idxDelimiter, out lengthList);

            // Handle the special case of no replaces and special count.
            if ((replaceCount == 0) || (maxString == 1))
                return (new string[1] { this.ToString() });

            if (omitEmptyEntries)
                return (this.SplitOmitEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
            else
                return (this.SplitKeepEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
        }


        public StringBuffer[] SplitToBuffer(StringSplitOptions options)
        {
            return (this.SplitToBuffer(new char[0], int.MaxValue, options));
        }

        public StringBuffer[] SplitToBuffer(int maxString, StringSplitOptions options)
        {
            return (this.SplitToBuffer(new char[0], maxString, options));
        }


        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified character array.</summary>
        public StringBuffer[] SplitToBuffer(char[] delimiter)
        {
            return (this.SplitToBuffer(delimiter, int.MaxValue, StringSplitOptions.None));
        }

        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified character array.
        /// A parameter specify the maximum number of StringBuffer to return.</summary>
        public StringBuffer[] SplitToBuffer(char[] delimiter, int maxString)
        {
            return (this.SplitToBuffer(delimiter, maxString, StringSplitOptions.None));
        }

        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified character array.
        /// A parameter specify whether to return empty array elements.</summary>
        public StringBuffer[] SplitToBuffer(char[] delimiter, StringSplitOptions options)
        {
            return (this.SplitToBuffer(delimiter, int.MaxValue, options));
        }

        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified character array.
        /// Parameters specify the maximum number of StringBuffer to return and whether to return empty array elements.</summary>
        public StringBuffer[] SplitToBuffer(char[] delimiter, int maxString, StringSplitOptions options)
        {
            if (delimiter == null)
                delimiter = StringBuffer.Whitespace;
            else if (delimiter.Length == 0)
                delimiter = StringBuffer.Whitespace;

            if ((maxString < 0) || (maxString > int.MaxValue))
                throw new ArgumentOutOfRangeException("maxString");

            bool omitEmptyEntries = (options == StringSplitOptions.RemoveEmptyEntries);

            if ((maxString == 0) || (omitEmptyEntries && (this.Length == 0)))
                return (new StringBuffer[0]);

            int[] idxDelimiter;
            int replaceCount = this.Delimit(delimiter, out idxDelimiter);

            // Handle the special case of no replaces and special count.
            if ((replaceCount == 0) || (maxString == 1))
                return (new StringBuffer[1] { this.Copy() });

            int[] lengthList = null;
            
            if (omitEmptyEntries)
                return (this.SplitBufferOmitEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
            else
                return (this.SplitBufferKeepEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
        }


        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified string array.</summary>
        public StringBuffer[] SplitToBuffer(string[] delimiter)
        {
            return (this.SplitToBuffer(delimiter, int.MaxValue, StringSplitOptions.None));
        }

        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// A parameter specify the maximum number of StringBuffer to return.</summary>
        public StringBuffer[] SplitToBuffer(string[] delimiter, int maxString)
        {
            return (this.SplitToBuffer(delimiter, maxString, StringSplitOptions.None));
        }

        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// A parameter specify whether to return empty array elements.</summary>
        public StringBuffer[] SplitToBuffer(string[] delimiter, StringSplitOptions options)
        {
            return (this.SplitToBuffer(delimiter, int.MaxValue, options));
        }

        /// <summary>Returns a StringBuffer array that contains the substrings in this instance that are delimited by elements of a specified string array.
        /// Parameters specify the maximum number of StringBuffer to return and whether to return empty array elements.</summary>
        public StringBuffer[] SplitToBuffer(string[] delimiter, int maxString, StringSplitOptions options)
        {
            if (delimiter == null)
                return (this.SplitToBuffer(StringBuffer.Whitespace, maxString, options));
            else if (delimiter.Length == 0)
                return (this.SplitToBuffer(StringBuffer.Whitespace, maxString, options));

            if ((maxString < 0) || (maxString > int.MaxValue))
                throw new ArgumentOutOfRangeException("maxString");

            bool omitEmptyEntries = (options == StringSplitOptions.RemoveEmptyEntries);

            if ((maxString == 0) || (omitEmptyEntries && (this.Length == 0)))
                return (new StringBuffer[0]);

            int[] lengthList;
            int[] idxDelimiter;
            int replaceCount = this.Delimit(delimiter, out idxDelimiter, out lengthList);

            // Handle the special case of no replaces and special count.
            if ((replaceCount == 0) || (maxString == 1))
                return (new StringBuffer[1] { this.Copy() });

            if (omitEmptyEntries)
                return (this.SplitBufferOmitEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
            else
                return (this.SplitBufferKeepEmptyEntries(ref lengthList, idxDelimiter, replaceCount, maxString));
        }

        #endregion
        

        #region "TOCHARARRAY | TOSTRING | COPY (aka TOSTRINGBUFFER)"

        /// <summary>Copies the characters in this instance to a character array.</summary>
        public char[] ToCharArray()
        {
            return (this.ToCharArray(0, this.Length));
        }

        /// <summary>Copies the characters in this instance to a character array.</summary>
        public char[] ToCharArray(int index)
        {
            return (this.ToCharArray(index, this.Length - index));
        }

        /// <summary>Copies the characters in this instance to a character array.</summary>
        public char[] ToCharArray(int index, int length)
        {
            if ((index < 0) || (index > this.Length))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if ((length < 0) || ((index + length) > this.Length))
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return (new char[0]);
            }

            if (this._cachedStr != null)
            {
                return (this._cachedStr.ToCharArray(index, length));
            }

            char[] result = new char[this.Length];
            var    chunk  = this._chunkHead;
            
            int i = 0;
            while (chunk != null)
            {
                int len = Math.Min(chunk._chars.Length, chunk._length);
                Array.Copy(chunk._chars, 0, result, i, len);
                i += len;
                chunk = chunk._next;
            }

            this._cachedStr = new string(result);

            return (this._cachedStr.ToCharArray(index, length));
        }


        /// <summary>Copies the characters in this instance into a string.</summary>    
        public override string ToString()
        {
            return (this.ToString(0, this.Length));
        }

        /// <summary>Copies the characters in this instance into a string.</summary>    
        public string ToString(int index)
        {
            return (this.ToString(index, this.Length - index));
        }

        /// <summary>Copies the characters in this instance into a string.</summary>    
        public string ToString(int index, int length)
        {
            if ((index < 0) || (index > this.Length))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if ((length < 0) || ((index + length) > this.Length))
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return (string.Empty);
            }
                        
            if (this._cachedStr != null)
            {
                return (this._cachedStr.Substring(index, length));
            }

            char[] result = new char[this.Length];
            var    chunk  = this._chunkHead;
            
            int i = 0;
            while (chunk != null)
            {
                int len = Math.Min(chunk._chars.Length, chunk._length);
                Array.Copy(chunk._chars, 0, result, i, len);
                i += len;
                chunk = chunk._next;
            }

            this._cachedStr = new string(result);

            return (this._cachedStr.Substring(index, length));
        }


        /// <summary>String buffer deep copy.</summary>    
        public StringBuffer Copy()
        {
            return (this.Copy(0, this.Length));
        }

        /// <summary>String buffer deep copy.</summary>    
        public StringBuffer Copy(int index)
        {
            return (this.Copy(index, this.Length - index));
        }

        /// <summary>String buffer deep copy.</summary>
        public StringBuffer Copy(int index, int length)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if ((length < 0) || ((index + length) > this.Length))
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return (StringBuffer.Empty);
            }

            if (this._cachedStr != null)
            {
                return (new StringBuffer(this._cachedStr.Substring(index, length)));
            }

            char[] result = new char[this.Length];
            var    chunk  = this._chunkHead;
            
            while (chunk != null)
            {
                Array.Copy(chunk._chars, 0, result, chunk._offset, chunk._length);
                chunk = chunk._next;
            }

            this._cachedStr = new string(result);

            return (new StringBuffer(this._cachedStr.Substring(index, length)));
        }
        
        #endregion


        #region "IENUMERATOR | IENUMERABLE"

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerator)this);
        }

        public bool MoveNext()
        {
            this._position++;

            if (this._position < this.Length)
            {
                return (true);
            }

            this._position = -1;
            return (false);
        }
        
        public void Reset()
        {
            this._position = -1;
        }
                
        public object Current
        {
            get
            {
                try
                {
                    return (this[this._position]);                    
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException("StringBuffer was modified; enumeration operation may not execute.");
                }                
            }
        }

        public IEnumerable<string> EnumerateLine()
        {
            int index  = 0;
            int length = this.Length;

            this._position = 0;

            while (index < length)
            {
                int endIndex = this.IndexOfAny(StringBuffer.Crlf, index);

                if (endIndex != -1)
                {
                    int len = endIndex + 1;
                    char ch = this[endIndex];

                    if (len < length)
                    {
                        if (((ch == '\r') && (this[len] == '\n')) ||
                            ((ch == '\n') && (this[len] == '\r')))
                        {
                            len++;
                        }
                    }

                    len   -= index;                    
                    yield return (this[index, (endIndex - index)]);
                    index += len;
                }
                else
                {
                    yield return (this[index, length - index]);
                    index = length;
                }
            }

            this._position = -1;
        }
        
        #endregion


        #region "ICOMPARABLE | IEQUATABLE"

        public int CompareTo(object value)
        {
            if (value == null)
                return (1);

            if (!(value is StringBuffer))
                throw new ArgumentException();

            return (StringBuffer.Compare(this, ((StringBuffer)value).ToString()));
        }

        public int CompareTo(StringBuffer other)
        {
            if (other == null)
                return (1);

            return (StringBuffer.Compare(this, other));
        }

        public int CompareTo(string other)
        {
            if (other == null)
                return (1);

            return (StringBuffer.Compare(this, other));
        }
        

        public bool Equals(StringBuffer other)
        {
            if (other == null)
                return (false);

            return (StringBuffer.Equals(this, other));            
        }

        public bool Equals(StringBuffer other, StringComparison comparisonType)
        {
            if (other == null)
                return (false);

            return (StringBuffer.Compare(this, other, comparisonType) == 0);            
        }

        public bool Equals(string other)
        {
            if (other == null)
                return (false);
            
            return (StringBuffer.Compare(this, other) == 0);  
        }

        public bool Equals(string other, StringComparison comparisonType)
        {
            if (other == null)
                return (false);

            return (StringBuffer.Compare(this, other, comparisonType) == 0);            
        }
        

        public override bool Equals(object obj)
        {
            StringBuffer buf = obj as StringBuffer;

            if (buf != null)
            {
                return (this.Equals(buf));
            }
            else
            {
                string str = obj as String;

                if (str != null)
                    return (this.Equals(str));
                else
                    return (false);
            }
        }
        
        public override int GetHashCode()
        {
            return (this.ToString().GetHashCode());
        }

        #endregion

       

        #region "PRIVATE METHODS"

        private void CheckEnumeration()
        {
            if (this._position != -1)
                throw new InvalidOperationException("StringBuffer was modified; enumeration operation may not execute.");
        }
        

        private int InternalFirstIndexOf(char value, int index, int length, bool ignoreCase)
        {
            int maxIndex = index + length - 1;

            while ((length > 0) && (index <= maxIndex))
            {
                if (this.InternalContains(value, index, ignoreCase))
                {
                    return (index);
                }

                index++;
                length--;
            }

            return (-1);
        }

        private int InternalFirstIndexOf(string value, int index, int length, bool ignoreCase)
        {
            int maxIndex = index + length - value.Length;

            while ((length > 0) && (index <= maxIndex))
            {
                if (this.InternalContains(value, index, length, ignoreCase))
                {
                    return (index);
                }

                index++;
                length--;
            }

            return (-1);
        }

        private int InternalFirstIndexOf(StringBuffer value, int index, int length, bool ignoreCase)
        {
            int maxIndex = index + length - value.Length;

            while ((length > 0) && (index <= maxIndex))
            {
                if (this.InternalContains(value, index, length, ignoreCase))
                {
                    return (index);
                }

                index++;
                length--;
            }

            return (-1);
        }
         
       
        private int InternalLastIndexOf(char value, int index, int length, bool ignoreCase)
        {
            int maxIndex = index + length - 1;

            while ((length > 0) && (maxIndex >= index))
            {
                if (this.InternalContains(value, maxIndex, ignoreCase))
                {
                    return (maxIndex);
                }

                maxIndex--;
                length--;
            }

            return (-1);
        }

        private int InternalLastIndexOf(string value, int index, int length, bool ignoreCase)
        {
            int maxIndex = index + length - value.Length;

            while ((length > 0) && (maxIndex >= index))
            {
                if (this.InternalContains(value, maxIndex, length, ignoreCase))
                {
                    return (maxIndex);
                }

                maxIndex--;
                length--;
            }

            return (-1);
        }

        private int InternalLastIndexOf(StringBuffer value, int index, int length, bool ignoreCase)
        {
            int maxIndex = index + length - value.Length;

            while ((length > 0) && (maxIndex >= index))
            {
                if (this.InternalContains(value, maxIndex, length, ignoreCase))
                {
                    return (maxIndex);
                }

                maxIndex--;
                length--;
            }

            return (-1);
        }
                
        
        private bool InternalContains(char value, int index, bool ignoreCase)
        {
            var chunk = this.FindChunkForIndex(index);
            int local = index - chunk._offset;

            if (local >= chunk._length)
                return (false);

            if (ignoreCase)
            {
                if (char.ToLower(value) == char.ToLower(chunk._chars[local])) 
                    return (true);
            }
            else
            {
                if (value == chunk._chars[local])
                    return (true);
            }
                
            return (false);
        }
        
        private bool InternalContains(string value, int index, int count, bool ignoreCase)
        {
            var chunk = this.FindChunkForIndex(index);
                        
            for (int i = 0, j = index - chunk._offset; i < value.Length; i++)
            {
                if (chunk == null)
                    return (false);

                if ((count == 0) || (j >= chunk._length))
                    return (false);

                if (ignoreCase)
                {
                    if (char.ToLower(value[i]) != char.ToLower(chunk._chars[j]))
                        return (false);
                }
                else
                {
                    if (value[i] != chunk._chars[j])
                        return (false);
                }

                count--;
                j++;

                if (j >= chunk._length)
                {
                    chunk = chunk._next;
                    j     = 0;
                }
            }                
            
            return (true);            
        }

        private bool InternalContains(StringBuffer value, int index, int count, bool ignoreCase)
        {
            var chunk = this.FindChunkForIndex(index);
            
            for (int i = 0, j = index - chunk._offset; i < value.Length; i++)
            {
                if (chunk == null)
                    return (false);

                if ((count == 0) || (j >= chunk._length))
                    return (false);

                if (ignoreCase)
                {
                    if (char.ToLower(value[i]) != char.ToLower(chunk._chars[j]))
                        return (false);
                }
                else
                {
                    if (value[i] != chunk._chars[j])
                        return (false);
                }

                count--;
                j++;

                if (j >= chunk._length)
                {
                    chunk = chunk._next;
                    j = 0;
                }
            }

            return (true);
        }
        

        private bool ContainsAnyWhitespace(int index)
        {
            var chunk = this.FindChunkForIndex(index);
            int local = index - chunk._offset;

            if (local >= chunk._length)
                return (false);

            char c = chunk._chars[local];

            for (int i = 0; i < StringBuffer.Whitespace.Length; i++)
            {
                if (StringBuffer.Whitespace[i] == c)
                    return (true);
            }

            return (false);
        }

        private void InternalTrim(char[] value, bool ignoreCase, bool start, bool end)
        {
            int startIndex = 0;

            if ((start) && (this.Length != 0))
            {
                for (int i = 0; i < value.Length; ++i)
                {
                    char c = value[i];
                    while ((startIndex < this.Length) && this.InternalContains(c, startIndex, ignoreCase))
                    {
                        startIndex++;
                        i = 0;
                    }
                }
            }

            int endIndex = this.Length - 1;

            if (end && (endIndex >= 0))
            {
                for (int i = 0; i < value.Length; ++i)
                {
                    char c = value[i];
                    while ((endIndex >= 0) && this.InternalContains(c, endIndex, ignoreCase))
                    {
                        endIndex--;
                        i = 0;
                    }
                }
            }

            int length = ++endIndex - startIndex;

            if ((length >= 0) && ((startIndex + length) <= this.Length))
                this.InternalCrop(startIndex, length);
        }
                
        private void InternalFormat(object[] args, IFormatProvider provider, int index, int length)
        {
            if (args.Length == 0)
                return;

            object[] source = Formatter.ClassifyObjects(args);

            var expression = new StringBuffer();
            var current    = '\0';
            var state      = FormatState.OutsideExpression;
            int totLength  = index + length;
            int openCount  = 0;
            int closeCount = 0;
            
            StringNode chunk;
            int local;

            do
            {
                chunk = this.FindChunkForIndex(index);
                local = index - chunk._offset;

                if (local > chunk._length)
                    break;

                current = chunk._chars[local];

                switch (state)
                {
                    case FormatState.OutsideExpression:
                        switch (current)
                        {
                            case '{':
                                // remove open bracket
                                totLength--;
                                this.InternalShrink(index--, 1);
                                openCount++;
                                state = FormatState.OpenBracket;
                                break;
                            case '}':
                                state = FormatState.CloseBracket;
                                break;
                            default:
                                if (openCount > 0)
                                {
                                    expression.InternalAppend(current);
                                    state = FormatState.InsideExpression;
                                }
                                break;
                        }
                        break;

                    case FormatState.OpenBracket:
                        switch (current)
                        {
                            case '{':
                                openCount++;
                                state = FormatState.OutsideExpression;
                                break;
                            default:
                                expression.InternalAppend(current);
                                state = FormatState.InsideExpression;
                                break;
                        }
                        break;

                    case FormatState.InsideExpression:
                        switch (current)
                        {
                            case '{':
                                state = FormatState.OpenBracket;
                                break;
                            case '}':
                                // remove close bracket
                                totLength--;
                                this.InternalShrink(index--, 1);
                                closeCount++;
                                state = FormatState.CloseBracket;
                                break;

                            default:
                                if (closeCount == 0)
                                    expression.InternalAppend(current);
                                else
                                {
                                    if (openCount != closeCount)
                                        throw new FormatException("Input string was not in a correct format.");

                                    expression.InternalClear(DEFAULT_CAPACITY);
                                    openCount = 0;
                                    closeCount = 0;
                                    state = FormatState.OutsideExpression;
                                }
                                break;
                        }
                        break;

                    case FormatState.CloseBracket:
                        switch (current)
                        {
                            case '}':
                                closeCount++;
                                state = FormatState.InsideExpression;
                                break;
                            default:
                                if (closeCount > 0)
                                {
                                    if (openCount != closeCount)
                                    {
                                        if (((openCount % 2) != 1) || ((closeCount % 2) != 1))
                                            throw new FormatException("Input string was not in a correct format.");
                                    }

                                    string value = Formatter.Eval(source, expression, provider);
                                   
                                    // this is not the usual way, could be removed.
                                    if (value.Length == 0)
                                    { // empty string was submitted or returned, normalize the space.
                                        totLength--;
                                        this.InternalShrink(index, 1);
                                    }

                                    this.InternalReplace(value, (index - (expression.Length + (closeCount >> 1))), expression.Length);
                                    index += (value.Length - expression.Length);
                                    totLength += (value.Length - expression.Length);

                                    expression.InternalClear(DEFAULT_CAPACITY);
                                    openCount = 0;
                                    closeCount = 0;
                                }
                                state = FormatState.OutsideExpression;
                                break;
                        }
                        break;

                    default:
                        throw new FormatException();
                }

                index++;                
            }            
            while (index <= totLength);
        }

        private void ToLowerCase(CultureInfo culture)
        {
            var chunk = this._chunkHead;

            while (chunk != null)
            {
                for (int i = 0; i < chunk._length; ++i)
                {                    
                    this.InternalReplace(char.ToLower(chunk._chars[i], culture), i, 1);
                }

                chunk = chunk._next;
            }
        }

        private void ToUpperCase(CultureInfo culture)
        {
            var chunk = this._chunkHead;

            while (chunk != null)
            {
                for (int i = 0; i < chunk._length; ++i)
                {
                    this.InternalReplace(char.ToUpper(chunk._chars[i], culture), i, 1);
                }

                chunk = chunk._next;
            }
        }

        
        private void InternalFindMultipleAndReplace(char oldValue, char newValue, int index, int length, bool ignoreCase)
        {            
            int endIndex = index + length - 1;

            while ((length > 0) && (endIndex >= index))
            {
                if (this.InternalContains(oldValue, endIndex, ignoreCase))
                    this.InternalReplace(newValue, endIndex, 1);

                endIndex--;
                length--;                
            }
        }

        private void InternalFindMultipleAndReplace(char oldValue, string newValue, int index, int length, bool ignoreCase)
        {
            int endIndex = index + length - 1;

            while ((length > 0) && (endIndex >= index))
            {
                if (this.InternalContains(oldValue, endIndex, ignoreCase))
                    this.InternalReplace(newValue, endIndex, 1);

                endIndex--;
                length--;
            }
        }
       
        private void InternalFindMultipleAndReplace(string oldValue, char newValue, int index, int length, bool ignoreCase)
        {
            int oldLength = oldValue.Length;
            int endIndex  = index + length - oldLength;

            while ((length > 0) && (endIndex >= index))
            {
                if (this.InternalContains(oldValue, endIndex, length, ignoreCase))
                {
                    this.InternalReplace(newValue, endIndex, oldLength);

                    endIndex -= oldLength;
                    length   -= oldLength;
                }
                else
                {
                    endIndex--;
                    length--;
                }
            }
        }
        
        private void InternalFindMultipleAndReplace(string oldValue, string newValue, int index, int length, bool ignoreCase)
        {
            int oldLength = oldValue.Length;
            int endIndex = index + length - oldLength;

            while ((length > 0) && (endIndex >= index))
            {
                if (this.InternalContains(oldValue, endIndex, length, ignoreCase))
                {
                    this.InternalReplace(newValue, endIndex, oldLength);

                    endIndex -= oldLength;
                    length   -= oldLength;
                }
                else
                {
                    endIndex--;
                    length--;
                }
            }
        }
        
       
        private void InternalReplace(char value, int index, int length)
        {
            if ((length - 1) > 0)
            {// oldLength is greater, shrink at index with diff length and assign
                this.InternalShrink(index + 1, length - 1);
            }
            
            this.InternalAssign(index, value);
        }
        
        private void InternalReplace(string value, int index, int length)
        {
            if (value.Length == 0)
            {
                this.InternalShrink(index, length);
            }
            else
            {
                if ((length - value.Length) > 0)
                { // oldLength is greater, shrink at index with diff length and assign
                    this.InternalShrink(index + value.Length, length - value.Length);                    
                }
                else if ((length - value.Length) < 0)
                { // newLength is greater, expand at index with diff length and assign
                    this.InternalExpand(index + length, value.Length - length);                    
                }
                
                this.InternalAssign(index, value, 0, value.Length);
            }
        }
       

        private void InternalCrop(int index)
        {            
            this.InternalCrop(index, this.Length - index);
        }
        
        private void InternalCrop(int index, int length)
        {
            // Keep the substring at the specified index with the specified length, remove the remaining text.
                        
            if ((this.Length == 0) || ((index == 0) && (length == this.Length)))
                return;

            if (length == 0)
            {
                this.InternalClear();
                return;
            }

            // Remove from index + length to the end
            this.InternalShrink(index + length);

            if (index != 0)
            {
                // Remove from start to index
                this.InternalShrink(0, index);
            }
        }


        // { Borrowed from the mono StringBuilder Split()
        private int Delimit(char[] chrDelimiter, out int[] idxDelimiter)
        {
            idxDelimiter = new int[this.Length];            
            int foundCount = 0;

            if ((chrDelimiter == null) || (chrDelimiter.Length == 0))
            {
                for (int i = 0; ((i < this.Length) && (foundCount < idxDelimiter.Length)); i++)
                {
                    if (char.IsWhiteSpace(this[i]))
                    {
                        idxDelimiter[foundCount++] = i;                        
                    }
                }
            }
            else
            {
                int sepChrCount = chrDelimiter.Length;
                int sepIdxCount = idxDelimiter.Length;

                for (int i = 0; ((i < this.Length) && (foundCount < sepIdxCount)); i++)
                {
                    for (int j = 0; j < sepChrCount; j++)
                    {
                        if (this[i] == chrDelimiter[j])
                        {
                            idxDelimiter[foundCount++] = i;                           
                            break;
                        }
                    }
                }
            }

            return (foundCount);
        }

        private int Delimit(string[] strDelimiter, out int[] idxDelimiter, out int[] lengthList)
        {
            idxDelimiter = new int[this.Length];
            lengthList   = new int[this.Length];

            int foundCount  = 0;
            int sepIdxCount = this.Length;
            int sepStrCount = strDelimiter.Length;
            string    strSB = this.ToString();

            for (int i = 0; ((i < this.Length) && (foundCount < sepIdxCount)); i++)
            {
                for (int j = 0; j < sepStrCount; j++)
                {
                    string separator = strDelimiter[j];

                    if (string.IsNullOrEmpty(separator))
                    {
                        continue;
                    }

                    int currSepLength = separator.Length;

                    if ((this[i] == separator[0]) && (currSepLength <= (this.Length - i)))
                    {
                        if ((currSepLength == 1) || (string.CompareOrdinal(strSB, i, separator, 0, currSepLength) == 0))
                        {
                            idxDelimiter[foundCount] = i;
                            lengthList[foundCount] = currSepLength;

                            foundCount++;
                            i += (currSepLength - 1);
                            break;
                        }
                    }
                }
            }

            return (foundCount);
        }
        
        
        private string[] SplitKeepEmptyEntries(ref int[] lengthList, int[] idxDelimiter, int replaceCount, int maxString)
        {
            int currIndex = 0;
            int arrIndex = 0;

            maxString--;
            int currReplace = (replaceCount < maxString) ? replaceCount : maxString;

            // Allocate space for the new array. +1 for the string from the end of the last replace to the end of the String.
            string[] splitStrings = new string[currReplace + 1];

            for (int i = 0; ((i < currReplace) && (currIndex < this.Length)); i++)
            {
                splitStrings[arrIndex++] = this.ToString(currIndex, idxDelimiter[i] - currIndex);
                currIndex = idxDelimiter[i] + ((lengthList == null) ? 1 : lengthList[i]);
            }

            // Handle the last string at the end of the array if there is one.
            if ((currIndex < this.Length) && (currReplace >= 0))
            {
                splitStrings[arrIndex] = this.ToString(currIndex);
            }
            else if (arrIndex == currReplace)
            {
                // We had a separator character at the end of a string. Rather than just allowing
                // a null character, we'll replace the last element in the array with an empty string.
                splitStrings[arrIndex] = string.Empty;
            }

            return (splitStrings);
        }

        private string[] SplitOmitEmptyEntries(ref int[] lengthList, int[] idxDelimiter, int replaceCount, int maxString)
        {
            int currIndex = 0;
            int arrIndex = 0;

            int currReplace = (replaceCount < maxString) ? (replaceCount + 1) : maxString;

            // Allocate space for the new array.
            string[] splitStrings = new string[currReplace];

            for (int i = 0; ((i < replaceCount) && (currIndex < this.Length)); i++)
            {
                if (idxDelimiter[i] - currIndex > 0)
                {
                    splitStrings[arrIndex++] = this.ToString(currIndex, idxDelimiter[i] - currIndex);
                }

                currIndex = idxDelimiter[i] + ((lengthList == null) ? 1 : lengthList[i]);

                if (arrIndex == (maxString - 1))
                {
                    // If all the remaining entries at the end are empty, skip them
                    while ((i < replaceCount - 1) && (currIndex == idxDelimiter[++i]))
                    {
                        currIndex += ((lengthList == null) ? 1 : lengthList[i]);
                    }

                    break;
                }
            }

            // Handle the last string at the end of the array if there is one.
            if (currIndex < this.Length)
            {
                splitStrings[arrIndex++] = this.ToString(currIndex);
            }

            string[] stringArray = splitStrings;

            if (arrIndex != currReplace)
            {
                stringArray = new string[arrIndex];

                for (int j = 0; j < arrIndex; j++)
                {
                    stringArray[j] = splitStrings[j];
                }
            }

            return (stringArray);
        }


        private StringBuffer[] SplitBufferKeepEmptyEntries(ref int[] lengthList, int[] idxDelimiter, int replaceCount, int maxString)
        {
            int currIndex = 0;
            int arrIndex  = 0;

            maxString--;
            int currReplace = (replaceCount < maxString) ? replaceCount : maxString;

            // Allocate space for the new array. +1 for the string from the end of the last replace to the end of the String.            
            StringBuffer[] splitStrings = new StringBuffer[currReplace + 1];

            for (int i = 0; ((i < currReplace) && (currIndex < this.Length)); i++)
            {                
                splitStrings[arrIndex++] = this.Copy(currIndex, idxDelimiter[i] - currIndex);
                currIndex = idxDelimiter[i] + ((lengthList == null) ? 1 : lengthList[i]);
            }

            // Handle the last string at the end of the array if there is one.
            if ((currIndex < this.Length) && (currReplace >= 0))
            {                
                splitStrings[arrIndex] = this.Copy(currIndex);
            }
            else if (arrIndex == currReplace)
            {
                // We had a separator character at the end of a string. Rather than just allowing
                // a null character, we'll replace the last element in the array with an empty string.
                splitStrings[arrIndex] = StringBuffer.Empty;
            }

            return (splitStrings);
        }

        private StringBuffer[] SplitBufferOmitEmptyEntries(ref int[] lengthList, int[] idxDelimiter, int replaceCount, int maxString)
        {
            int currIndex = 0;
            int arrIndex  = 0;

            int currReplace = (replaceCount < maxString) ? (replaceCount + 1) : maxString;

            // Allocate space for the new array.            
            StringBuffer[] splitStrings = new StringBuffer[currReplace];

            for (int i = 0; ((i < replaceCount) && (currIndex < this.Length)); i++)
            {
                if (idxDelimiter[i] - currIndex > 0)
                {                    
                    splitStrings[arrIndex++] = this.Copy(currIndex, idxDelimiter[i] - currIndex);
                }

                currIndex = idxDelimiter[i] + ((lengthList == null) ? 1 : lengthList[i]);

                if (arrIndex == (maxString - 1))
                {
                    // If all the remaining entries at the end are empty, skip them
                    while ((i < replaceCount - 1) && (currIndex == idxDelimiter[++i]))
                    {
                        currIndex += ((lengthList == null) ? 1 : lengthList[i]);
                    }

                    break;
                }
            }

            // Handle the last string at the end of the array if there is one.
            if (currIndex < this.Length)
            {                
                splitStrings[arrIndex++] = this.Copy(currIndex);
            }

            StringBuffer[] stringArray = splitStrings;

            if (arrIndex != currReplace)
            {
                stringArray = new StringBuffer[arrIndex];

                for (int j = 0; j < arrIndex; j++)
                {
                    stringArray[j] = splitStrings[j];
                }
            }

            return (stringArray);
        }
        // }


        #region "CORE METHODS"
        
        /// <summary>Appends a character. Invalidate the cached string.</summary>    
        private void InternalAppend(char value)
        {
            if ((this._maxCapacity - this.Length) == 0)
            {
                throw new ArgumentOutOfRangeException("MaxCapacity");
            }

            var lastChunk = this._chunkTail;

            if ((MAX_CHUNK_SIZE - lastChunk._length) > 0)
            {
                int capacity, newLength;
                capacity = this.GetChunkCapacity(lastChunk, 1, out newLength);

                if (this._chunkTail._chars.Length != capacity)
                {
                    Array.Resize(ref lastChunk._chars, capacity);
                }

                lastChunk._chars[lastChunk._length] = value;
                lastChunk._length += newLength;

                this._chunkTail = lastChunk;
            }
            else
            {
                var newChunk = StringNode.CreateInstance(lastChunk._offset + lastChunk._length, 0);
                lastChunk._next = newChunk;
                this._chunkTail = newChunk;

                int capacity, newLength;
                capacity = this.GetChunkCapacity(newChunk, 1, out newLength);

                newChunk._chars = new char[capacity];
                newChunk._chars[newChunk._length] = value;
                newChunk._length = newLength;
            }

            this._cachedStr = null;
        }

        /// <summary>Appends an array of null characters. Invalidate the cached string.</summary>
        private void InternalAppend(int length)
        {
            char[] value = new char[length];

            for (int i = 0; i < length; ++i)
            {
                value[i] = '\0';
            }

            this.InternalAppend(value, 0, length);
        }
        
        /// <summary>Appends an array of characters. Invalidate the cached string.</summary>
        private void InternalAppend(char[] value, int length)
        {
            this.InternalAppend(value, 0, length);
        }
        
        /// <summary>Appends a string. Invalidate the cached string.</summary>
        private void InternalAppend(string value, int startIndex, int length)
        {
            this.InternalAppend(value.ToCharArray(), startIndex, length);
        }
                
        /// <summary>Appends an array of characters. Invalidate the cached string.</summary>
        private void InternalAppend(char[] value, int startIndex, int length)
        {
            // - First it complete the last chunk, up to the MAX_CHUNK_SIZE.
            // - Second, if it remain's text and the length is bigger than MAX_CHUNK_SIZE
            //   it create new complete chunk whose length are equal to MAX_CHUNK_SIZE
            //   until the length was lesser than MAX_CHUNK_SIZE or equal to zero.
            // - Third, if it remain's text, the length is then lesser than MAX_CHUNK_SIZE.
            //   so it create new chunk, whose length is based on the capacity behavior (see: GetChunkCapacity()).

            if ((this._maxCapacity - this.Length) < length)
            {
                throw new ArgumentOutOfRangeException("MaxCapacity");
            }

            var lastChunk = this._chunkTail;

            // 1 - Complement, fill the current chunk up to the MAX_CHUNK_SIZE.
            if ((MAX_CHUNK_SIZE - lastChunk._length) > 0)
            {
                int capacity, newLength;
                capacity = this.GetChunkCapacity(lastChunk, length, out newLength);

                if (lastChunk._chars.Length != capacity)
                {
                    Array.Resize(ref lastChunk._chars, capacity);
                }
                                
                Array.Copy(value, startIndex, lastChunk._chars, lastChunk._length, newLength);

                lastChunk._length += newLength;
                startIndex += newLength;
                length -= newLength;

                this._chunkTail = lastChunk;
            }

            // 2 - Full chunk, create and fill only 'complete' chunk.
            if ((length >= MAX_CHUNK_SIZE) && (this.Length < this._maxCapacity))
            {
                int repeatCnt = length / MAX_CHUNK_SIZE;
                int capacity = MAX_CHUNK_SIZE;
                int newLength = MAX_CHUNK_SIZE;

                while ((repeatCnt > 0) && (this.Length < this._maxCapacity))
                {
                    var newChunk = StringNode.CreateInstance(lastChunk._offset + lastChunk._length, 0, MAX_CHUNK_SIZE, null);
                    lastChunk._next = newChunk;
                    this._chunkTail = newChunk;

                    if ((newChunk._offset + capacity) > this._maxCapacity)
                    {
                        capacity = this._maxCapacity - newChunk._offset;
                        newLength = this._maxCapacity - this.Length;
                    }

                    if (newChunk._chars.Length != capacity)
                    {
                        Array.Resize(ref newChunk._chars, capacity);
                    }

                    Array.Copy(value, startIndex, newChunk._chars, newChunk._length, newLength);

                    newChunk._length = newLength;
                    startIndex += newLength;
                    length -= newLength;

                    lastChunk = newChunk;
                    repeatCnt--;
                }
            }

            // 3 - Remains, create new 'incomplete' chunk.
            if ((length > 0) && (this.Length < this._maxCapacity))
            {
                var newChunk = StringNode.CreateInstance(lastChunk._offset + lastChunk._length, 0);
                lastChunk._next = newChunk;
                this._chunkTail = newChunk;

                int capacity, newLength;
                capacity = this.GetChunkCapacity(newChunk, length, out newLength);

                newChunk._chars = new char[capacity];

                Array.Copy(value, startIndex, newChunk._chars, newChunk._length, newLength);

                newChunk._length = newLength;
            }

            this._cachedStr = null;
        }
        

        /// <summary>Assign a character. Invalidate the cached string.</summary>
        private void InternalAssign(int index, char value)
        {
            var chunk = this.FindChunkForIndex(index);
            int local = index - chunk._offset;

            if (local >= chunk._length)
            {
                throw new ArgumentOutOfRangeException("localIndex");
            }

            chunk._chars[local] = value;            

            this._cachedStr = null;
        }
        
        /// <summary>Assign a string. Invalidate the cached string.</summary>
        private void InternalAssign(int index, string value, int startIndex, int length)
        {
            this.InternalAssign(index, value.ToCharArray(), startIndex, length);            
        }
        
        /// <summary>Assign an array of characters. Invalidate the cached string.</summary>
        private void InternalAssign(int index, char[] value, int startIndex, int length)
        {
            var chunk = this.FindChunkForIndex(index);
            int local = index - chunk._offset;

            if (local >= chunk._length)
            {
                throw new ArgumentOutOfRangeException("localIndex");
            }

            while ((length > 0) && (chunk != null))
            {
                int len = Math.Min(chunk._chars.Length - local, length);

                Array.Copy(value, startIndex, chunk._chars, local, len);

                startIndex += len;
                local += len;
                length -= len;

                if (local >= chunk._chars.Length)
                {
                    local = 0;
                    chunk = chunk._next;
                }
            }

            this._cachedStr = null;
        }


        /// <summary>Shrink: remove at index until the end. Invalidate the cached string.</summary>
        private void InternalShrink(int index)
        {
            this.InternalShrink(index, this.Length - index);
        }

        /// <summary>Shrink: remove at index with the specified length. Invalidate the cached string.</summary>
        private void InternalShrink(int index, int length)
        {
            // Check if it could be cleared.
            if ((index == 0) && (length == this.Length))
            {
                this.InternalClear();
                return;
            }

            // Check if it could be cropped instead, way faster.
            if ((index + length) == this.Length)
            {
                // Simply remove the end.
                var lastBuffer  = this._chunkTail;
                var firstBuffer = this.FindChunkForIndex(index);

                if (lastBuffer != firstBuffer)
                {
                    // Remove full chunk until the lastbuffer(chunk tail) equal the firstbuffer.
                    while (lastBuffer != firstBuffer)
                    {
                        var previous     = this.GetPreviousChunk(lastBuffer);
                        previous._next  = null;
                        this._chunkTail = previous;

                        lastBuffer._offset = 0;
                        lastBuffer._length = 0;
                        lastBuffer._chars = null;

                        lastBuffer = previous;
                    }
                }

                int minLocalIndex = index - lastBuffer._offset;

                if ((minLocalIndex == 0) && (lastBuffer != this._chunkHead))
                {
                    // In case the minLocalIndex of the firstBuffer is equal to zero, remove the chunk, except if it's the head.
                    var previous = this.GetPreviousChunk(lastBuffer);
                    previous._next = null;
                    this._chunkTail = previous;

                    lastBuffer._offset = 0;
                    lastBuffer._length = 0;
                    lastBuffer._chars = null;
                }
                else
                {
                    // Clean the remaining chars.
                    int len = lastBuffer._length - minLocalIndex;
                    lastBuffer._length = minLocalIndex;

                    if (len > 0)
                    {
                        char[] chars = new char[len];                                                
                        for (int i = 0; i < len; ++i) { chars[i] = '\0'; }
                        Array.Copy(chars, 0, lastBuffer._chars, minLocalIndex, len);
                    }
                }
            }
            else
            { // the usual way
                // From start, walk to the right and shift the text to the left, up to the end, then remove the last part.

                int maxGlobalIndex = index + length;
                var srceChunk      = this.FindChunkForIndex(maxGlobalIndex);
                int maxLocalIndex  = maxGlobalIndex - srceChunk._offset;

                int minGlobalIndex = index;
                var destChunk      = this.FindChunkForIndex(minGlobalIndex);
                int minLocalIndex  = minGlobalIndex - destChunk._offset;

                int destLocalIndex = minLocalIndex;
                int srceLocalIndex = maxLocalIndex;

                int count     = index + length;
                int newLength = this.Length;

                while ((srceChunk != null) && (destChunk != null) && (count < newLength))
                {
                    while ((minLocalIndex < destChunk._length) && (maxLocalIndex < srceChunk._length) && (count < newLength))
                    {
                        count++;
                        minLocalIndex++;
                        maxLocalIndex++;
                    }

                    int copyCount = Math.Min(maxLocalIndex - srceLocalIndex, minLocalIndex - destLocalIndex);

                    // Shift the text to the left.
                    Array.Copy(srceChunk._chars, srceLocalIndex, destChunk._chars, destLocalIndex, copyCount);

                    // Update index.
                    destLocalIndex = minLocalIndex;
                    srceLocalIndex = maxLocalIndex;

                    // Check limit, and assign zero to local index.            
                    if ((srceLocalIndex == srceChunk._length))
                    {
                        srceChunk = srceChunk._next;
                        maxLocalIndex = srceLocalIndex = 0;
                    }

                    if ((destLocalIndex == destChunk._length) || (count == newLength))
                    {
                        destChunk._length = minLocalIndex;
                        destChunk = destChunk._next;
                        minLocalIndex = destLocalIndex = 0;
                    }
                    //
                }

                // Clean and re-tail
                var chunk01 = this.FindChunkForIndex((newLength - length - 1));
                int len     = chunk01._chars.Length - chunk01._length;

                if (len > 0)
                {                    
                    char[] chars = new char[len];
                    for (int i = 0; i < len; ++i) { chars[i] = '\0'; }
                    Array.Copy(chars, 0, chunk01._chars, chunk01._length, len);
                }

                var chunk02 = chunk01._next;
                chunk01._next   = null;
                this._chunkTail = chunk01;

                while (chunk02 != null)
                {
                    chunk02._offset = 0;
                    chunk02._length = 0;
                    chunk02._chars  = null;
                    chunk01          = chunk02._next;
                    chunk02._next   = null;
                    chunk02          = chunk01;
                    chunk01          = null;
                }
                //
            }

            this._cachedStr = null;
        }

        /// <summary>Expand: make the necessary space at index for insertion. Invalidate the cached string.</summary>
        private void InternalExpand(int index, int length)
        {
            // Make room for insertion at the end of the StringBuffer.
            // From the end, walk to the left and shift the text to the right up to the insertion index.
            // There isn't any text insertion here, we make just room for assignement, 'InternalAssign' is in in charge to.

            int oldLength     = this.Length;
            var srceChunk     = this.FindChunkForIndex(oldLength);
            int minLocalIndex = oldLength - srceChunk._offset;

            // Temp array appended to StringBuffer to make room necessary to shift the text to the right in respect of the chunk capacity.             
            this.InternalAppend(length);
            
            var destChunk = this.FindChunkForIndex(this.Length);
            int maxLocalIndex = this.Length - destChunk._offset;

            int srceLocalIndex = minLocalIndex;
            int destLocalIndex = maxLocalIndex;

            // From the end, we walk to the left, chunk by chunk up to the index insertion, and shift the text to the right.        
            while (true)
            {
                // Search for the minimum length to copy without 'cross copy error', up to the insertion index.
                // we take the minimum length available from the index to the capacity between the two chunk (could be the same or not).
                while ((srceLocalIndex > 0) && (destLocalIndex > 0) && (oldLength > index))
                {
                    oldLength--;
                    srceLocalIndex--;
                    destLocalIndex--;
                }

                int copyCount = Math.Min(minLocalIndex - srceLocalIndex, maxLocalIndex - destLocalIndex);

                // Shift the text to the right
                Array.Copy(srceChunk._chars, srceLocalIndex, destChunk._chars, destLocalIndex, copyCount);

                // Reached the insertion index, go away.
                if (oldLength == index)
                {
                    break;
                }

                // Check limit, and assign previous chunk capacity to local index.
                if (srceLocalIndex == 0)
                {
                    srceChunk = this.GetPreviousChunk(srceChunk);

                    if (srceChunk == null)
                    {
                        break;
                    }

                    minLocalIndex = srceLocalIndex = srceChunk._chars.Length;
                }

                if (destLocalIndex == 0)
                {
                    destChunk = this.GetPreviousChunk(destChunk);

                    if (destChunk == null)
                    {
                        break;
                    }

                    maxLocalIndex = destLocalIndex = destChunk._chars.Length;
                }
                //
            }

            this._cachedStr = null;
        }


        /// <summary>Clear the text and set to default capacity. Invalidate the cached string.</summary>
        private void InternalClear()
        {
            this.InternalClear(DEFAULT_CAPACITY);
        }

        /// <summary>Clear the text and set the specified capacity. Invalidate the cached string.</summary>
        private void InternalClear(int capacity)
        {
            this._chunkTail = null;
            var chunk01 = this._chunkHead;
            var chunk02 = (StringNode)null;

            while (chunk01 != null)
            {
                chunk01._offset = 0;
                chunk01._length = 0;
                chunk01._chars = null;
                chunk02 = chunk01._next;
                chunk01._next = null;
                chunk01 = chunk02;
                chunk02 = null;
            }

            if (capacity < DEFAULT_CAPACITY)
            {
                capacity = DEFAULT_CAPACITY;
            }
            else if (capacity > MAX_CHUNK_SIZE)
            {
                capacity = MAX_CHUNK_SIZE;
            }

            this._chunkHead = StringNode.CreateInstance(0, 0, capacity, null);
            this._chunkTail = this._chunkHead;

            this._cachedStr = null;
        }
                
        #endregion


        #region "CHUNK"

        /// <summary>Chunk.</summary>
        private class StringNode
        {
            public int        _offset;
            public int        _length;
            public char[]     _chars;
            public StringNode _next;

            public static StringNode CreateInstance(int offset, int length)
            {
                StringNode sn = new StringNode();
                sn._offset = offset;
                sn._length = length;
                sn._chars  = null;
                sn._next   = null;

                return (sn);
            }

            public static StringNode CreateInstance(int offset, int length, int capacity, StringNode next)
            {
                StringNode sn = new StringNode();
                sn._offset = offset;
                sn._length = length;
                sn._chars  = new char[capacity];
                sn._next   = next;

                return (sn);
            }
        }
        
        /// <summary>.</summary>
        private int GetChunkCapacity(StringNode chunk, int length, out int newLength)
        {
            int capacity = DEFAULT_CAPACITY;
            newLength = length;

            while (length > (capacity - chunk._length))
            {
                // Here the capacity behavior, it simply multiply by 2 until it cover the length. Could be improved.
                capacity <<= 1;

                // We ensure, we don't get capacity greater than MAX_CHUNK_SIZE and the StringBuffer text doesn't exceed the MaxCapacity.
                if ((capacity > MAX_CHUNK_SIZE) || ((chunk._offset + capacity) > this._maxCapacity))
                {
                    capacity  = Math.Min(MAX_CHUNK_SIZE, this._maxCapacity - chunk._offset);
                    newLength = Math.Min(Math.Min(MAX_CHUNK_SIZE - chunk._length, length), (this._maxCapacity - this.Length));
                    break;
                }
            }

            return (capacity);
        }

        /// <summary>Find chunk from global index.
        /// <para/>If out of range doesn't return null, instead it return the last chunk.</summary>
        private StringNode FindChunkForIndex(int index)
        {
            var chunk = this._chunkHead;

            while (chunk != null)
            {
                if ((index >= chunk._offset) && (index < (chunk._offset + chunk._length)))
                {
                    return (chunk);
                }

                chunk = chunk._next;
            }
            
            return (this._chunkTail);
        }

        /// <summary>Get the previous chunk</summary>
        private StringNode GetPreviousChunk(StringNode chunk)
        {
            if (chunk == this._chunkHead)
            {
                return (null);
            }

            return (this.FindChunkForIndex(chunk._offset - 1));
        }
                
        #endregion

        #endregion


        #region "PUBLIC STATIC"

        public static bool IsNullOrEmpty(StringBuffer sb)
        {
            if (sb == null)
            {
                return (true);
            }

            if (sb.IsEmpty)
            {
                return (true);
            }

            return (false);
        }

        public static bool IsNullOrWhitespace(StringBuffer sb)
        {
            if (sb == null)
            {
                return (true);
            }

            if (sb.IsEmpty)
            {
                return (true);
            }
            
            foreach (char c in sb)
            {
                if (!char.IsWhiteSpace(c))
                {
                    return (false);
                }
            }
            
            return (true);
        }


        public static int Compare(StringBuffer sbA, StringBuffer sbB)
        {
            return (CultureInfo.CurrentCulture.CompareInfo.Compare(sbA.ToString(), sbB.ToString(), CompareOptions.None));            
        }

        public static int Compare(StringBuffer sbA, string strB)
        {
            return (CultureInfo.CurrentCulture.CompareInfo.Compare(sbA.ToString(), strB, CompareOptions.None));
        }


        public static int Compare(StringBuffer sbA, StringBuffer sbB, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return (CultureInfo.CurrentCulture.CompareInfo.Compare(sbA.ToString(), sbB.ToString(), CompareOptions.IgnoreCase));
            }

            return (CultureInfo.CurrentCulture.CompareInfo.Compare(sbA.ToString(), sbB.ToString(), CompareOptions.None));            
        }

        public static int Compare(StringBuffer sbA, string strB, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return (CultureInfo.CurrentCulture.CompareInfo.Compare(sbA.ToString(), strB, CompareOptions.IgnoreCase));
            }

            return (CultureInfo.CurrentCulture.CompareInfo.Compare(sbA.ToString(), strB, CompareOptions.None));
        }


        public static int Compare(StringBuffer sbA, StringBuffer sbB, bool ignoreCase, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            return (culture.CompareInfo.Compare(sbA.ToString(), sbB.ToString()));           
        }

        public static int Compare(StringBuffer sbA, string strB, bool ignoreCase, CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            return (culture.CompareInfo.Compare(sbA.ToString(), strB));
        }


        public static int Compare(StringBuffer sbA, StringBuffer sbB, StringComparison comparisonType)
        {
            return (string.Compare(sbA.ToString(), sbB.ToString(), comparisonType));
        }

        public static int Compare(StringBuffer sbA, string strB, StringComparison comparisonType)
        {
            return (string.Compare(sbA.ToString(), strB, comparisonType));
        }


        public static bool Equals(StringBuffer sbA, StringBuffer sbB)
        {
            if ((Object)sbA == (Object)sbB)
            {
                return (true);
            }

            if (((Object)sbA == null) || ((Object)sbB == null))
            {
                return (false);
            }

            if (sbA.Length != sbB.Length)
            {
                return (false);
            }

            return (string.Equals(sbA.ToString(), sbB.ToString()));            
        }

        public static bool Equals(StringBuffer sbA, string strB)
        {            
            if (((Object)sbA == null) || ((Object)strB == null))
            {
                return (false);
            }

            if (sbA.Length != strB.Length)
            {
                return (false);
            }

            return (string.Equals(sbA.ToString(), strB));
        }


        public static bool Equals(StringBuffer sbA, StringBuffer sbB, StringComparison comparisonType)
        {
            return (StringBuffer.Compare(sbA, sbB, comparisonType) == 0);
        }

        public static bool Equals(StringBuffer sbA, string strB, StringComparison comparisonType)
        {
            return (StringBuffer.Compare(sbA, strB, comparisonType) == 0);
        }


        public static StringBuffer Empty
        {
            get { return (new StringBuffer("")); }
        }
        

        public static StringBuffer Format(StringBuffer buffer, params object[] args)
        {
            return (StringBuffer.Format(buffer, null, args));
        }

        public static StringBuffer Format(StringBuffer buffer, IFormatProvider provider, params object[] args)
        {
            if (StringBuffer.IsNullOrEmpty(buffer))
            {
                throw new ArgumentNullException();
            }
            
            var sb = buffer.Copy();
            sb.FormatWith(0, sb.Length, provider, args);

            return (sb);
        }

        public static char[] GetWhitespaces()
        {
            return (StringBuffer.Whitespace);
        }
        
        #endregion
                
    }
}
