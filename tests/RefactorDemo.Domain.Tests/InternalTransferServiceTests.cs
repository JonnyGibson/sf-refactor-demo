using System;
using RefactorDemo.Domain;
using RefactorDemo.Adapters;
using Xunit;

namespace RefactorDemo.Domain.Tests
{
    public class InternalTransferServiceFeatureTests
    {
        [Fact]
        public void TransferHistory_Tracks_Transfers()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo);
            var req = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 10,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            service.Transfer(req);
            Assert.Single(service.GetTransferHistory());
        }

        [Fact]
        public void ReverseTransfer_Reverses_Transfer()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo);
            var req = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 10,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            service.Transfer(req);
            var reversed = service.ReverseTransfer(req.TransferId);
            Assert.True(reversed);
            Assert.Equal(1000, service.GetAccountBalance("A"));
            Assert.Equal(500, service.GetAccountBalance("B"));
        }

        [Fact]
        public void GetAccountBalance_Returns_Correct_Balance()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo);
            Assert.Equal(1000, service.GetAccountBalance("A"));
            Assert.Equal(500, service.GetAccountBalance("B"));
        }
    }
}
