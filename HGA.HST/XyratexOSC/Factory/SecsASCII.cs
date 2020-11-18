using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    /// <summary>
    /// Secs-II ASCII Encoded string
    /// </summary>
    public sealed class SecsASCII : ISecsValue, IComparable<SecsASCII>, IEnumerable<SecsASCIIChar>
    {
        public static readonly SecsASCII Empty = new SecsASCII(new byte[] { });

        private readonly Byte[] _data;
        private readonly string _format = "A";
        
        /// <summary>
        /// Gets the number of ASCII characters in the current ASCII string.
        /// </summary>
        /// <value>
        /// The ASCII string length.
        /// </value>
        public int Length
        {
            get
            {
                return _data.Length;
            }
        }

        /// <summary>
        /// Gets the ASCII <see cref="Byte"/> at the specified index.
        /// </summary>
        /// <value>
        /// The ASCII <see cref="Byte"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public SecsASCIIChar this[int index]
        {
            get
            {
                return (SecsASCIIChar)_data[index];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsASCII" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="shallow">if set to <c>true</c> performs a shallow copy.</param>
        private SecsASCII(Byte[] value, bool shallow)
        {
            if (shallow)
            {
                _data = value;
            }
            else
            {
                _data = new Byte[value.Length];
                Buffer.BlockCopy(value, 0, _data, 0, value.Length);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsASCII"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SecsASCII(Byte[] value)
            : this(value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsASCII"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex
        /// or
        /// length
        /// </exception>
        public SecsASCII(Byte[] value, int startIndex, int length)
        {
            if (value == null) 
                throw new ArgumentNullException("data");

            if (startIndex < 0 || startIndex > value.Length) 
                throw new ArgumentOutOfRangeException("startIndex");

            if (length < 0 || startIndex + length > value.Length)
                throw new ArgumentOutOfRangeException("length");
 
            _data = new byte[length];
            Buffer.BlockCopy(value, startIndex, _data, 0, length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecsASCII"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SecsASCII(string value)
        {
            _data = Encoding.ASCII.GetBytes(value);
        }

        /// <summary>
        /// Gets the .NET equivalent primitive type.
        /// </summary>
        /// <returns>The .NET primitive equivalent.</returns>
        public Type GetDotNetType()
        {
            return typeof(string);
        }

        /// <summary>
        /// Converts to an object as the .NET primitive type.
        /// </summary>
        /// <returns>The .NET primitive object.</returns>
        public object ToDotNet()
        {
            return (string)this;
        }

        /// <summary>
        /// Gets the SECS-II format string.
        /// </summary>
        /// <returns>The format name.</returns>
        public string GetSECSFormat()
        {
            return _format;
        }

        /// <summary>
        /// Gets the number of ASCII characters in the string.
        /// </summary>
        /// <returns>The length of the string.</returns>
        public int GetSECSLength()
        {
            return Length;
        }

        /// <summary>
        /// Returns a reference to this instance of <see cref="SecsASCII"/>.
        /// </summary>
        /// <returns></returns>
        public SecsASCII Clone()
        {
            return this;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">The value.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return CompareTo((SecsASCII)obj);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates 
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int CompareTo(SecsASCII value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return SecsASCII.Compare(this, value);
        }

        int IComparable<SecsASCII>.CompareTo(SecsASCII value)
        {
            return this.CompareTo(value);
        }

        /// <summary>
        /// Determines whether the specified ASCII string occurs within this object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        public bool Contains(SecsASCII value)
        {
            if (value == null) throw new ArgumentNullException("value");
            return this.IndexOf(value) >= 0;
        }

        public bool Contains(SecsASCIIChar value)
        {
            byte valueByte = value.ToByte();

            foreach (byte b in this._data)
            {
                if (b.Equals(valueByte))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified ASCII string occurs at the end of this string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the ASCII string ends with the specified string.</returns>
        public bool EndsWith(SecsASCII value)
        {
            return this.EndsWith(value, false);
        }

        /// <summary>
        /// Determines whether the specified ASCII string occurs at the end of this string, using case-matching if specified.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>
        ///   <c>true</c> if the ASCII string ends with the specified string.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public bool EndsWith(SecsASCII value, bool ignoreCase)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return SecsASCII.Compare(this, this._data.Length - value._data.Length, value, 0, value.Length, ignoreCase) == 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this <see cref="SecsASCII"/> instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this <see cref="SecsASCII"/> instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this <see cref="SecsASCII"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SecsASCII))
                return false;

            return _data.Equals((SecsASCII)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="SecsASCII" /> value is equal to this <see cref="SecsASCII"/> instance.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Equals(SecsASCII value)
        {
            return this.CompareTo(value) == 0;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<SecsASCIIChar> GetEnumerator()
        {
            foreach (byte b in _data)
                yield return new SecsASCIIChar(b);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _data.GetHashCode();
        }

        /// <summary>
        /// Determines the first index of the specified <see cref="SecsASCII"/> character occurrence.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <returns>The index of the first occurrence, or -1 if no match is found.</returns>
        public int IndexOf(SecsASCII value)
        {
            return this.IndexOf(value, 0, this.Length, false);
        }

        /// <summary>
        /// Determines the first index of the specified <see cref="SecsASCII" /> string occurrence, using case-matching as specified.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignores case-matching.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        public int IndexOf(SecsASCII value, bool ignoreCase)
        {
            return this.IndexOf(value, 0, this.Length, ignoreCase);
        }

        /// <summary>
        /// Determines the first index after the specified starting index of the specified <see cref="SecsASCII" /> string occurrence.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex is outside of range</exception>
        public int IndexOf(SecsASCII value, int startIndex)
        {
            return this.IndexOf(value, startIndex, this.Length - startIndex, false);
        }

        /// <summary>
        /// Determines the first index after the specified starting index of the specified <see cref="SecsASCII" /> string occurrence, using case-matching as specified.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex is outside of range</exception>
        public int IndexOf(SecsASCII value, int startIndex, bool ignoreCase)
        {
            return this.IndexOf(value, startIndex, this.Length - startIndex, ignoreCase);
        }


        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in this instance. The search starts at a specified character position and
        /// examines a specified number of character positions.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>The index of the first occurrence, or -1 if no match is found.</returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int IndexOf(SecsASCII value, int startIndex, int count)
        {
            return this.IndexOf(value, startIndex, count, false);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in this instance. The search starts at a specified character position and
        /// examines a specified number of character positions.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="count">The number of positions to examine.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>The index of the start of the occurrence, or -1 if no match is found.</returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int IndexOf(SecsASCII value, int startIndex, int count, bool ignoreCase)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            if (startIndex < 0 || startIndex > this._data.Length) 
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0 || startIndex + count > this._data.Length || count < value._data.Length) 
                throw new ArgumentOutOfRangeException("count");

            int charactersFound = 0;

            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (i + (value._data.Length - charactersFound) > this._data.Length) 
                    return -1;

                byte byteA = this._data[i];
                byte byteB = value._data[charactersFound];

                if (ignoreCase)
                {
                    byteA = SecsASCIIChar.ToLower(byteA);
                    byteB = SecsASCIIChar.ToLower(byteB);
                }

                if (byteA == byteB) 
                    charactersFound++;
                else 
                    charactersFound = 0;

                if (charactersFound == value._data.Length)
                    return (i - charactersFound + 1);
            }

            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters.
        /// </summary>
        /// <param name="values">The ASCII character array.</param>
        /// <returns>The index of the first occurrence, or -1 if no match is found.</returns>
        public int IndexOfAny(params SecsASCIIChar[] values)
        {
            return this.IndexOfAny(values, 0, this._data.Length, false);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters.
        /// </summary>
        /// <param name="values">The ASCII character enumeration.</param>
        /// <returns>The index of the first occurrence, or -1 if no match is found.</returns>
        public int IndexOfAny(IEnumerable<SecsASCIIChar> values)
        {
            return this.IndexOfAny(values, 0, this._data.Length, false);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters, using case-matching only if specified.
        /// </summary>
        /// <param name="values">The ASCII character enumeration.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, ignore case matching.</param>
        /// <returns>The index of the first occurrence, or -1 if no match is found.</returns>
        public int IndexOfAny(IEnumerable<SecsASCIIChar> values, bool ignoreCase)
        {
            return this.IndexOfAny(values, 0, this._data.Length, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters, starting the search at the specified index.
        /// </summary>
        /// <param name="values">The ASCII character enumeration.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        public int IndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex)
        {
            return this.IndexOfAny(values, startIndex, this._data.Length - startIndex, false);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters, starting the search at the specified index
        /// and using case-matching only if specified.
        /// </summary>
        /// <param name="values">The ASCII character enumeration.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">values is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count out of range</exception>
        public int IndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex, bool ignoreCase)
        {
            return this.IndexOfAny(values, startIndex, this._data.Length - startIndex, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters, searching the 
        /// specified number of characters starting at the specified index.
        /// </summary>
        /// <param name="values">The ASCII character enumeration.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="count">The number of character to evaluate.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">values is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count out of range </exception>
        public int IndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex, int count)
        {
            return this.IndexOfAny(values, startIndex, count, false);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of
        /// any character in a specified array of ASCII characters, searching the
        /// specified number of characters starting at the specified index, using case-matching as specified.
        /// </summary>
        /// <param name="values">The ASCII character enumeration.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="count">The number of character to evaluate.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>
        /// The index of the first occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">values is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count out of range </exception>
        public int IndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex, int count, bool ignoreCase)
        {
            if (values == null) 
                throw new ArgumentNullException("values");

            if (startIndex < 0 || startIndex > this._data.Length) 
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0 || startIndex + count > this._data.Length) 
                throw new ArgumentOutOfRangeException("count");

            List<byte> valueBytes = new List<byte>();

            foreach (SecsASCIIChar c in values)
            {
                if (ignoreCase) 
                    valueBytes.Add(SecsASCIIChar.ToLower(c.ToByte()));
                else 
                    valueBytes.Add(c.ToByte());
            }

            for (int i = 0; i < this._data.Length; i++)
            {
                byte b = this._data[i];

                if (ignoreCase) 
                    b = SecsASCIIChar.ToLower(b);

                if (valueBytes.Contains(this._data[i])) 
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Inserts the specified ASCII string into this object at the specified index.
        /// </summary>
        /// <param name="value">The ASCII string.</param>
        /// <param name="index">The index.</param>
        /// <returns>The resulting ASCII string after insertion is complete.</returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
        public SecsASCII Insert(SecsASCII value, int index)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            if (index < 0 || index > this._data.Length) 
                throw new ArgumentOutOfRangeException("index");

            int totalBytes = this._data.Length + value._data.Length;
            byte[] data = new byte[totalBytes];

            Buffer.BlockCopy(this._data, 0, data, 0, index);
            Buffer.BlockCopy(value._data, 0, data, index, value._data.Length);
            Buffer.BlockCopy(this._data, index, data, index + value._data.Length, this._data.Length - index);

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int LastIndexOf(SecsASCII value)
        {
            return this.LastIndexOf(value, 0, this.Length, false);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in this instance, using case-matching only if specified.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>
        /// The index of the start of the occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int LastIndexOf(SecsASCII value, bool ignoreCase)
        {
            return this.LastIndexOf(value, 0, this.Length, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in this instance. The search starts at the specified character position.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <returns>
        /// The index of the start of the occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int LastIndexOf(SecsASCII value, int startIndex)
        {
            return this.LastIndexOf(value, startIndex, this.Length - startIndex, false);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in this instance. The search starts at the specified character position and
        /// uses case-matching only if specified.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>
        /// The index of the start of the occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int LastIndexOf(SecsASCII value, int startIndex, bool ignoreCase)
        {
            return this.LastIndexOf(value, startIndex, this.Length - startIndex, ignoreCase);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in this instance. The search starts at the specified character position and
        /// examines a specified number of character positions.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="count">The number of positions to examine.</param>
        /// <returns>
        /// The index of the start of the occurrence, or -1 if no match is found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int LastIndexOf(SecsASCII value, int startIndex, int count)
        {
            return this.LastIndexOf(value, startIndex, count, false);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string
        /// in this instance. The search starts at a specified character position and
        /// examines a specified number of character positions.
        /// </summary>
        /// <param name="value">The ASCII string to seek.</param>
        /// <param name="startIndex">The search starting index.</param>
        /// <param name="count">The number of positions to examine.</param>
        /// <param name="ignoreCase">if set to <c>true</c> ignore case-matching.</param>
        /// <returns>The index of the start of the occurrence, or -1 if no match is found.</returns>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex or count are outside of range</exception>
        public int LastIndexOf(SecsASCII value, int startIndex, int count, bool ignoreCase)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            if (startIndex < 0 || startIndex > this._data.Length) 
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0 || startIndex + count > this._data.Length) 
                throw new ArgumentOutOfRangeException("count");

            int lastIndexFound = -1;
            int result = startIndex - 1;

            do
            {
                result = this.IndexOf(value, result + 1, count - (result + 1), ignoreCase);

                if (result >= 0)
                    lastIndexFound = result;
            }
            while (result >= 0 && result + 1 < this._data.Length - value._data.Length);

            return lastIndexFound;
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public int LastIndexOfAny(params SecsASCIIChar[] values)
        {
            return this.LastIndexOfAny(values, 0, this._data.Length, false);
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public int LastIndexOfAny(IEnumerable<SecsASCIIChar> values)
        {
            return this.LastIndexOfAny(values, 0, this._data.Length, false);
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public int LastIndexOfAny(IEnumerable<SecsASCIIChar> values, bool ignoreCase)
        {
            return this.LastIndexOfAny(values, 0, this._data.Length, ignoreCase);
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public int LastIndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex)
        {
            return this.LastIndexOfAny(values, startIndex, this._data.Length - startIndex, false);
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public int LastIndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex, bool ignoreCase)
        {
            return this.LastIndexOfAny(values, startIndex, this._data.Length - startIndex, ignoreCase);
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int LastIndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex, int count)
        {
            return this.LastIndexOfAny(values, startIndex, count, false);
        }

        /// <summary>
        /// Lasts the index of any.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">values</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex
        /// or
        /// count
        /// </exception>
        public int LastIndexOfAny(IEnumerable<SecsASCIIChar> values, int startIndex, int count, bool ignoreCase)
        {
            if (values == null) 
                throw new ArgumentNullException("values");

            if (startIndex < 0 || startIndex > this._data.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0 || startIndex + count > this._data.Length) 
                throw new ArgumentOutOfRangeException("count");

            List<byte> valueBytes = new List<byte>();

            foreach (SecsASCIIChar c in values)
            {
                if (ignoreCase) 
                    valueBytes.Add(SecsASCIIChar.ToLower(c.ToByte()));
                else 
                    valueBytes.Add(c.ToByte());
            }

            int lastIndex = -1;

            for (int i = 0; i < this._data.Length; i++)
            {
                byte b = this._data[i];

                if (ignoreCase) 
                    b = SecsASCIIChar.ToLower(b);

                if (valueBytes.Contains(this._data[i])) 
                    lastIndex = i;
            }

            return lastIndex;
        }

        /// <summary>
        /// Pads the left.
        /// </summary>
        /// <param name="totalLength">The total length.</param>
        /// <returns></returns>
        public SecsASCII PadLeft(int totalLength)
        {
            return this.PadLeft(totalLength, SecsASCIIChar.Space);
        }

        /// <summary>
        /// Pads the left.
        /// </summary>
        /// <param name="totalLength">The total length.</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">totalLength</exception>
        public SecsASCII PadLeft(int totalLength, SecsASCIIChar c)
        {
            if (totalLength < this._data.Length) 
                throw new ArgumentOutOfRangeException("totalLength");

            byte[] data = new byte[totalLength];
            byte charByte = c.ToByte();

            int i = 0;

            for (; i + this._data.Length < totalLength; i++)
            {
                data[i] = charByte;
            }

            Buffer.BlockCopy(this._data, 0, data, i, this._data.Length);

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Pads the right.
        /// </summary>
        /// <param name="totalLength">The total length.</param>
        /// <returns></returns>
        public SecsASCII PadRight(int totalLength)
        {
            return this.PadRight(totalLength, SecsASCIIChar.Space);
        }

        /// <summary>
        /// Pads the right.
        /// </summary>
        /// <param name="totalLength">The total length.</param>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">totalLength</exception>
        public SecsASCII PadRight(int totalLength, SecsASCIIChar c)
        {
            if (totalLength < this._data.Length) 
                throw new ArgumentOutOfRangeException("totalLength");

            byte[] data = new byte[totalLength];
            byte charByte = c.ToByte();

            Buffer.BlockCopy(this._data, 0, data, 0, this._data.Length);

            for (int i = this._data.Length; i < totalLength; i++)
            {
                data[i] = charByte;
            }

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Removes the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns></returns>
        public SecsASCII Remove(int startIndex)
        {
            return this.Remove(startIndex, this._data.Length - startIndex);
        }

        /// <summary>
        /// Removes the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex
        /// or
        /// count
        /// </exception>
        public SecsASCII Remove(int startIndex, int count)
        {
            if (startIndex < 0 || startIndex > this._data.Length) 
                throw new ArgumentOutOfRangeException("startIndex");

            if (count < 0 || startIndex + count > this._data.Length) 
                throw new ArgumentOutOfRangeException("count");

            byte[] data = new byte[this._data.Length - count];

            Buffer.BlockCopy(this._data, 0, data, 0, startIndex);
            Buffer.BlockCopy(this._data, startIndex + count, data, startIndex, this._data.Length - count - startIndex);

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Replaces the specified old string.
        /// </summary>
        /// <param name="oldString">The old string.</param>
        /// <param name="newString">The new string.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// oldString
        /// or
        /// newString
        /// </exception>
        public SecsASCII Replace(SecsASCII oldString, SecsASCII newString)
        {
            if (oldString == null) 
                throw new ArgumentNullException("oldString");

            if (newString == null) 
                throw new ArgumentNullException("newString");

            List<int> indexes = new List<int>();
            int index = 0;

            do
            {
                index = this.IndexOf(oldString, index, false);

                if (index >= 0)
                {
                    indexes.Add(index);
                    index += oldString._data.Length;
                }
            }
            while (index >= 0 && index + oldString.Length < this._data.Length);

            if (indexes.Count == 0)
            {
                return this.Clone();
            }

            byte[] data = new byte[this._data.Length - (oldString._data.Length * indexes.Count) + (newString._data.Length * indexes.Count)];

            int oldIndex = 0;
            int newIndex = 0;

            foreach (int stringIndex in indexes)
            {
                Buffer.BlockCopy(this._data, oldIndex, data, newIndex, stringIndex - oldIndex);
                newIndex += stringIndex - oldIndex;
                oldIndex = stringIndex + oldString._data.Length;
                Buffer.BlockCopy(newString._data, 0, data, newIndex, newString._data.Length);
                newIndex += newString._data.Length;
            }

            Buffer.BlockCopy(this._data, oldIndex, data, newIndex, this._data.Length - oldIndex);

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Replaces the specified old character.
        /// </summary>
        /// <param name="oldChar">The old character.</param>
        /// <param name="newChar">The new character.</param>
        /// <returns></returns>
        public SecsASCII Replace(SecsASCIIChar oldChar, SecsASCIIChar newChar)
        {
            if (oldChar == newChar)
                return this.Clone();

            SecsASCIIChar[] oldChars = new SecsASCIIChar[] { oldChar };

            List<int> indexes = new List<int>();
            int index = 0;

            do
            {
                index = this.IndexOfAny(oldChars, index, false);

                if (index >= 0)
                {
                    indexes.Add(index);
                    index++;
                }
            }
            while (index >= 0 && index + 1 < this._data.Length);

            if (indexes.Count == 0) return this.Clone();

            byte[] data = new byte[this._data.Length];

            int oldIndex = 0;
            int newIndex = 0;

            foreach (int stringIndex in indexes)
            {
                Buffer.BlockCopy(this._data, oldIndex, data, newIndex, stringIndex - oldIndex);
                newIndex += stringIndex - oldIndex;
                oldIndex = stringIndex + 1;
                data[newIndex] = newChar.ToByte();
                newIndex++;
            }

            Buffer.BlockCopy(this._data, oldIndex, data, newIndex, this._data.Length - oldIndex);

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <returns></returns>
        public SecsASCII[] Split(params SecsASCII[] seperators)
        {
            return this.Split(seperators, int.MaxValue, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCII> seperators)
        {
            return this.Split(seperators, int.MaxValue, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCII> seperators, StringSplitOptions options)
        {
            return this.Split(seperators, int.MaxValue, options);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCII> seperators, int count)
        {
            return this.Split(seperators, count, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <param name="count">The count.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCII> seperators, int count, StringSplitOptions options)
        {
            List<SecsASCII> parts = new List<SecsASCII>();

            int startIndex = 0;

            for (int dataIndex = 0; dataIndex < this._data.Length; dataIndex++)
            {
                int charsFound = 0;
                bool found = false;

                foreach (SecsASCII seperator in seperators)
                {
                    charsFound = 0;

                    if (dataIndex + seperator._data.Length > this._data.Length) break;

                    for (int sepIndex = 0; sepIndex < seperator.Length; sepIndex++)
                    {
                        if (this._data[dataIndex + sepIndex] == seperator[sepIndex]) 
                            charsFound++;
                        else 
                            charsFound = 0;
                    }

                    if (charsFound == seperator._data.Length) 
                        found = true;
                }

                if (found)
                {
                    SecsASCII part = this.Substring(startIndex, dataIndex - startIndex);

                    if (part._data.Length > 0 || options == StringSplitOptions.None)
                        parts.Add(part);

                    startIndex = dataIndex + charsFound;
                    dataIndex += charsFound - 1;

                    if (parts.Count + 1 == count) 
                        break;
                }
            }

            SecsASCII remainingPart = this.Substring(startIndex);
            if (remainingPart._data.Length > 0 || options == StringSplitOptions.None)
                parts.Add(remainingPart);

            return parts.ToArray();
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <returns></returns>
        public SecsASCII[] Split(params SecsASCIIChar[] seperators)
        {
            return this.Split(seperators, int.MaxValue, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCIIChar> seperators)
        {
            return this.Split(seperators, int.MaxValue, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits the specified seperators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCIIChar> seperators, StringSplitOptions options)
        {
            return this.Split(seperators, int.MaxValue, options);
        }

        /// <summary>
        /// Splits the specified separators.
        /// </summary>
        /// <param name="seperators">The seperators.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public SecsASCII[] Split(IEnumerable<SecsASCIIChar> separators, int count)
        {
            return this.Split(separators, count, StringSplitOptions.None);
        }

        public SecsASCII[] Split(IEnumerable<SecsASCIIChar> separators, int count, StringSplitOptions options)
        {
            List<SecsASCII> parts = new List<SecsASCII>();

            int startIndex = 0;

            for (int dataIndex = 0; dataIndex < this._data.Length; dataIndex++)
            {
                bool found = false;

                foreach (SecsASCIIChar separator in separators)
                {
                    if (this._data[dataIndex] == separator)
                        found = true;
                }

                if (found)
                {
                    SecsASCII part = this.Substring(startIndex, dataIndex - startIndex);

                    if (part._data.Length > 0 || options == StringSplitOptions.None)
                        parts.Add(part);

                    startIndex = dataIndex + 1;

                    if (parts.Count + 1 == count)
                        break;
                }
            }

            SecsASCII remainingPart = this.Substring(startIndex);
            if (remainingPart._data.Length > 0 || options == StringSplitOptions.None)
                parts.Add(remainingPart);

            return parts.ToArray();
        }

        public bool StartsWith(SecsASCII value)
        {
            return this.StartsWith(value, false);
        }

        public bool StartsWith(SecsASCII value, bool ignoreCase)
        {
            if (value == null) throw new ArgumentNullException("value");
            return SecsASCII.Compare(this, 0, value, 0, value.Length, ignoreCase) == 0;
        }

        public SecsASCII Substring(int startIndex)
        {
            return this.Substring(startIndex, this._data.Length - startIndex);
        }

        public SecsASCII Substring(int startIndex, int length)
        {
            if (startIndex < 0 || startIndex > _data.Length) 
                throw new ArgumentOutOfRangeException("startIndex");

            if (length < 0 || startIndex + length > _data.Length) 
                throw new ArgumentOutOfRangeException("length");

            byte[] newData = new byte[length];
            Buffer.BlockCopy(_data, startIndex, newData, 0, length);
            return new SecsASCII(newData, true);
        }

        public SecsASCIIChar[] ToCharArray()
        {
            SecsASCIIChar[] chars = new SecsASCIIChar[this._data.Length];
            for (int i = 0; i < this._data.Length; i++)
                chars[i] = new SecsASCIIChar(this._data[i]);

            return chars;
        }

        public SecsASCII ToLower()
        {
            SecsASCII s = SecsASCII.Copy(this);

            for (int i = 0; i < s._data.Length; i++)
            {
                byte b = s._data[i];
                if (SecsASCIIChar.IsUpper(b))
                    s._data[i] = SecsASCIIChar.ToLower(b);
            }

            return s;
        }

        public SecsASCII ToUpper()
        {
            SecsASCII s = SecsASCII.Copy(this);

            for (int i = 0; i < s._data.Length; i++)
            {
                byte b = s._data[i];
                if (SecsASCIIChar.IsLower(b))
                    s._data[i] = SecsASCIIChar.ToUpper(b);
            }

            return s;
        }

        /// <summary>
        /// Trims whitespace from the beginning and end of this ASCII string.
        /// </summary>
        /// <returns></returns>
        public SecsASCII Trim()
        {
            int charsAtStart = 0;
            int charsAtEnd = 0;

            for (int i = 0; i < this._data.Length; i++)
            {
                if (SecsASCIIChar.IsWhitespace(this._data[i]))
                    charsAtStart++;
                else
                    break;
            }

            for (int i = this._data.Length - 1; i >= charsAtStart; i--)
            {
                if (SecsASCIIChar.IsWhitespace(this._data[i]))
                    charsAtEnd++;
                else
                    break;
            }

            byte[] data = new byte[this._data.Length - charsAtStart - charsAtEnd];
            Buffer.BlockCopy(this._data, charsAtStart, data, 0, data.Length);
            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Trims the specified characters from the beginning and end of this ASCII string.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public SecsASCII Trim(params SecsASCIIChar[] values)
        {
            int charsAtStart = 0;
            int charsAtEnd = 0;

            for (int i = 0; i < this._data.Length; i++)
            {
                bool found = false;

                foreach (SecsASCIIChar c in values)
                {
                    if (this._data[i].Equals(c.ToByte()))
                    {
                        charsAtStart++;
                        found = true;
                        break;
                    }
                }

                if (!found) 
                    break;
            }

            for (int i = this._data.Length - 1; i >= charsAtStart; i--)
            {
                bool found = false;

                foreach (SecsASCIIChar c in values)
                {
                    if (this._data[i].Equals(c.ToByte()))
                    {
                        charsAtEnd++;
                        found = true;
                        break;
                    }
                }

                if (!found) 
                    break;
            }

            byte[] data = new byte[this._data.Length - charsAtStart - charsAtEnd];
            Buffer.BlockCopy(this._data, charsAtStart, data, 0, data.Length);
            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Trims the whitespace characters off the end of this ASCII string.
        /// </summary>
        /// <returns>The resulting ASCII string.</returns>
        public SecsASCII TrimEnd()
        {
            int charsAtEnd = 0;

            for (int i = this._data.Length - 1; i >= 0; i--)
            {
                if (SecsASCIIChar.IsWhitespace(this._data[i]))
                    charsAtEnd++;
                else
                    break;
            }

            byte[] data = new byte[this._data.Length - charsAtEnd];
            Buffer.BlockCopy(this._data, 0, data, 0, data.Length);
            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Trims the specified ASCII characters off the end of this ASCII string.
        /// </summary>
        /// <param name="values">The ASCII characters to trim.</param>
        /// <returns>The resulting ASCII string.</returns>
        public SecsASCII TrimEnd(params SecsASCIIChar[] values)
        {
            int charsAtEnd = 0;

            for (int i = this._data.Length - 1; i >= 0; i--)
            {
                bool found = false;

                foreach (SecsASCIIChar c in values)
                {
                    if (this._data[i].Equals(c.ToByte()))
                    {
                        charsAtEnd++;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    break;
            }

            byte[] data = new byte[this._data.Length - charsAtEnd];
            Buffer.BlockCopy(this._data, 0, data, 0, data.Length);
            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Trims the whitespace ASCII characters from the start of this ASCII string.
        /// </summary>
        /// <returns>The resulting ASCII string.</returns>
        public SecsASCII TrimStart()
        {
            int charsAtStart = 0;

            for (int i = 0; i < this._data.Length; i++)
            {
                if (SecsASCIIChar.IsWhitespace(this._data[i]))
                    charsAtStart++;
                else
                    break;
            }

            byte[] data = new byte[this._data.Length - charsAtStart];
            Buffer.BlockCopy(this._data, charsAtStart, data, 0, data.Length);
            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Trims the specified ASCII characters from the start of this ASCII string.
        /// </summary>
        /// <param name="values">The ASCII characters to trim.</param>
        /// <returns>The resulting ASCII string.</returns>
        public SecsASCII TrimStart(params SecsASCIIChar[] values)
        {
            int charsAtStart = 0;

            for (int i = 0; i < this._data.Length; i++)
            {
                bool found = false;

                foreach (SecsASCIIChar c in values)
                {
                    if (this._data[i].Equals(c.ToByte()))
                    {
                        charsAtStart++;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    break;
            }

            byte[] data = new byte[this._data.Length - charsAtStart];
            Buffer.BlockCopy(this._data, charsAtStart, data, 0, data.Length);
            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Encoding.ASCII.GetString(_data);
        }

        /// <summary>
        /// Compares the specified string1.
        /// </summary>
        /// <param name="string1">The string1.</param>
        /// <param name="string2">The string2.</param>
        /// <returns></returns>
        public static int Compare(SecsASCII string1, SecsASCII string2)
        {
            return Compare(string1, string2, false);
        }

        /// <summary>
        /// Compares the specified string1.
        /// </summary>
        /// <param name="string1">The string1.</param>
        /// <param name="string2">The string2.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// string1
        /// or
        /// string2
        /// </exception>
        public static int Compare(SecsASCII string1, SecsASCII string2, bool ignoreCase)
        {
            if (string1 == null) 
                throw new ArgumentNullException("string1");

            if (string2 == null) 
                throw new ArgumentNullException("string2");

            return SafeCompare(string1, 0, string2, 0, Math.Max(string1._data.Length, string2._data.Length), ignoreCase);
        }

        /// <summary>
        /// Compares the specified string1.
        /// </summary>
        /// <param name="string1">The string1.</param>
        /// <param name="indexA">The index a.</param>
        /// <param name="string2">The string2.</param>
        /// <param name="indexB">The index b.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static int Compare(SecsASCII string1, int indexA, SecsASCII string2, int indexB, int length)
        {
            return Compare(string1, indexA, string2, indexB, length, false);
        }

        /// <summary>
        /// Compares the specified string1.
        /// </summary>
        /// <param name="string1">The string1.</param>
        /// <param name="index1">The index1.</param>
        /// <param name="string2">The string2.</param>
        /// <param name="index2">The index2.</param>
        /// <param name="length">The length.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// string1
        /// or
        /// string2
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// index1
        /// or
        /// index2
        /// or
        /// length
        /// </exception>
        public static int Compare(SecsASCII string1, int index1, SecsASCII string2, int index2, int length, bool ignoreCase)
        {
            if (string1 == null) 
                throw new ArgumentNullException("string1");
            if (string2 == null) 
                throw new ArgumentNullException("string2");
            if (index1 < 0 || index1 > string1._data.Length) 
                throw new ArgumentOutOfRangeException("index1");
            if (index2 < 0 || index2 > string2._data.Length) 
                throw new ArgumentOutOfRangeException("index2");
            if (length < 0 || index1 + length > string1._data.Length || index2 + length > string2._data.Length) 
                throw new ArgumentOutOfRangeException("length");

            return SafeCompare(string1, index1, string2, index2, length, ignoreCase);
        }

        private static int SafeCompare(SecsASCII strA, int indexA, SecsASCII strB, int indexB, int length, bool ignoreCase)
        {
            for (int i = 0; i < length; i++)
            {
                int iA = i + indexA;
                int iB = i + indexB;

                if (iA == strA._data.Length && iB == strB._data.Length) 
                    return 0;

                if (iA == strA._data.Length) 
                    return -1;

                if (iB == strB._data.Length) 
                    return 1;

                byte byteA = strA._data[iA];
                byte byteB = strB._data[iB];

                if (ignoreCase)
                {
                    byteA = SecsASCIIChar.ToLower(byteA);
                    byteB = SecsASCIIChar.ToLower(byteB);
                }

                if (byteA < byteB) 
                    return -1;

                if (byteB < byteA)
                    return 1;
            }

            return 0;
        }

        /// <summary>
        /// Creates a copy of the specified string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The copy.</returns>
        public static SecsASCII Copy(SecsASCII value)
        {
            return new SecsASCII(value._data, false);
        }

        /// <summary>
        /// Concatenates the specified strings to the end of this ASCII string.
        /// </summary>
        /// <param name="values">The ASCII string values.</param>
        /// <returns>The resulting ASCII string.</returns>
        public static SecsASCII Concat(params SecsASCII[] values)
        {
            return Concat((IEnumerable<SecsASCII>)values);
        }

        /// <summary>
        /// Concats the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">values</exception>
        public static SecsASCII Concat(IEnumerable<SecsASCII> values)
        {
            if (values == null) 
                throw new ArgumentNullException("values");

            int totalBytes = 0;

            foreach (SecsASCII asciiString in values)
            {
                if (asciiString == null) 
                    continue;

                totalBytes += asciiString._data.Length;
            }

            byte[] data = new byte[totalBytes];
            int offset = 0;

            foreach (SecsASCII asciiString in values)
            {
                if (asciiString == null)
                    continue;

                Buffer.BlockCopy(asciiString._data, 0, data, offset, asciiString._data.Length);
                offset += asciiString._data.Length;
            }

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Determines whether the specified ASCII string is null or empty.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns><c>true</c> if the specified string is null or empty.</returns>
        public static bool IsNullOrEmpty(SecsASCII value)
        {
            return (value == null || value._data.Length == 0);
        }

        /// <summary>
        /// Determines whether the specified ASCII string is null or empty.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns><c>true</c> if the specified string is null or whitespace.</returns>
        public static bool IsNullOrWhitespace(SecsASCII value)
        {
            if (value == null || value._data.Length == 0)
                return true;

            foreach (byte b in value._data)
            {
                if (!SecsASCIIChar.IsWhitespace(b))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Joins the specified separator.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static SecsASCII Join(SecsASCII separator, params SecsASCII[] values)
        {
            return Join(separator, (IEnumerable<SecsASCII>)values);
        }

        /// <summary>
        /// Joins the specified separator.
        /// </summary>
        /// <param name="separator">The separator.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// separator
        /// or
        /// values
        /// </exception>
        public static SecsASCII Join(SecsASCII separator, IEnumerable<SecsASCII> values)
        {
            if (separator == null) throw new ArgumentNullException("separator");
            if (values == null) throw new ArgumentNullException("values");

            int totalBytes = 0;
            
            foreach (SecsASCII asciiString in values)
            {
                if (asciiString == null) 
                    continue;

                totalBytes += asciiString._data.Length;
                totalBytes += separator._data.Length;
            }

            if (totalBytes > 0) 
                totalBytes -= separator._data.Length;

            byte[] data = new byte[totalBytes];
            int offset = 0;

            foreach (SecsASCII asciiString in values)
            {
                if (asciiString == null) 
                    continue;

                Buffer.BlockCopy(asciiString._data, 0, data, offset, asciiString._data.Length);
                offset += asciiString._data.Length;

                if (offset < totalBytes)
                {
                    Buffer.BlockCopy(separator._data, 0, data, offset, separator._data.Length);
                    offset += separator._data.Length;
                }
            }

            return new SecsASCII(data, true);
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public static SecsASCII Parse(string value)
        {
            if (value == null) 
                throw new ArgumentNullException("value");

            return new SecsASCII(Encoding.ASCII.GetBytes(value), true);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static SecsASCII operator +(SecsASCII val1, SecsASCII val2)
        {
            return SecsASCII.Concat(val1, val2);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="chr">The character.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">val1</exception>
        public static SecsASCII operator +(SecsASCII val1, SecsASCIIChar chr)
        {
            if (val1 == null) 
                throw new ArgumentNullException("val1");

            int totalBytes = val1._data.Length + 1;

            byte[] data = new byte[totalBytes];

            Buffer.BlockCopy(val1._data, 0, data, 0, val1._data.Length);
            data[totalBytes - 1] = chr.ToByte();

            return new SecsASCII(data);
        }

        /// <summary>
        /// Determines if the two <see cref="SecsASCII"/> values are equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if equal; otherwise, false.</returns>
        public static bool operator ==(SecsASCII val1, SecsASCII val2)
        {
            return SecsASCII.Compare(val1, val2) == 0;
        }

        /// <summary>
        /// Determines if the two <see cref="SecsASCII"/> values are not equal.
        /// </summary>
        /// <param name="val1">Value 1</param>
        /// <param name="val2">Value 2</param>
        /// <returns>True, if not equal; otherwise, false.</returns>
        public static bool operator !=(SecsASCII val1, SecsASCII val2)
        {
            return SecsASCII.Compare(val1, val2) != 0;
        }

        /// <summary>
        /// Implicit conversion from <see cref="SecsASCII"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator String(SecsASCII value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Implicit conversion from <see cref="System.String"/> to <see cref="SecsASCII"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator SecsASCII(string value)
        {
            return new SecsASCII(value);
        }
    }
}
