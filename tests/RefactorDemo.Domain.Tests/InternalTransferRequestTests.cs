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
                SourceAccountId = src ?? string.Empty,
                DestinationAccountId = dest ?? string.Empty,
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
            Assert.NotNull(deserialized);
            Assert.Equal(req.SourceAccountId, deserialized.SourceAccountId);
            Assert.Equal(req.DestinationAccountId, deserialized.DestinationAccountId);
            Assert.Equal(req.Amount, deserialized.Amount);
            Assert.Equal(req.TransferId, deserialized.TransferId);
        }
    }
}
