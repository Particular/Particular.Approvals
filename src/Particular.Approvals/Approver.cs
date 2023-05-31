﻿namespace Particular.Approvals
{
    using System;
    using System.IO;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using NUnit.Framework;

    /// <summary>
    /// Verifies that values contain approved content.
    /// </summary>
    public static class Approver
    {
        static readonly string approvalFilesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "ApprovalFiles");
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
        public static void Verify(string value, Func<string, string> scrubber = null, string scenario = null)
        {
            var parts = TestContext.CurrentContext.Test.ClassName.Split('.');
            var className = parts[parts.Length - 1];
            var methodName = TestContext.CurrentContext.Test.MethodName;
            var scenarioName = string.IsNullOrEmpty(scenario) ? "" : scenario + ".";

            if (scrubber != null)
            {
                value = scrubber(value);
            }

            var receivedFile = Path.Combine(approvalFilesPath, $"{className}.{methodName}.{scenarioName}received.txt");
            File.WriteAllText(receivedFile, value);

            var approvedFile = Path.Combine(approvalFilesPath, $"{className}.{methodName}.{scenarioName}approved.txt");
            if (!File.Exists(approvedFile))
            {
                File.WriteAllText(approvedFile, string.Empty);
            }
            var approvedText = File.ReadAllText(approvedFile);

            var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
            var normalizedReceivedText = value.Replace("\r\n", "\n");

            Assert.AreEqual(normalizedApprovedText, normalizedReceivedText, "Approval verification failed.");

            File.Delete(receivedFile);
        }

        /// <summary>
        /// Verifies that the received object, after it has been serialized, matches the contents of the corresponding approval file.
        /// </summary>
        /// <param name="value">The object to verify.</param>
        /// <param name="scrubber">A delegate that modifies the received object, after it has been serialized, before comparing it to the approval file.</param>
        /// <param name="scenario">A value that will be added to the name of the approval file.</param>
        public static void Verify(object value, Func<string, string> scrubber = null, string scenario = null)
        {
            var json = JsonSerializer.Serialize(value, value.GetType(), jsonSerializerOptions);

            Verify(json, scrubber, scenario);
        }
    }
}
