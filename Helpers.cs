using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeFuzzer
{
    public static class Helpers
    {
        /// <summary>
        /// Tests if the contents of two lists are equal.
        /// </summary>
        public static bool ListEqual<T>(IReadOnlyList<T> Left, IReadOnlyList<T> Right)
        {
            return Enumerable.SequenceEqual<T>(Left, Right);

            /*
            // Alternative implementation
            int lCount = Left.Count;
            if (lCount != Right.Count)
                return false;

            for (int i = 0; i < lCount; i++)
            {
                if (!Left[i].Equals(Right[i]))
                    return false;
            }
            return true;
            */
        }

        /// <summary>
        /// Randomly picks an element from the given list.
        /// </summary>
        public static T PickRandomElement<T>(IReadOnlyList<T> List, Random Rand)
        {
            return List[Rand.Next(0, List.Count)];
        }

        /// <summary>
        /// Randomly picks an element from the given list.
        /// </summary>
        public static T PickRandomElement<T>(T[] List, Random Rand)
        {
            return List[Rand.Next(0, List.Length)];
        }

        /// <summary>
        /// Generates a random concatenation of the given set of characters.
        /// The resulting string has the specified length.
        /// </summary>
        public static string GenerateRandomString(
            char[] Characters, int Length, Random Rand)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Length; i++)
                sb.Append(PickRandomElement<char>(Characters, Rand));

            return sb.ToString();
        }
    }
}
