using RefactorDemo.Adapters;
using System.Fabric;
using System.Fabric.Health;

namespace RefactorDemo.Domain
{
    public interface IHealthReporter
    {
        void ReportHealth(string property, HealthState state, string description);
    }

    public class ServiceFabricHealthReporter : IHealthReporter
    {
        private readonly StatelessServiceContext _serviceContext;

        public ServiceFabricHealthReporter(StatelessServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
        }

        public void ReportHealth(string property, HealthState state, string description)
        {
            try
            {
                var healthInformation = new HealthInformation("InternalTransferService", property, state)
                {
                    Description = description,
                    TimeToLive = TimeSpan.FromMinutes(5),
                    RemoveWhenExpired = true
                };

                var healthReport = new StatelessServiceInstanceHealthReport(
                    _serviceContext.PartitionId,
                    _serviceContext.InstanceId,
                    healthInformation);

                var fabricClient = new FabricClient();
                fabricClient.HealthManager.ReportHealth(healthReport);
            }
            catch (Exception)
            {
                // Swallow exceptions in test environment
            }
        }
    }

    public class TestHealthReporter : IHealthReporter
    {
        public List<(string Property, HealthState State, string Description)> Reports { get; } = new();

        public void ReportHealth(string property, HealthState state, string description)
        {
            Reports.Add((property, state, description));
        }
    }

    public class InternalTransferService
    {
        private readonly AccountRepository _accountRepository;
        private readonly List<InternalTransferRequest> _history = new();
        private readonly IHealthReporter _healthReporter;

        public InternalTransferService(AccountRepository accountRepository, StatelessServiceContext serviceContext)
        {
            _accountRepository = accountRepository;
            _healthReporter = new ServiceFabricHealthReporter(serviceContext);
        }

        public InternalTransferService(AccountRepository accountRepository, IHealthReporter healthReporter)
        {
            _accountRepository = accountRepository;
            _healthReporter = healthReporter;
        }

        public InternalTransferResult Transfer(InternalTransferRequest request)
        {
            if (!request.IsValid(out var validationError))
            {
                _healthReporter.ReportHealth("ValidationError", HealthState.Warning, validationError);
                return new InternalTransferResult
                {
                    Success = false,
                    FailureReason = validationError,
                    TransferId = request.TransferId
                };
            }

            if (!_accountRepository.Debit(request.SourceAccountId, request.Amount))
            {
                _healthReporter.ReportHealth("InsufficientFunds", HealthState.Warning, "Insufficient funds for transfer.");
                return new InternalTransferResult
                {
                    Success = false,
                    FailureReason = "Insufficient funds",
                    TransferId = request.TransferId
                };
            }

            _accountRepository.Credit(request.DestinationAccountId, request.Amount);
            _history.Add(request);
            _healthReporter.ReportHealth("TransferSuccess", HealthState.Ok, "Transfer completed successfully.");

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
            if (original == null)
            {
                _healthReporter.ReportHealth("ReverseTransferError", HealthState.Warning, "Original transfer not found.");
                return false;
            }

            if (!_accountRepository.Debit(original.DestinationAccountId, original.Amount))
            {
                _healthReporter.ReportHealth("ReverseTransferError", HealthState.Warning, "Insufficient funds to reverse transfer.");
                return false;
            }

            _accountRepository.Credit(original.SourceAccountId, original.Amount);
            _healthReporter.ReportHealth("ReverseTransferSuccess", HealthState.Ok, "Transfer reversed successfully.");
            return true;
        }

        public decimal GetAccountBalance(string accountId) => _accountRepository.GetBalance(accountId);
    }
}
