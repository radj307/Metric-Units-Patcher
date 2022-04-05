using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetricUnits.Util
{
    internal class Utilities
    {
        /// <summary>
        /// Returns true when c is any digit, a period, or a dash. (negative)
        /// </summary>
        /// <param name="c">Input Character</param>
        /// <returns>bool</returns>
        public static bool IsValidNumber(char c) => char.IsDigit(c) || c == '.' || c == '-';
        /// <summary>
        /// Returns true when all characters in the given string are digits, periods, or dashes.
        /// </summary>
        /// <param name="s">Input String</param>
        /// <returns>bool</returns>
        public static bool IsValidNumber(string s) => s.All(IsValidNumber);

        /// <summary>
        /// The regular expression used to detect imperial length units.
        /// </summary>
        public static Regex regex = new("(<*)([0-9.-]+)(>*)\\s+?\\b(foot|feet|inch|inches|mile|miles)\\b", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Convert all values measured with imperial length units in the input string into their metric equivalents by using regular expressions & the ckconv executable.
        /// </summary>
        /// <param name="s">Input String</param>
        /// <param name="ckconv">Converter Object</param>
        /// <param name="Settings">Settings Object</param>
        /// <param name="editor_id">Optional editor ID used to print a more descriptive log message.</param>
        /// <returns><list type="table">
        /// <item><term>Item1</term><description>The output string.</description></item>
        /// <item><term>Item2</term><description>The number of changes made to the string.</description></item>
        /// </list></returns>
        public static (string, ulong) PatchString(string s, CreationKitUnitConverter ckconv, Settings Settings, string? editor_id = null)
        {
            if (s.Length == 0)
                return (s, 0);
            ulong count = 0ul;

            s = regex.Replace(s, delegate (Match m)
            {
                if (!IsValidNumber(m.Groups[1].Value))
                    return m.Groups[0].Value; // not all characters are valid when converted to a double

                string in_meters = ckconv.Convert(m.Groups[2].Value, m.Groups[4].Value, "m");

                if (in_meters.Length == 0)
                    return m.Groups[0].Value;

                bool centimeters = false;

                var i = in_meters.IndexOf('.');
                if (!in_meters.StartsWith('0'))
                {
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
                else if (Settings.allow_centimeters) // move the decimal 2 to the right and call it centimeters
                {
                    in_meters = in_meters.Remove(0, i + 1).Insert(i + 1, ".");
                    centimeters = true;
                }
                // else ignore, or result could potentially be 0

                string s = $"{m.Groups[1].Value}{in_meters}{m.Groups[3].Value} {(centimeters ? "centi" : "")}{(Settings.use_american_spelling ? "meters" : "metres")}";

                ++count;

                if (editor_id != null)
                    Console.WriteLine($"'{m.Groups[0].Value}' => '{s}' in record {editor_id}");
                else
                    Console.WriteLine($"'{m.Groups[0].Value}' => '{s}'");

                return s;
            });

            return (s, count);
        }
    }
}
