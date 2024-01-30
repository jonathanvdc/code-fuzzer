using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeFuzzer
{
    /// <summary>
    /// A token definition for single-line comments.
    /// </summary>
    public class SingleLineCommentDef : ITokenDef
    {
        public SingleLineCommentDef(
            string Prefix, IEnumerable<char> Terminators,
            int MaxCommentLength)
        {
            this.Prefix = Prefix;
            this.MaxCommentLength = MaxCommentLength;
            this.termArr = Enumerable.ToArray<char>(Terminators);
            this.terms = new HashSet<char>(termArr);
            var validCharList = new List<char>();
            for (char i = (char)0x20; i <= (char)0xFE; i++)
            {
                if (!terms.Contains(i))
                    validCharList.Add(i);
            }
            this.validChars = validCharList.ToArray();
        }

        /// <summary>
        /// Gets the prefix that starts a single-line comment.
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Gets the set of terminators for this single-line comment definition.
        /// </summary>
        public IEnumerable<char> Terminators { get { return terms; } }

        /// <summary>
        /// Gets the maximum length of a comment's contents.
        /// </summary>
        public int MaxCommentLength { get; private set; }

        private HashSet<char> terms;
        private char[] termArr;
        private char[] validChars;

        /// <summary>
        /// Reads a token of this type from the given string slice. The length
        /// of the token is then returned. If the front of the slice does not
        /// define such a token, then zero is returned.
        /// </summary>
        public int Read(ForwardSlice Slice)
        {
            if (Slice.Pop(Prefix.Length) != Prefix)
                return 0;

            int i = Prefix.Length;
            char c;
            while (Slice.TryPop(out c) && !terms.Contains(c))
                i++;

            if (Slice.TryPop(out c))
            {
                // assert(terms.Contains(c))
                i++;
            }

            return i;
        }

        /// <summary>
        /// Generates a new token value.
        /// </summary>
        public string Generate(Random Rand)
        {
            return Prefix
                + Helpers.GenerateRandomString(
                    validChars, Rand.Next(0, MaxCommentLength), Rand)
                + Helpers.PickRandomElement<char>(termArr, Rand).ToString();
        }
    }
}
