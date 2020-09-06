using Supervisor.Controllers;
using AutoFixture;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supervisor;
using System;
using Supervisor.ActionHandlers;
using Moq;
using System.Text.Json;
using Supervisor.Models;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Supervisor.Providers;

namespace Supervisor.UnitTest.Controllers
{
    public class GitHubEventControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IActionHandler> _actionHandlerMock;

        private readonly GitHubEventController _controller;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private readonly GitHubAction _aGitHubAction;

        private readonly byte[] _secretBytes;

        private string? _aSignature;

        public GitHubEventControllerTests()
        {
            _actionHandlerMock = new Mock<IActionHandler>(MockBehavior.Strict);

            var secretProvider = _fixture.Create<WebhookSecretProvider>();
            _secretBytes = Encoding.ASCII.GetBytes(secretProvider.WebhookSecret);
            _controller = new GitHubEventController(_actionHandlerMock.Object, secretProvider);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            _aGitHubAction = _fixture.Freeze<GitHubAction>();

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        [Fact]
        public async Task GivenAnEventController_WithAnUnsupportedEvent_WhenReceivingAnEvent_ThenTheEventIsNotHandled()
        {
            // Arrange
            var unsupportedEvent = SetupWithEvent(false);

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<NotSupportedException>(executeAsync);
            Assert.Contains(unsupportedEvent, exception.Message);
        }

        [Fact]
        public async Task GivenAnEventController_WithNoSignature_WhenReceivingAnEvent_ThenTheEventIsNotHandled()
        {
            // Arrange
            SetupWithEvent();

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(executeAsync);
            Assert.Contains(Constants.GitHubHeaderSignature, exception.Message);
        }

        [Fact]
        public async Task GivenAnEventController_WithNoBody_WhenReceivingAnEvent_ThenTheEventIsNotHandled()
        {
            // Arrange
            SetupWithSignature();

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(executeAsync);
            Assert.Contains("eventPayload", exception.Message);
        }

        [Fact]
        public async Task GivenAnEventController_WithPrefixlessSignature_WhenReceivingAnEvent_ThenTheEventIsNotHandled()
        {
            // Arrange
            await SetupWithBodyAsync();

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(executeAsync);
        }

        [Fact]
        public async Task GivenAnEventController_WithWrongSignature_WhenReceivingAnEvent_ThenTheEventIsUnauthorized()
        {
            // Arrange
            _aSignature = $"{Constants.Sha1Prefix}{_fixture.Create<string>()}";
            await SetupWithBodyAsync();

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(executeAsync);
        }

        [Fact]
        public async Task GivenAnEventController_WhenReceivingAnEvent_ThenTheEventIsHandled()
        {
            // Arrange
            var payload = JsonSerializer.Serialize(_aGitHubAction, _jsonSerializerOptions);
            var payloadBytes = Encoding.ASCII.GetBytes(payload);
            using var hmSha1 = new HMACSHA1(_secretBytes);
            var signatureWithoutPrefix = StringHelpers.ToHexString(hmSha1.ComputeHash(payloadBytes));
            _aSignature = $"sha1={signatureWithoutPrefix}";

            _actionHandlerMock.Setup(m => m.HandleAsync(It.Is<GitHubAction>(a => GitHubActionComparer(a))))
                .Returns(Task.CompletedTask);

            await SetupWithBodyAsync();

            // Act
            var returnCode = await _controller.ReceiveEventAsync();

            // Assert
            Assert.IsAssignableFrom<OkResult>(returnCode);
            _actionHandlerMock.Verify(m => m.HandleAsync(It.Is<GitHubAction>(a => GitHubActionComparer(a))), Times.Once);
        }

        private bool GitHubActionComparer(GitHubAction gitHubAction)
        {
            var actualSerializedAction = JsonSerializer.Serialize(gitHubAction, _jsonSerializerOptions);
            var expectedSerializedAction = JsonSerializer.Serialize(_aGitHubAction, _jsonSerializerOptions);
            return actualSerializedAction.Equals(expectedSerializedAction);
        }

        private string SetupWithEvent(bool isEventSupported = true)
        {
            string eventName = _fixture.Create<string>();
            _actionHandlerMock.Setup(m => m.CanHandleAction(eventName)).Returns(isEventSupported);
            _controller.ControllerContext.HttpContext.Request.Headers[Constants.GitHubHeaderEvent] = eventName;
            return eventName;
        }

        private void SetupWithSignature()
        {
            SetupWithEvent();
            _aSignature ??= _fixture.Create<string>();
            _controller.ControllerContext.HttpContext.Request.Headers[Constants.GitHubHeaderSignature] = _aSignature;
        }

        private async Task SetupWithBodyAsync()
        {
            SetupWithSignature();
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            await streamWriter.WriteAsync(JsonSerializer.Serialize(_aGitHubAction, _jsonSerializerOptions));
            await streamWriter.FlushAsync();
            _controller.ControllerContext.HttpContext.Request.Body = memoryStream;
            memoryStream.Seek(0, SeekOrigin.Begin);
        }
    }
}
