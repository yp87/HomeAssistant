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

namespace UnitTest
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

            var secret = _fixture.Create<string>();
            _secretBytes = Encoding.ASCII.GetBytes(secret);
            _controller = new GitHubEventController(_actionHandlerMock.Object, secret);
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
            var unSupportedEvent = SetupWithEvent(false);

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<NotSupportedException>(executeAsync);
            Assert.Contains(unSupportedEvent, exception.Message);
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
            Assert.Contains(GitHubHeader.Signature, exception.Message);
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
            _aSignature = $"sha1={_fixture.Create<string>()}";
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

            _actionHandlerMock.Setup(m => m.Handle(It.Is<GitHubAction>(a => GitHubActionComparer(a))));

            await SetupWithBodyAsync();

            // Act
            await _controller.ReceiveEventAsync();

            // Assert
            _actionHandlerMock.Verify(m => m.Handle(It.Is<GitHubAction>(a => GitHubActionComparer(a))), Times.Once);
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
            _controller.ControllerContext.HttpContext.Request.Headers[GitHubHeader.Event] = eventName;
            return eventName;
        }

        private void SetupWithSignature()
        {
            SetupWithEvent();
            _aSignature ??= _fixture.Create<string>();
            _controller.ControllerContext.HttpContext.Request.Headers[GitHubHeader.Signature] = _aSignature;
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
