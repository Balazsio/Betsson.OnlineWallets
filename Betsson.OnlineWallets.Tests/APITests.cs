using System.Net;
using Microsoft.Testing.Extensions.Telemetry;
using Newtonsoft.Json.Linq;
using RestSharp;
using static System.Net.WebRequestMethods;

namespace Betsson.OnlineWallets.Tests
{
    public class APITests
    {
        [SetUp]
        public void Setup()
        {
        }

        #region Endpoint tests

        [Test]
        public void GetBalance_OK_Response()
        {
            //Arrange: setup all the information to do the request            
            RestClient client = new RestClient(TestsEndpoints.getBalanceUrl);
            RestRequest restRequest = new RestRequest(TestsEndpoints.getBalanceUrl, Method.Get);

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses
            Assert.That(restResponse, Is.Not.Null);
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void Deposit_OK_Response()
        {
            //Arrange: setup all the information to do the request  
            RestClient client = new RestClient(TestsEndpoints.postDepositUrl);
            var restRequest = new RestRequest(TestsEndpoints.postDepositUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", 50 } });

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses
            Assert.That(restResponse, Is.Not.Null);
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        }

        [Test]
        public void Withdraw_OK_Response()
        {
            //Arrange: setup all the information to do the request  
            RestClient client = new RestClient(TestsEndpoints.postWithdrawUrl);
            RestRequest restRequest = new RestRequest(TestsEndpoints.postWithdrawUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", 50 } });

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses
            Assert.That(restResponse, Is.Not.Null);
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        #endregion

        #region End2End tests 

        [Test]
        public void DepositAmount_BalanceEqualsToTheSumOfDepositPlusBalance()
        {
            //get initial balance
            var client = new RestClient(TestsEndpoints.getBalanceUrl);
            var restRequest = new RestRequest(TestsEndpoints.getBalanceUrl, Method.Get);
            RestResponse restResponse = client.Execute(restRequest);

            dynamic data = JObject.Parse(restResponse.Content);
            double balanceBeforeDeposit = data.amount;
            double depositAmount = 100;

            //deposit
            client = new RestClient(TestsEndpoints.postDepositUrl);
            restRequest = new RestRequest(TestsEndpoints.postDepositUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", depositAmount } });
            restResponse = client.Execute(restRequest);


            double expectedBalanceAfterDeposit = balanceBeforeDeposit + depositAmount;

            //get balance after deposit
            client = new RestClient(TestsEndpoints.getBalanceUrl);
            restRequest = new RestRequest(TestsEndpoints.getBalanceUrl, Method.Get);
            restResponse = client.Execute(restRequest);

            data = JObject.Parse(restResponse.Content);
            double balanceAfterDeposit = data.amount;

            //Assert: new balance = deposit + balance            
            Assert.That(expectedBalanceAfterDeposit.Equals(balanceAfterDeposit));
        }

        [Test]
        public void WithdrawAmount_BalanceEqualsToTheOldBalanceMinusWithdraw()
        {
            double depositAmount = 100;

            //deposit 100 so balance is not zero
            var client = new RestClient(TestsEndpoints.postDepositUrl);
            var restRequest = new RestRequest(TestsEndpoints.postDepositUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", depositAmount } });
            var restResponse = client.Execute(restRequest);

            //get balance after deposit
            client = new RestClient(TestsEndpoints.getBalanceUrl);
            restRequest = new RestRequest(TestsEndpoints.getBalanceUrl, Method.Get);
            restResponse = client.Execute(restRequest);

            dynamic data = JObject.Parse(restResponse.Content);
            double balanceAfterDeposit = data.amount;

            //withdraw
            double withdrawAmount = 50;

            client = new RestClient(TestsEndpoints.postWithdrawUrl);
            restRequest = new RestRequest(TestsEndpoints.postWithdrawUrl, Method.Post)
               .AddJsonBody(new Dictionary<string, double> { { "amount", withdrawAmount } });

            restResponse = client.Execute(restRequest);

            double expectedCurrentbalance = balanceAfterDeposit - withdrawAmount;

            //get balance after withdraw
            client = new RestClient(TestsEndpoints.getBalanceUrl);
            restRequest = new RestRequest(TestsEndpoints.getBalanceUrl, Method.Get);
            restResponse = client.Execute(restRequest);

            data = JObject.Parse(restResponse.Content);
            double balanceAfterWithdraw = data.amount;

            Assert.That(expectedCurrentbalance.Equals(balanceAfterWithdraw));

        }


        [Test]
        public void WithdrawMoreThanBalance_ThrowsInsufficientBalanceException()
        {
            //get balance
            var client = new RestClient(TestsEndpoints.getBalanceUrl);
            var restRequest = new RestRequest(TestsEndpoints.getBalanceUrl, Method.Get);
            RestResponse restResponse = client.Execute(restRequest);

            dynamic data = JObject.Parse(restResponse.Content);
            double currentBalance = data.amount;
            double moreThanCurrentBalance = currentBalance + 1;

            Console.WriteLine("*Your balance before withdraw:  " + currentBalance);

            //Withdraw more than current balance
            client = new RestClient(TestsEndpoints.postWithdrawUrl);
            restRequest = new RestRequest(TestsEndpoints.postWithdrawUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", moreThanCurrentBalance } });

            restResponse = client.Execute(restRequest);

            ////Assert: validate responses           
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(restResponse.Content, Does.Contain("InsufficientBalanceException"));
        }

        [Test]
        public void WithdrawNegativeAmount_BadRequest()
        {
            //Arrange: setup all the information to do the request  
            RestClient client = new RestClient(TestsEndpoints.postWithdrawUrl);
            RestRequest restRequest = new RestRequest(TestsEndpoints.postWithdrawUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", -50 } });

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses            
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(restResponse.Content, Does.Contain("'Amount' must be greater than or equal to '0'"));
        }

        [Test]
        public void WithdrawInvalidAmount_BadRequest()
        {
            //Arrange: setup all the information to do the request  
            RestClient client = new RestClient(TestsEndpoints.postWithdrawUrl);
            RestRequest restRequest = new RestRequest(TestsEndpoints.postWithdrawUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, string> { { "amount", "b" } });

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses            
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(restResponse.Content, Does.Contain("One or more validation errors occurred"));
        }

        [Test]
        public void DepositNegativeAmount_BadRequest()
        {

            //Arrange: setup all the information to do the request  
            RestClient client = new RestClient(TestsEndpoints.postDepositUrl);
            var restRequest = new RestRequest(TestsEndpoints.postDepositUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", -50 } });

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(restResponse.Content, Does.Contain("'Amount' must be greater than or equal to '0'"));

        }

        [Test]
        public void DepositInvalidAmount_BadRequest()
        {
            //Arrange: setup all the information to do the request  
            RestClient client = new RestClient(TestsEndpoints.postDepositUrl);
            var restRequest = new RestRequest(TestsEndpoints.postDepositUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, string> { { "amount", "b" } });

            //Act: make the request
            RestResponse restResponse = client.Execute(restRequest);

            //Assert: validate responses
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(restResponse.Content, Does.Contain("One or more validation errors occurred"));
        }
        #endregion


    }

}