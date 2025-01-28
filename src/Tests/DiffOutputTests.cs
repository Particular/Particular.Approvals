namespace Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;
    using Particular.Approvals;

    [TestFixture]
    class DiffOutputTests
    {
        static readonly string approvalFilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", "..", "ApprovalFiles");

        [Test]
        public void ShortTest() => Test("I am the very NOPE of a modern major general.");

        [Test]
        public void LongWithNewlineCharacters()
        {
            var text = @"Lorem ipsum dolor sit amet.
Consectetur adipiscing elit.
Aliquam et arcu at lacus tincidunt tempus.
Aliquam ultricies odio sit amet.
Velit convallis, sit amet maximus nisi rhoncus.
Phasellus eu quam gravida.
Euismod arcu non, posuere metus.
Maecenas sed nisi sed magna consectetur bibendum.
Sed non lectus ornare. iaculis magna et, laoreet ipsum.
Aenean imperdiet enim et justo iaculis cursus.
---PROBLEM HERE---
Aenean dictum urna sed egestas ultrices.
Duis lobortis massa quis enim suscipit pellentesque porta sodales mi.
Fusce eget mi eget ex consequat sagittis.
Sed lobortis ligula nec rhoncus luctus.
Curabitur vel nunc vel dolor feugiat lacinia id vitae erat.
Duis pulvinar metus at lacus consequat posuere.
Nullam non libero sit amet tortor imperdiet hendrerit.
Curabitur id nisi feugiat, scelerisque leo vitae, euismod metus.
Sed ornare massa sit amet nulla pharetra, a commodo turpis maximus.
Aenean et orci in mi scelerisque tristique non in quam.
";

            Test(text);
        }

        [Test]
        public void LongWithProblemAtTheEnd()
        {
            var text = @"Lorem ipsum dolor sit amet.
Consectetur adipiscing elit.
Aliquam et arcu at lacus tincidunt tempus.
Aliquam ultricies odio sit amet.
Velit convallis, sit amet maximus nisi rhoncus.
Phasellus eu quam gravida.
Euismod arcu non, posuere metus.
Maecenas sed nisi sed magna consectetur bibendum.
Sed non lectus ornare. iaculis magna et, laoreet ipsum.
Aenean imperdiet enim et justo iaculis cursus.
Aenean dictum urna sed egestas ultrices.
Duis lobortis massa quis enim suscipit pellentesque porta sodales mi.
Fusce eget mi eget ex consequat sagittis.
Sed lobortis ligula nec rhoncus luctus.
Curabitur vel nunc vel dolor feugiat lacinia id vitae erat.
Duis pulvinar metus at lacus consequat posuere.
Nullam non libero sit amet tortor imperdiet hendrerit.
Curabitur id nisi feugiat, scelerisque leo vitae, euismod metus.
Sed ornare massa sit amet nulla pharetra, a commodo turpis maximus.
Aenean et orci in mi scelerisque tristique non in quam.
---PROBLEM HERE---
";
            Test(text);
        }

        void Test(string text, [CallerMemberName] string callerMemberName = null)
        {
            var exception = Assert.Throws<Exception>(() => Approver.Verify(text, scenario: "OriginalTest", callerMemberName: callerMemberName));
            var originalTestFile = Path.Combine(approvalFilesPath, $"{nameof(DiffOutputTests)}.{callerMemberName}.OriginalTest.received.txt");
            File.Delete(originalTestFile);
            Approver.Verify(exception.Message, scenario: "ExceptionText", callerMemberName: callerMemberName);
        }
    }
}
