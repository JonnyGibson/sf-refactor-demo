namespace RefactorDemo.Domain
{
    public class InternalTransferRequest
    {
        public string SourceAccountId { get; set; }
        public string DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransferId { get; set; }
        public DateTime Timestamp { get; set; }

        public bool IsValid(out string validationError)
        {
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
            validationError = null;
            return true;
        }
    }
}
