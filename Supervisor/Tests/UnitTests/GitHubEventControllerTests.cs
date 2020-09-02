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

namespace UnitTest
{
    public class GitHubEventControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly Mock<IActionHandler> _actionHandlerMock;

        private readonly GitHubEventController _controller;

        public GitHubEventControllerTests()
        {
            _actionHandlerMock = new Mock<IActionHandler>(MockBehavior.Strict);
            _controller = new GitHubEventController(_actionHandlerMock.Object, _fixture.Create<string>());
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GivenAnEventController_WithAnUnsupportedEvent_WhenReceivingAnEvent_ThenTheEventIsNotHandled()
        {
            // Arrange
            string unSupportedEvent = _fixture.Create<string>();
            _actionHandlerMock.Setup(m => m.CanHandleAction(unSupportedEvent)).Returns(false);

            _controller.ControllerContext.HttpContext.Request.Headers[GitHubHeader.Event] = unSupportedEvent;

            // Act
            Func<Task> executeAsync = () => _controller.ReceiveEventAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<NotSupportedException>(executeAsync);
            Assert.Contains(unSupportedEvent, exception.Message);
        }
    }
}
