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
        public void Can_Approve_Text_With_Category()
        {
            Approver.Verify("Text to\r\napprove", "test");
        }

        [Test]
        public void Can_Approve_Object()
        {
            var sample = new Sample { Value1 = "Value", Value2 = 42 };

            Approver.Verify(sample);
        }

        [Test]
        public void Can_Approve_Object_With_Category()
        {
            var sample = new Sample { Value1 = "Value", Value2 = 42 };

            Approver.Verify(sample, "test");
        }
    }

    class Sample
    {
        public string Value1 { get; set; }

        public int Value2 { get; set; }
    }
}
