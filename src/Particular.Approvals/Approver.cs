namespace Particular.Approvals
{
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
        public static void Verify(string text)
        {
            var parts = TestContext.CurrentContext.Test.ClassName.Split('.');
            var className = parts[parts.Length - 1];
            var methodName = TestContext.CurrentContext.Test.MethodName;

            var receivedFile = Path.Combine(approvalFilesPath, $"{className}.{methodName}.received.txt");
            File.WriteAllText(receivedFile, text);

            var approvedFile = Path.Combine(approvalFilesPath, $"{className}.{methodName}.approved.txt");
            var approvedText = File.ReadAllText(approvedFile);

            var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
            var normalizedReceivedText = text.Replace("\r\n", "\n");

            Assert.AreEqual(normalizedApprovedText, normalizedReceivedText);

            File.Delete(receivedFile);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        public static void Verify(object data)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            settings.Converters.Add(new StringEnumConverter());

            var json = JsonConvert.SerializeObject(data, settings);

            Verify(json);
        }
    }
}
