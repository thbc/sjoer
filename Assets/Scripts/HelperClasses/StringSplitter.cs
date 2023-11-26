using System;
using System.Linq;

namespace Assets.HelperClasses
{
    public class StringSplitter
    {
        public static string SplitString(string input)
        {
            int maxFinalLength = 12;
            int hyphenPosition = 5; // Hyphen after 5th character

            if (input.Length <= maxFinalLength)
            {
                return input;
            }

            int firstSpaceIndex = input.IndexOf(' ');
            if (firstSpaceIndex != -1 && firstSpaceIndex < maxFinalLength)
            {
                return input.Substring(0, firstSpaceIndex);
            }

            if (input.Contains(" "))
            {
                return Abbreviate(input, maxFinalLength);
            }

            return AddHyphenAtPosition(input, hyphenPosition, maxFinalLength);
        }

        private static string Abbreviate(string input, int maxLength)
        {
            var abbreviation = string.Concat(input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(word => word[0]));
            return abbreviation.Length > maxLength ? abbreviation.Substring(0, maxLength) : abbreviation;
        }

        private static string AddHyphenAtPosition(string input, int position, int maxLength)
        {
            if (input.Length <= position)
            {
                return input;
            }

            string result = input.Substring(0, position) + "-" + input.Substring(position);

            // Check if the second part after the hyphen is too short
            int hyphenIndex = result.IndexOf('-');
            string secondPart = result.Substring(hyphenIndex + 1);
            if (secondPart.Length <= 2)
            {
                result = result.Substring(0, hyphenIndex); // Trim after the hyphen
            }

            return result.Length > maxLength ? result.Substring(0, maxLength) : result;
        }
    }
}
