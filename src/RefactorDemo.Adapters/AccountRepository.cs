using System.Collections.Generic;

namespace RefactorDemo.Adapters
{
    public class AccountRepository
    {
        private readonly Dictionary<string, decimal> _accounts = new();
        public AccountRepository()
        {
            // Demo accounts
            _accounts["A"] = 1000m;
            _accounts["B"] = 500m;
        }
        public bool Debit(string accountId, decimal amount)
        {
            if (!_accounts.ContainsKey(accountId) || _accounts[accountId] < amount)
                return false;
            _accounts[accountId] -= amount;
            return true;
        }
        public void Credit(string accountId, decimal amount)
        {
            if (!_accounts.ContainsKey(accountId))
                _accounts[accountId] = 0m;
            _accounts[accountId] += amount;
        }
        public decimal GetBalance(string accountId)
        {
            return _accounts.TryGetValue(accountId, out var bal) ? bal : 0m;
        }
    }
}
