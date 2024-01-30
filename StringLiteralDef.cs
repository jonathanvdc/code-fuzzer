using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeFuzzer;

namespace MiniCFuzzer
{
    /// <summary>
    /// A token definition for string literals.
    /// </summary>
    public class StringLiteralDef : ITokenDef
    {
        public StringLiteralDef(
            char Delimiter, IEnumerable<char> EscapableCharacters,
            int MinStringLength, int MaxStringLength)
        {
            this.Delimiter = Delimiter;
            this.MinStringLength = MinStringLength;
            this.MaxStringLength = MaxStringLength;
            this.escCharArr = Enumerable.ToArray<char>(EscapableCharacters);
            this.escCharSet = new HashSet<char>(escCharArr);
            List<char> validCharList = new List<char>();
            for (char i = (char)0x20; i <= (char)0xFE; i++)
            {
                if (i != '\\' && i != Delimiter)
                    validCharList.Add(i);
            }
            this.validChars = validCharList.ToArray();
        }

        /// <summary>
        /// Gets the delimiter character for the string literal.
        /// </summary>
        public char Delimiter { get; private set; }

        /// <summary>
        /// Gets the set of escapable characters in a string literal.
        /// </summary>
        public IEnumerable<char> EscapableCharacters { get { return escCharSet; } }

        /// <summary>
        /// Gets the minimal lenght of a string token's contents.
        /// </summary>
        public int MinStringLength { get; private set; }

        /// <summary>
        /// Gets the maximal length of a string token's contents.
        /// </summary>
        public int MaxStringLength { get; private set; }

        private char[] escCharArr;
        private char[] validChars;
        private HashSet<char> escCharSet;

        /// <summary>
        /// Reads a token of this type from the given string slice. The length
        /// of the token is then returned. If the front of the slice does not
        /// define such a token, then zero is returned.
        /// </summary>
        public int Read(ForwardSlice Slice)
        {
            int oldLength = Slice.Length;
            char c;
            if (!Slice.TryPop(out c) || c != Delimiter)
                return 0;

            while (Slice.TryPop(out c))
            {
                if (c == '\\')
                    Slice.TryPop(out c);
                else if (c == Delimiter)
                    break;
            }

            // Read the delimiter.
            Slice.TryPop(out c);

            return oldLength - Slice.Length;
        }

        /// <summary>
        /// Generates a new token value.
        /// </summary>
        public string Generate(Random Rand)
        {
            string contents = Helpers.GenerateRandomString(
                    validChars, Rand.Next(MinStringLength, MaxStringLength + 1), Rand);

            int escSeqCount = Rand.Next(0, Math.Min(1, contents.Length / 4));
            var escPoints = new HashSet<int>();
            for (int i = 0; i < escSeqCount; i++)
            {
                escPoints.Add(Rand.Next(0, contents.Length));
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(Delimiter);
            for (int i = 0; i < contents.Length; i++)
            {
                if (escPoints.Contains(i))
                {
                    sb.Append('\\');
                    sb.Append(Helpers.PickRandomElement<char>(escCharArr, Rand));
                }
                else
                {
                    sb.Append(contents[i]);
                }
            }
            sb.Append(Delimiter);
            return sb.ToString();
        }
    }
}
