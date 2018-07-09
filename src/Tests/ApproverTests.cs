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
    }
}
