using Mutagen.Bethesda.Strings;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetricUnits.Util
{
    internal class Utilities
    {
        /// <summary>
        /// Returns true when the given character is a digit.
        /// </summary>
        /// <param name="c">Input Character</param>
        /// <returns>bool</returns>
        public static bool Isdigit(char c)
        {
            switch (c)
            {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return true;
            default:
                return false;
            }
        }
        /// <summary>
        /// Returns true when c is any digit, a period, or a dash. (negative)
        /// </summary>
        /// <param name="c">Input Character</param>
        /// <returns>bool</returns>
        public static bool IsValidNumber(char c)
        {
            return Isdigit(c) || c == '.' || c == '-';
        }
        /// <summary>
        /// Returns true when all characters in the given string are digits, periods, or dashes.
        /// </summary>
        /// <param name="s">Input String</param>
        /// <returns>bool</returns>
        public static bool IsValidNumber(string s)
        {
            return s.All(ch => IsValidNumber(ch));
        }

        public static Regex regex = new("(<*)([0-9.-]+)(>*)\\s\\b(foot|feet|inch|inches|mile|miles)\\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public static (string, ulong) PatchString(string s, CreationKitUnitConverter ckconv, Settings Settings, string? editor_id = null)
        {
            if (s.Length == 0)
                return (s, 0);
            ulong count = 0ul;

            s = regex.Replace(s, delegate (Match m)
            {
                if (!IsValidNumber(m.Groups[1].Value))
                    return m.Groups[0].Value; // not all characters are valid when converted to a double

                // get the equivalent value in meters from ckconv.exe
                string in_meters = ckconv.Convert(m.Groups[2].Value, m.Groups[4].Value, "m");

                if (in_meters.Length == 0)
                    return m.Groups[0].Value;

                bool centimeters = false;

                if (!in_meters.StartsWith('0'))
                {
                    var i = in_meters.IndexOf('.');
                    if (i != -1)
                    {
                        if (Settings.max_decimal == 0)
                            in_meters = in_meters[..i];
                        else
                        {
                            int end = i + (int)Settings.max_decimal + 1;
                            if (end < in_meters.Length)
                                in_meters = in_meters[..end];
                        }
                    }
                }
                else
                {
                    in_meters = ckconv.Convert(m.Groups[2].Value, m.Groups[4].Value, "cm");
                    centimeters = true;
                }

                string s = $"{m.Groups[1].Value}{in_meters}{m.Groups[3].Value} {(centimeters ? "centi" : "")}{(Settings.use_american_spelling ? "meters" : "metres")}";

                ++count;

                if (editor_id != null)
                    Console.WriteLine($"\"{m.Groups[0].Value}\" => \"{s}\" in record {editor_id}");
                else
                    Console.WriteLine($"\"{m.Groups[0].Value}\" => \"{s}\"");

                return s;
            });

            return (s, count);
        }
    }
}
