namespace Particular.Approvals
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

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
        /// <param name="callerClassName"></param>
        /// <param name="callerMethodName"></param>
        public static void Verify(string text, [CallerFilePath] string callerClassName = null, [CallerMemberName] string callerMethodName = null)
        {
            VerifyInternal(text, callerClassName, callerMethodName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callerClassName"></param>
        /// <param name="callerMethodName"></param>
        public static void Verify(object data, [CallerFilePath] string callerClassName = null, [CallerMemberName] string callerMethodName = null)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            settings.Converters.Add(new StringEnumConverter());

            var json = JsonConvert.SerializeObject(data, settings);

            VerifyInternal(json, callerClassName, callerMethodName);
        }

        static void VerifyInternal(string text, string callerClassName, string callerMethodName)
        {
            callerClassName = Path.GetFileNameWithoutExtension(callerClassName);

            var receivedFile = Path.Combine(approvalFilesPath, $"{callerClassName}.{callerMethodName}.received.txt");
            File.WriteAllText(receivedFile, text);

            var approvedFile = Path.Combine(approvalFilesPath, $"{callerClassName}.{callerMethodName}.approved.txt");
            var approvedText = File.ReadAllText(approvedFile);

            var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
            var normalizedReceivedText = text.Replace("\r\n", "\n");

            Assert.AreEqual(normalizedApprovedText, normalizedReceivedText);

            File.Delete(receivedFile);
        }
    }
}
