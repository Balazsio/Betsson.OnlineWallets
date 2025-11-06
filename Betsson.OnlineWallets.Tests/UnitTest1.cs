using System.Net;
using Microsoft.Testing.Extensions.Telemetry;
using Newtonsoft.Json.Linq;
using RestSharp;
using static System.Net.WebRequestMethods;

namespace Betsson.OnlineWallets.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        #region Endpoint testing basics. OK responses, positive path.

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
                .AddJsonBody(new Dictionary<string, double> {{"amount" , 50 }});

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

            Console.WriteLine("*Your balance before withdraw:  "+currentBalance);
            
            //Withdraw more than current balance
            client = new RestClient(TestsEndpoints.postWithdrawUrl);
            restRequest = new RestRequest(TestsEndpoints.postWithdrawUrl, Method.Post)
                .AddJsonBody(new Dictionary<string, double> { { "amount", moreThanCurrentBalance } });
            
            restResponse = client.Execute(restRequest);

            ////Assert: validate responses           
            Assert.That(restResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(restResponse.Content, Does.Contain("InsufficientBalanceException"));
            
        }



    }

}