namespace Betsson.OnlineWallets.Tests
{
    internal static class TestsEndpoints
    {
        public const string baseUrl = "http://localhost:8080/onlinewallet/";

        public const string getBalanceUrl = baseUrl + "balance";
        public const string postDepositUrl = baseUrl + "deposit";
        public const string postWithdrawUrl = baseUrl + "withdraw";
    }
}