using System;
using System.Fabric;
using System.Fabric.Health;
using RefactorDemo.Domain;
using RefactorDemo.Adapters;
using Xunit;

namespace RefactorDemo.Domain.Tests
{
    public class InternalTransferServiceTests
    {
        private readonly TestHealthReporter _healthReporter = new();

        [Fact]
        public void Transfer_Succeeds_WhenFundsAreSufficient()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo, _healthReporter);

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
            Assert.Contains(_healthReporter.Reports, r => r.Property == "TransferSuccess" && r.State == HealthState.Ok);
        }

        [Fact]
        public void TransferHistory_Tracks_Transfers()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo, _healthReporter);
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
            var service = new InternalTransferService(repo, _healthReporter);
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
            var service = new InternalTransferService(repo, _healthReporter);
            Assert.Equal(1000, service.GetAccountBalance("A"));
            Assert.Equal(500, service.GetAccountBalance("B"));
        }

        [Fact]
        public void Transfer_ReturnsValidationError_WhenSourceAccountEmpty()
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo, _healthReporter);
            var req = new InternalTransferRequest
            {
                SourceAccountId = string.Empty,
                DestinationAccountId = "B",
                Amount = 100,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = service.Transfer(req);
            Assert.False(result.Success);
            Assert.Equal("SourceAccountId is required.", result.FailureReason);
            Assert.Contains(_healthReporter.Reports, r => r.Property == "ValidationError" && r.State == HealthState.Warning);
        }
    }
}
