namespace Tests
{
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
    }

    class Sample
    {
        public string Value1 { get; set; }

        public int Value2 { get; set; }
    }
}
