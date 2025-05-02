using RefactorDemo.Adapters;
using Xunit;

namespace RefactorDemo.Adapters.Tests
{
    public class AccountRepositoryTests
    {
        [Fact]
        public void Debit_Fails_When_InsufficientFunds()
        {
            var repo = new AccountRepository();
            Assert.False(repo.Debit("A", 99999));
        }

        [Fact]
        public void Credit_Increases_Balance()
        {
            var repo = new AccountRepository();
            repo.Credit("A", 100);
            Assert.Equal(1100, repo.GetBalance("A"));
        }
    }
}
