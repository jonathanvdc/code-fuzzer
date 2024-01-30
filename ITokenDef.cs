using System;
using System.IO;

namespace CodeFuzzer
{
    /// <summary>
    /// An interface for token definitions, i.e., values that can recognize
    /// strings as a token type, and generate random strings for the token type.
    /// </summary>
    public interface ITokenDef
    {
        /// <summary>
        /// Reads a token of this type from the given string slice. The length
        /// of the token is then returned. If the front of the slice does not
        /// define such a token, then zero is returned.
        /// </summary>
        int Read(ForwardSlice Slice);

        /// <summary>
        /// Generates a new token value.
        /// </summary>
        string Generate(Random Rand);
    }
}
