using System;
using RefactorDemo.Domain;
using RefactorDemo.Adapters;

namespace RefactorDemo.ServiceFabric.InternalTransfer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var repo = new AccountRepository();
            var service = new InternalTransferService(repo);
            var request = new InternalTransferRequest
            {
                SourceAccountId = "A",
                DestinationAccountId = "B",
                Amount = 200m,
                TransferId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };
            var result = service.Transfer(request);
            Console.WriteLine(result.Success
                ? $"Transfer succeeded: {request.Amount} from {request.SourceAccountId} to {request.DestinationAccountId}"
                : $"Transfer failed: {result.FailureReason}");
        }
    }
}
