using System;
using RefactorDemo.Domain;
using Xunit;

namespace RefactorDemo.Domain.Tests
{
    public class InternalTransferRequestTests
    {
        [Theory]
        [InlineData(null, "B", 100, false)]
        [InlineData("A", null, 100, false)]
        [InlineData("A", "B", 0, false)]
        [InlineData("A", "A", 100, false)]
        [InlineData("A", "B", 100, true)]
        public void IsValid_ValidatesCorrectly(string src, string dest, decimal amt, bool expected)
        {
            var req = new InternalTransferRequest
            {
                SourceAccountId = src,
                DestinationAccountId = dest,
                Amount = amt,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = req.IsValid(out var _);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CanSerializeAndDeserializeRequest()
        {
            var req = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 123.45m,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var json = System.Text.Json.JsonSerializer.Serialize(req);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<InternalTransferRequest>(json);
            Assert.Equal(req.SourceAccountId, deserialized.SourceAccountId);
            Assert.Equal(req.DestinationAccountId, deserialized.DestinationAccountId);
            Assert.Equal(req.Amount, deserialized.Amount);
            Assert.Equal(req.TransferId, deserialized.TransferId);
        }
    }

    public class InternalTransferServiceTests
    {
        [Fact]
        public void Transfer_ReturnsValidationError()
        {
            var repo = new RefactorDemo.Adapters.AccountRepository();
            var service = new InternalTransferService(repo);
            var req = new InternalTransferRequest
            {
                SourceAccountId = null,
                DestinationAccountId = "B",
                Amount = 100,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = service.Transfer(req);
            Assert.False(result.Success);
            Assert.Equal("SourceAccountId is required.", result.FailureReason);
        }
    }
}
