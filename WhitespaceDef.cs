using System;

namespace CodeFuzzer
{

/// <summary>
/// A token definition for whitespace.
/// </summary>
public class WhitespaceDef : ITokenDef
{
    public WhitespaceDef()
    { }

    /// <summary>
    /// Reads a token of this type from the given string slice. The length
    /// of the token is then returned. If the front of the slice does not
    /// define such a token, then zero is returned.
    /// </summary>
    public int Read(ForwardSlice Slice)
    {
        char c;
        return Slice.TryPop(out c) && char.IsWhiteSpace(c) ? 1 : 0;
    }

    private static string[] wsChars = new string[] { " ", "\n", "\t", "\r" };

    /// <summary>
    /// Generates a new token value.
    /// </summary>
    public string Generate(Random Rand)
    {
        return Helpers.PickRandomElement<string>(wsChars, Rand);
    }
}

}
