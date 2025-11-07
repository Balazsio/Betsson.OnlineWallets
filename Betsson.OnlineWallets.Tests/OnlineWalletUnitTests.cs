using AutoMapper;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Web.Controllers;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Web.Models;
using Microsoft.Extensions.Logging;

namespace Betsson.OnlineWallets.Tests;

[TestFixture]
public class OnlineWalletUnitTests
{
    private  Mock<ILogger<OnlineWalletController>> _loggerMock;
    private  Mock<IMapper> _mapperMock;
    private  Mock<IOnlineWalletService> _walletServiceMock;
    private  OnlineWalletController _controller;


    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<OnlineWalletController>>();
        _mapperMock = new Mock<IMapper>();
        _walletServiceMock = new Mock<IOnlineWalletService>();

        _controller = new OnlineWalletController(
            _loggerMock.Object,
            _mapperMock.Object,
            _walletServiceMock.Object
        );
    }

    [Test]
    public async Task Balance_ShouldReturnOkResult_WithMappedResponse()
    {
        // Arrange
        var balance = new Balance { Amount = 100 };
        var balanceResponse = new BalanceResponse { Amount = 100 };

        _walletServiceMock.Setup(s => s.GetBalanceAsync()).ReturnsAsync(balance);
        _mapperMock.Setup(m => m.Map<BalanceResponse>(balance)).Returns(balanceResponse);

        // Act
        var result = await _controller.Balance();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.InstanceOf<BalanceResponse>());
        var response = okResult.Value as BalanceResponse;
        Assert.That(response!.Amount, Is.EqualTo(100));

        _walletServiceMock.Verify(s => s.GetBalanceAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<BalanceResponse>(balance), Times.Once);
    }

    [Test]
    public async Task Deposit_ShouldReturnOkResult_WithMappedResponse()
    {
        // Arrange
        var request = new DepositRequest { Amount = 50 };
        var deposit = new Deposit { Amount = 50 };
        var balance = new Balance { Amount = 150 };
        var response = new BalanceResponse { Amount = 150 };

        _mapperMock.Setup(m => m.Map<Deposit>(request)).Returns(deposit);
        _walletServiceMock.Setup(s => s.DepositFundsAsync(deposit)).ReturnsAsync(balance);
        _mapperMock.Setup(m => m.Map<BalanceResponse>(balance)).Returns(response);

        // Act
        var result = await _controller.Deposit(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.InstanceOf<BalanceResponse>());
        var balanceResponse = okResult.Value as BalanceResponse;
        Assert.That(balanceResponse!.Amount, Is.EqualTo(150));

        _mapperMock.Verify(m => m.Map<Deposit>(request), Times.Once);
        _walletServiceMock.Verify(s => s.DepositFundsAsync(deposit), Times.Once);
        _mapperMock.Verify(m => m.Map<BalanceResponse>(balance), Times.Once);
    }

    [Test]
    public async Task Withdraw_ShouldReturnOkResult_WithMappedResponse()
    {
        // Arrange
        var request = new WithdrawalRequest { Amount = 20 };
        var withdrawal = new Withdrawal { Amount = 20 };
        var balance = new Balance { Amount = 80 };
        var response = new BalanceResponse { Amount = 80 };

        _mapperMock.Setup(m => m.Map<Withdrawal>(request)).Returns(withdrawal);
        _walletServiceMock.Setup(s => s.WithdrawFundsAsync(withdrawal)).ReturnsAsync(balance);
        _mapperMock.Setup(m => m.Map<BalanceResponse>(balance)).Returns(response);

        // Act
        var result = await _controller.Withdraw(request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.InstanceOf<BalanceResponse>());
        var balanceResponse = okResult.Value as BalanceResponse;
        Assert.That(balanceResponse!.Amount, Is.EqualTo(80));

        _mapperMock.Verify(m => m.Map<Withdrawal>(request), Times.Once);
        _walletServiceMock.Verify(s => s.WithdrawFundsAsync(withdrawal), Times.Once);
        _mapperMock.Verify(m => m.Map<BalanceResponse>(balance), Times.Once);
    }

}
