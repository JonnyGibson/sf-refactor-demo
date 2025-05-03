namespace RefactorDemo.Domain
{
    public class InternalTransferRequest
    {
        public string SourceAccountId { get; set; } = string.Empty;
        public string DestinationAccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransferId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        public bool IsValid(out string validationError)
        {
            validationError = string.Empty;
            
            if (string.IsNullOrWhiteSpace(SourceAccountId))
            {
                validationError = "SourceAccountId is required.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(DestinationAccountId))
            {
                validationError = "DestinationAccountId is required.";
                return false;
            }
            if (Amount <= 0)
            {
                validationError = "Amount must be positive.";
                return false;
            }
            if (SourceAccountId == DestinationAccountId)
            {
                validationError = "Source and destination accounts must be different.";
                return false;
            }
            return true;
        }
    }
}
