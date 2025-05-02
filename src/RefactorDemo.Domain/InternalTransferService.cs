using RefactorDemo.Adapters;

namespace RefactorDemo.Domain
{
    public class InternalTransferService
    {
        private readonly AccountRepository _accountRepository;
        private readonly List<InternalTransferRequest> _history = new();
        public InternalTransferService(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public InternalTransferResult Transfer(InternalTransferRequest request)
        {
            if (!request.IsValid(out var validationError))
            {
                return new InternalTransferResult
                {
                    Success = false,
                    FailureReason = validationError,
                    TransferId = request.TransferId
                };
            }
            if (!_accountRepository.Debit(request.SourceAccountId, request.Amount))
            {
                return new InternalTransferResult
                {
                    Success = false,
                    FailureReason = "Insufficient funds",
                    TransferId = request.TransferId
                };
            }
            _accountRepository.Credit(request.DestinationAccountId, request.Amount);
            _history.Add(request);
            return new InternalTransferResult
            {
                Success = true,
                FailureReason = null,
                TransferId = request.TransferId
            };
        }
        public IReadOnlyList<InternalTransferRequest> GetTransferHistory() => _history.AsReadOnly();
        public bool ReverseTransfer(string transferId)
        {
            var original = _history.FirstOrDefault(x => x.TransferId == transferId);
            if (original == null) return false;
            if (!_accountRepository.Debit(original.DestinationAccountId, original.Amount)) return false;
            _accountRepository.Credit(original.SourceAccountId, original.Amount);
            return true;
        }
        public decimal GetAccountBalance(string accountId) => _accountRepository.GetBalance(accountId);
    }
}
