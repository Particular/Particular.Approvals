# Particular.Approvals

This repo contains out standardized way of using approval tests to verify and highlight any changes to public API's.

## Usage

1. Remove `ApprovalTests`, `ApprovalUtilities`, and `ObjectApproval` packages
1. Add `Particular.Approvals` package
1. Update `PublicApiGenerator` to latest version
1. Remove `<Optimize>False</Optimize>` from test project file
1. Remove any `[MethodImpl(MethodImplOptions.NoInlining)]` from tests using approvals
1. Update tests to use `Approver.Verify()`
1. Move all approval files into an ApprovalFiles folder in the root of the test project
1. Remove `ApprovalTestConfig.cs`, `TestApprover.cs`, etc (anything directly referencing ApprovalTests)

After conversion, an API approval test should look identical to the following (other than adjusting the specific type, of course): https://github.com/Particular/NServiceBus.AzureStorageQueues/blob/develop/src/Tests/APIApprovals.cs

If a project isn't multi-targeted you could skip the `excludeAttributes`, but it wouldn't hurt to have it regardless.


