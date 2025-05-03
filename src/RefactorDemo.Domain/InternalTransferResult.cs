namespace RefactorDemo.Domain
{
    public class InternalTransferResult
    {
        public bool Success { get; set; }
        public string? FailureReason { get; set; }
        public string TransferId { get; set; } = string.Empty;
    }
}
