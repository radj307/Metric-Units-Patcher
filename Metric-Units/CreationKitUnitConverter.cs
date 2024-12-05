using System.Diagnostics;
using System.IO;

namespace MetricUnits
{
    /// <summary>
    /// Exposes measurement unit conversion methods. <br/><br/>
    /// This uses an embedded version of <b>ckconv.exe</b> to perform the actual conversions. <br/>
    /// You can find more information on ckconv here: <see href="https://github.com/radj307/Gamebryo-Engine-Unit-Converter"/>
    /// </summary>
    internal class CreationKitUnitConverter
    {
        public CreationKitUnitConverter(string executable = "ckconv.exe")
        {
            executable_name = executable;
            string temp_path = Path.GetTempPath();
            path = Path.Combine(temp_path, executable_name);
            // write the executable resource to temp
            File.WriteAllBytes(path, Properties.Resources.ckconv);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Failed to write required resource to temp! Check the permissions of directory: \"{temp_path}\"", executable_name);
        }

        private readonly string executable_name;
        private readonly string path;

        /// <summary>
        /// Calls ckconv with the given arguments, and returns the first line from STDOUT.
        /// </summary>
        /// <param name="arguments">The full argument string to pass to ckconv.exe</param>
        /// <returns>string?</returns>
        private string? Exec(string arguments)
        {
            using Process ckconv = new();
            ckconv.StartInfo.FileName = path;
            ckconv.StartInfo.Arguments = arguments;
            ckconv.StartInfo.UseShellExecute = false;
            ckconv.StartInfo.RedirectStandardOutput = true;
            ckconv.Start();
            return ckconv.StandardOutput.ReadLine(); // read until newline
        }

        /// <summary>
        /// Gets the version number of the embedded executable.
        /// </summary>
        /// <param name="include_name">When true, the utility name is included. ('ckconv')</param>
        /// <returns>String containing the version number of ckconv.</returns>
        public string GetVersion(bool include_name = true) => Exec($"-v{(include_name ? "" : "q")}") ?? "[NULL]";

        /// <summary>
        /// Convert the given input unit and value to a specified output unit. <br/>
        /// This is simply a wrapper around the <see cref="Exec(string)"/> function that automatically fills in the needed commandline arguments.
        /// </summary>
        /// <param name="input_value">The value to convert.</param>
        /// <param name="input_unit">The name of the unit to convert from. (This is the unit that 'input_value' is measured in.)</param>
        /// <param name="output_unit">The name of the unit to convert to.</param>
        /// <param name="additionalArgs">Optional additional arguments to pass to ckconv.exe</param>
        /// <returns>String representation of the converted value.</returns>
        public string Convert(string input_value, string input_unit, string output_unit, string? additionalArgs = null)
        {
            var result = Exec($"-nq {additionalArgs} {input_unit} {input_value} {output_unit}");
            if (result == null)
                return input_value;
            return result;
        }
    }
}
