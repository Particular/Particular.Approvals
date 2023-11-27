namespace Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Particular.Approvals;

    [TestFixture]
    class ApproverTests
    {
        [Test]
        public void Can_Approve_Text()
        {
            Approver.Verify("Text to\r\napprove");
        }

        [Test]
        public void Can_Approve_Text_With_Scenario()
        {
            Approver.Verify("Text to\r\napprove", scenario: "test");
        }

        [Test]
        public void Can_Approve_Text_With_Scrubber()
        {
            Approver.Verify("Text to approve", s => s.Replace("approve", "replace"));
        }

        [Test]
        public void Can_Approve_Object()
        {
            var sample = new Sample { Value1 = "Value", Value2 = 42 };

            Approver.Verify(sample);
        }

        [Test]
        public void Can_Approve_Object_With_Scenario()
        {
            var sample = new Sample { Value1 = "Value", Value2 = 42 };

            Approver.Verify(sample, scenario: "test");
        }

        [Test]
        public void Can_Approve_Object_With_Scrubber()
        {
            var sample = new Sample { Value1 = "Value", Value2 = 42 };

            Approver.Verify(sample, s => s.Replace("42", "100"));
        }

        [Test]
        public void Can_process_complex_character_encodings_without_escaping()
        {
            var sample = new Sample { Value1 = "Foo => Bar // <br />" };

            Approver.Verify(sample);
        }

        [Test]
        public void Can_handle_missing_approval_file_for_new_test()
        {
            var sample = "Foo";

            var name = $"{nameof(ApproverTests)}.{nameof(Can_handle_missing_approval_file_for_new_test)}";

            var approved = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "ApprovalFiles", $"{name}.approved.txt");
            var received = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "ApprovalFiles", $"{name}.received.txt");

            // Delete approval file if it exists from previous test run
            if (File.Exists(approved))
            {
                File.Delete(approved);
            }

            var exception = Assert.Throws<Exception>(() => Approver.Verify(sample));

            Assert.Multiple(() =>
            {
                Assert.That(exception.Message, Contains.Substring("Approval verification failed"));

                Assert.That(File.Exists(approved));
                Assert.That(File.ReadAllText(approved), Is.Empty);
            });

            File.Delete(approved);
        }
    }

    class Sample
    {
        public string Value1 { get; set; }

        public int Value2 { get; set; }
    }
}
