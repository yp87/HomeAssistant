using AutoFixture;
using Xunit;
using Supervisor.ActionHandlers;
using Moq;
using Supervisor.Models;
using Supervisor.Automation;

namespace Supervisor.UnitTest.ActionHandlers
{
    public class CheckRunActionHandlerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly CheckRunActionHandler _actionHandler;

        private Mock<IAutomationUpdater> _automationUpdaterMock;

        public CheckRunActionHandlerTests()
        {
            _automationUpdaterMock = new Mock<IAutomationUpdater>();
            _actionHandler = new CheckRunActionHandler(_automationUpdaterMock.Object);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithUnsupportedActionName_WhenVerifyingActionSupport_ThenTheActionIsNotSupported()
        {
            // Arrange
            var unsupportedActionName = _fixture.Create<string>();

            // Act
            bool isActionSupported = _actionHandler.CanHandleAction(unsupportedActionName);

            // Assert
            Assert.False(isActionSupported);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithSupportedActionName_WhenVerifyingActionSupport_ThenTheActionIsSupported()
        {
            // Arrange
            var supportedActionName = "check_run";

            // Act
            bool isActionSupported = _actionHandler.CanHandleAction(supportedActionName);

            // Assert
            Assert.True(isActionSupported);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithoutCheckRun_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anIncompletedBuild = _fixture.Create<GitHubAction>();
            anIncompletedBuild.CheckRun = null;

            // Act
            _actionHandler.Handle(anIncompletedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithoutStatus_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anIncompletedBuild = _fixture.Create<GitHubAction>();
            anIncompletedBuild.CheckRun!.Status = null;

            // Act
            _actionHandler.Handle(anIncompletedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithoutCompletedBuild_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anIncompletedBuild = _fixture.Create<GitHubAction>();

            // Act
            _actionHandler.Handle(anIncompletedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithoutSuccessBuild_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var aFailedBuild = _fixture.Create<GitHubAction>();
            aFailedBuild.CheckRun!.Status = "completed";

            // Act
            _actionHandler.Handle(aFailedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithoutCheckSuit_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anInvalidBuild = _fixture.Create<GitHubAction>();
            anInvalidBuild.CheckRun!.Status = "completed";
            anInvalidBuild.CheckRun.Conclusion = "success";
            anInvalidBuild.CheckRun.CheckSuite = null;

            // Act
            _actionHandler.Handle(anInvalidBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithNonMasterBranch_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var aBuildOnANonMasterBranch = _fixture.Create<GitHubAction>();
            aBuildOnANonMasterBranch.CheckRun!.Status = "completed";
            aBuildOnANonMasterBranch.CheckRun.Conclusion = "success";

            // Act
            _actionHandler.Handle(aBuildOnANonMasterBranch);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithPullRequests_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var aSuccessfulMasterForPRBuild = _fixture.Create<GitHubAction>();
            aSuccessfulMasterForPRBuild.CheckRun!.Status = "completed";
            aSuccessfulMasterForPRBuild.CheckRun.Conclusion = "success";
            aSuccessfulMasterForPRBuild.CheckRun.CheckSuite!.HeadBranch = "master";

            // Act
            _actionHandler.Handle(aSuccessfulMasterForPRBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Never);
        }

        [Fact]
        public void GivenAnCRActionHandler_WithNullPullRequests_WhenHandling_ThenTheActionIsHandled()
        {
            // Arrange
            var aSuccessfulMasterBuild = _fixture.Create<GitHubAction>();
            aSuccessfulMasterBuild.CheckRun!.Status = "completed";
            aSuccessfulMasterBuild.CheckRun.Conclusion = "success";
            aSuccessfulMasterBuild.CheckRun.CheckSuite!.HeadBranch = "master";
            aSuccessfulMasterBuild.CheckRun.CheckSuite.PullRequests = null;

            // Act
            _actionHandler.Handle(aSuccessfulMasterBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Once);
        }

        [Fact]
        public void GivenAnCRActionHandler_WhenHandling_ThenTheActionIsHandled()
        {
            // Arrange
            var aSuccessfulMasterBuild = _fixture.Create<GitHubAction>();
            aSuccessfulMasterBuild.CheckRun!.Status = "completed";
            aSuccessfulMasterBuild.CheckRun.Conclusion = "success";
            aSuccessfulMasterBuild.CheckRun.CheckSuite!.HeadBranch = "master";
            aSuccessfulMasterBuild.CheckRun.CheckSuite.PullRequests = new PullRequest[0];

            // Act
            _actionHandler.Handle(aSuccessfulMasterBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.Update(), Times.Once);
        }
    }
}
