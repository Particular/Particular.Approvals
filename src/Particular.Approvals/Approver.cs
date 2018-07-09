namespace Particular.Approvals
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using NUnit.Framework;

    /// <summary>
    ///
    /// </summary>
    public static class Approver
    {
        static readonly string approvalFilesPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "ApprovalFiles");

        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="scrubber"></param>
        /// <param name="category"></param>
        public static void Verify(string text, Func<string, string> scrubber = null, string category = null)
        {
            var parts = TestContext.CurrentContext.Test.ClassName.Split('.');
            var className = parts[parts.Length - 1];
            var methodName = TestContext.CurrentContext.Test.MethodName;
            var categoryName = string.IsNullOrEmpty(category) ? "" : category + ".";

            if (scrubber != null)
            {
                text = scrubber(text);
            }

            var receivedFile = Path.Combine(approvalFilesPath, $"{className}.{methodName}.{categoryName}received.txt");
            File.WriteAllText(receivedFile, text);

            var approvedFile = Path.Combine(approvalFilesPath, $"{className}.{methodName}.{categoryName}approved.txt");
            var approvedText = File.ReadAllText(approvedFile);

            if (scrubber != null)
            {
                approvedText = scrubber(approvedText);
            }

            var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
            var normalizedReceivedText = text.Replace("\r\n", "\n");

            Assert.AreEqual(normalizedApprovedText, normalizedReceivedText);

            File.Delete(receivedFile);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="scrubber"></param>
        /// <param name="category"></param>
        public static void Verify(object data, Func<string, string> scrubber = null, string category = null)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            settings.Converters.Add(new StringEnumConverter());

            var json = JsonConvert.SerializeObject(data, settings);

            Verify(json, scrubber, category);
        }
    }
}
