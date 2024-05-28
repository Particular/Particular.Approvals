namespace Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using Particular.Approvals;

    [TestFixture]
    class ApiCasingProblems
    {
        [TestCase("Feedback_on_casing_problems")]
        [TestCase("Feedback_ON_casing_PROBLEMS")]
        public void Feedback_on_casing_problems(string overrideMemberName)
        {
            var text = "Text to approve";

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    var exception = Assert.Throws<Exception>(() => Approver.Verify(text, callerMemberName: overrideMemberName));
                    Assert.That(exception.Message, Contains.Substring("case-insensitive match"));
                }
                else
                {
                    // Windows and MacOS are case insensitive
                    Approver.Verify(text, callerMemberName: overrideMemberName);
                }
            }
            finally
            {
                // File must be cleaned up or will ruin other runtime tests
                var approvalFilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "ApprovalFiles");
                var pattern = $"{nameof(ApiCasingProblems)}.{nameof(Feedback_on_casing_problems)}.approved.txt";
                var directory = new DirectoryInfo(approvalFilesPath);
                var matchingFiles = directory.GetFiles()
                    .Where(f => f.Name.Equals(pattern, StringComparison.OrdinalIgnoreCase) && !f.Name.Equals("APICasingProblems.Feedback_on_casing_problems.approved.txt", StringComparison.Ordinal));

                foreach (var file in matchingFiles)
                {
                    file.Delete();
                }
            }
        }
    }
}
