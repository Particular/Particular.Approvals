namespace Particular.Approvals
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Verifies that values contain approved content.
    /// </summary>
    public static class Approver
    {
        static readonly string approvalFilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "ApprovalFiles");
        static readonly JsonSerializerOptions jsonSerializerOptions;

        static Approver()
        {
            jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        /// <summary>
        /// Verifies that the received string matches the contents of the corresponding approval file.
        /// </summary>
        /// <param name="value">The string to verify.</param>
        /// <param name="scrubber">A delegate that modifies the received string before comparing it to the approval file.</param>
        /// <param name="scenario">A value that will be added to the name of the approval file.</param>
        /// <param name="callerFilePath">Do not provide a value for this parameter. It is populated with the value from <see cref="CallerFilePathAttribute"/>.</param>
        /// <param name="callerMemberName">Do not provide a value for this parameter. It is populated with the value from <see cref="CallerMemberNameAttribute"/>.</param>
        public static void Verify(string value, Func<string, string> scrubber = null, string scenario = null, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
        {
            var fileName = Path.GetFileNameWithoutExtension(callerFilePath);
            var scenarioName = string.IsNullOrEmpty(scenario) ? "" : scenario + ".";

            if (scrubber != null)
            {
                value = scrubber(value);
            }

            var receivedFile = Path.Combine(approvalFilesPath, $"{fileName}.{callerMemberName}.{scenarioName}received.txt");
            File.WriteAllText(receivedFile, value);

            var approvedFile = Path.Combine(approvalFilesPath, $"{fileName}.{callerMemberName}.{scenarioName}approved.txt");

            var approvedExists = File.Exists(approvedFile);

            if (!approvedExists)
            {
                File.WriteAllText(approvedFile, string.Empty);
            }

            var approvedText = File.ReadAllText(approvedFile);

            var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
            var normalizedReceivedText = value.Replace("\r\n", "\n");

            if (!string.Equals(normalizedApprovedText, normalizedReceivedText))
            {
                if (!approvedExists)
                {
                    DetectCaseMismatches(approvedFile);
                }

                throw new Exception($"Approval verification failed.");
            }

            File.Delete(receivedFile);
        }

        static void DetectCaseMismatches(string approvedFile)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(approvedFile));
            if (directory.Exists)
            {
                var approvedFileName = Path.GetFileName(approvedFile);
                var matchFileByCase = directory.GetFiles()
                    .FirstOrDefault(file => file.Name.Equals(approvedFileName, StringComparison.OrdinalIgnoreCase) && !file.Name.Equals(approvedFileName, StringComparison.Ordinal));

                if (matchFileByCase != null)
                {
                    throw new Exception("Approval verification failed because the *.approved.txt file was not found, but another file in the directory is a case-insensitive match. If this error is occurring on a case-sensitive system (Linux) then ensure the casing of the test class file, approval file, and test class name, and test method name all match.");
                }
            }
        }

        /// <summary>
        /// Verifies that the received object, after it has been serialized, matches the contents of the corresponding approval file.
        /// </summary>
        /// <param name="value">The object to verify.</param>
        /// <param name="scrubber">A delegate that modifies the received object, after it has been serialized, before comparing it to the approval file.</param>
        /// <param name="scenario">A value that will be added to the name of the approval file.</param>
        /// <param name="callerFilePath">Do not provide a value for this parameter. It is populated with the value from <see cref="CallerFilePathAttribute"/>.</param>
        /// <param name="callerMemberName">Do not provide a value for this parameter. It is populated with the value from <see cref="CallerMemberNameAttribute"/>.</param>
        public static void Verify(object value, Func<string, string> scrubber = null, string scenario = null, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), jsonSerializerOptions);

            Verify(json, scrubber, scenario, callerFilePath, callerMemberName);
        }
    }
}
