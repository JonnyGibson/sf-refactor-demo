using System;
using RefactorDemo.Domain;
using RefactorDemo.Adapters;
using Xunit;

namespace RefactorDemo.ServiceFabric.InternalTransfer.Tests
{
    public class InternalTransferServiceTests
    {
        [Fact]
        public void Transfer_Succeeds_WhenFundsAreSufficient()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo);
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
            var service = new InternalTransferService(repo);
            Assert.Equal(1000, service.GetAccountBalance("A"));
            Assert.Equal(500, service.GetAccountBalance("B"));
        }
    }
}
