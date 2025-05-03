using System;
using RefactorDemo.Domain;
using RefactorDemo.Adapters;
using Xunit;

namespace RefactorDemo.ComponentTests
{
    public class TransferHistoryComponentTests
    {
        private readonly TestHealthReporter _healthReporter = new();

        [Fact]
        public void TransferHistory_Records_Transfers()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo, _healthReporter);
            var req = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 50,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = service.Transfer(req);
            Assert.True(result.Success);
            Assert.Single(service.GetTransferHistory());
            Assert.Contains(_healthReporter.Reports, r => r.Property == "TransferSuccess" && r.State == System.Fabric.Health.HealthState.Ok);
        }

        [Fact]
        public void ReverseTransfer_EndToEnd()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo, _healthReporter);
            var req = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 25,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = service.Transfer(req);
            Assert.True(result.Success);
            var reversed = service.ReverseTransfer(req.TransferId);
            Assert.True(reversed);
            Assert.Equal(1000, service.GetAccountBalance("A"));
            Assert.Equal(500, service.GetAccountBalance("B"));
            Assert.Contains(_healthReporter.Reports, r => r.Property == "ReverseTransferSuccess" && r.State == System.Fabric.Health.HealthState.Ok);
        }
    }
}
