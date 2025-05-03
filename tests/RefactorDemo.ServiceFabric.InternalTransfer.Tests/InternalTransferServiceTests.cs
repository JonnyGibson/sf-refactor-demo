using System;
using System.Fabric;
using Moq;
using RefactorDemo.Domain;
using RefactorDemo.Adapters;
using Xunit;

namespace RefactorDemo.ServiceFabric.InternalTransfer.Tests
{
    public class ServiceFabricInternalTransferTests
    {
        private StatelessServiceContext CreateMockContext()
        {
            var nodeContext = new NodeContext("Node1", new NodeId(1, 1), 0, "Type1", "1.0");
            var mockActivationContext = new Mock<ICodePackageActivationContext>();
            mockActivationContext.Setup(m => m.ApplicationName).Returns("fabric:/TestApp");
            mockActivationContext.Setup(m => m.ApplicationTypeName).Returns("TestAppType");
            mockActivationContext.Setup(m => m.CodePackageName).Returns("Code");
            mockActivationContext.Setup(m => m.CodePackageVersion).Returns("1.0");
            mockActivationContext.Setup(m => m.ContextId).Returns("TestContext");
            mockActivationContext.Setup(m => m.LogDirectory).Returns("/tmp/log");
            mockActivationContext.Setup(m => m.TempDirectory).Returns("/tmp/temp");
            mockActivationContext.Setup(m => m.WorkDirectory).Returns("/tmp/work");

            return new StatelessServiceContext(
                nodeContext,
                mockActivationContext.Object,
                "ServiceType",
                new Uri("fabric:/TestApp/TestService"),
                null,
                Guid.NewGuid(),
                long.MaxValue);
        }

        [Fact]
        public void Transfer_Succeeds_WhenFundsAreSufficient()
        {
            var repo = new AccountRepository();
            var context = CreateMockContext();
            var service = new InternalTransferService(repo, context);
            var request = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 100m,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = service.Transfer(request);
            Assert.True(result.Success);
            Assert.Null(result.FailureReason);
            Assert.Equal(900m, repo.GetBalance("A"));
            Assert.Equal(600m, repo.GetBalance("B"));
        }

        [Fact]
        public void GetAccountBalance_Works()
        {
            var repo = new AccountRepository();
            var context = CreateMockContext();
            var service = new InternalTransferService(repo, context);
            Assert.Equal(1000, service.GetAccountBalance("A"));
            Assert.Equal(500, service.GetAccountBalance("B"));
        }
    }
}
