using System;
using System.Text;

namespace CodeFuzzer
{
    /// <summary>
    /// A slice of a string that can only move forward.
    /// </summary>
    public struct ForwardSlice
    {
        public ForwardSlice(string Value)
        {
            this.pos = 0;
            this.val = Value;
        }

        private int pos;
        private string val;

        /// <summary>
        /// Checks if the forward slice is empty.
        /// </summary>
        public bool IsEmpty { get { return Length == 0; } }

        /// <summary>
        /// Gets the length of this forward slice.
        /// </summary>
        public int Length { get { return val.Length - pos; } }

        /// <summary>
        /// Tries to peek a character the first character in the forward slice.
        /// </summary>
        public bool TryPeek(out char Result)
        {
            if (IsEmpty)
            {
                Result = (char)0;
                return false;
            }
            else
            {
                Result = val[pos];
                return true;
            }
        }

        /// <summary>
        /// Tries to pop a character the first character in the forward slice.
        /// </summary>
        public bool TryPop(out char Result)
        {
            bool success = TryPeek(out Result);
            if (success)
                pos++;

            return success;
        }

        /// <summary>
        /// Pops a string of the given length from the forward slice. If the
        /// desired number of characters exceeds the slice's length, then
        /// the entire slice is returned.
        /// </summary>
        public string Peek(int Count)
        {
            int len = Math.Min(Count, this.Length);
            return val.Substring(pos, len);
        }

        /// <summary>
        /// Pops a string of the given length from the forward slice. If the
        /// desired number of characters exceeds the slice's length, then
        /// the entire slice is returned.
        /// </summary>
        public string Pop(int Count)
        {
            int len = Math.Min(Count, this.Length);
            string result = val.Substring(pos, len);
            pos += len;
            return result;
        }

        public string Substring(int Offset, int Count)
        {
            return val.Substring(pos + Offset, Count);
        }

        public string Substring(int Count)
        {
            return Substring(0, Count);
        }

        public override string ToString()
        {
            return Peek(Length);
        }
    }
}
