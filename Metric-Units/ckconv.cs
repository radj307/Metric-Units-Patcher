using System.Diagnostics;
using System.IO;

namespace MetricUnits
{
    /// <summary>
    /// Uses 
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
        private string? exec(string arguments)
        {
            using (Process ckconv = new())
            {
                ckconv.StartInfo.FileName = executable_name;
                ckconv.StartInfo.Arguments = arguments;
                ckconv.StartInfo.UseShellExecute = false;
                ckconv.StartInfo.RedirectStandardOutput = true;
                ckconv.Start();
                return ckconv.StandardOutput.ReadLine(); // read until newline
            }
        }

        public string GetVersion(bool include_name = true)
        {
            return exec($"-v{(include_name ? "" : "q")}") ?? "[NULL]";
        }

        public string Convert(string input_value, string input_unit, string output_unit)
        {
            var result = exec($"-nq {input_unit} {input_value} {output_unit}");
            if (result == null)
                return input_value;
            return result;
        }
    }
}
