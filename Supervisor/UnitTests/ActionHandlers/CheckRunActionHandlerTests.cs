using AutoFixture;
using Xunit;
using Supervisor.ActionHandlers;
using Moq;
using Supervisor.Models;
using Supervisor.Automation;
using System.Threading.Tasks;

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
            var automationClientMock = new Mock<IAutomationClient>();
            _actionHandler = new CheckRunActionHandler(_automationUpdaterMock.Object, automationClientMock.Object);
            automationClientMock.Setup(m => m.NotifyAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
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
        public async Task GivenAnCRActionHandler_WithoutCheckRun_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anIncompletedBuild = _fixture.Create<GitHubAction>();
            anIncompletedBuild.CheckRun = null;

            // Act
            await _actionHandler.HandleAsync(anIncompletedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithoutStatus_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anIncompletedBuild = _fixture.Create<GitHubAction>();
            anIncompletedBuild.CheckRun!.Status = null;

            // Act
            await _actionHandler.HandleAsync(anIncompletedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithoutCompletedBuild_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anIncompletedBuild = _fixture.Create<GitHubAction>();

            // Act
            await _actionHandler.HandleAsync(anIncompletedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithoutSuccessBuild_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var aFailedBuild = _fixture.Create<GitHubAction>();
            aFailedBuild.CheckRun!.Status = Constants.GitHubBuildCompleted;

            // Act
            await _actionHandler.HandleAsync(aFailedBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithoutCheckSuit_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var anInvalidBuild = _fixture.Create<GitHubAction>();
            anInvalidBuild.CheckRun!.Status = Constants.GitHubBuildCompleted;
            anInvalidBuild.CheckRun.Conclusion = Constants.GitHubBuildSuccess;
            anInvalidBuild.CheckRun.CheckSuite = null;

            // Act
            await _actionHandler.HandleAsync(anInvalidBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithNonMasterBranch_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var aBuildOnANonMasterBranch = _fixture.Create<GitHubAction>();
            aBuildOnANonMasterBranch.CheckRun!.Status = Constants.GitHubBuildCompleted;
            aBuildOnANonMasterBranch.CheckRun.Conclusion = Constants.GitHubBuildSuccess;

            // Act
            await _actionHandler.HandleAsync(aBuildOnANonMasterBranch);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithPullRequests_WhenHandling_ThenTheActionIsNotHandled()
        {
            // Arrange
            var aSuccessfulMasterForPRBuild = _fixture.Create<GitHubAction>();
            aSuccessfulMasterForPRBuild.CheckRun!.Status = Constants.GitHubBuildCompleted;
            aSuccessfulMasterForPRBuild.CheckRun.Conclusion = Constants.GitHubBuildSuccess;
            aSuccessfulMasterForPRBuild.CheckRun.CheckSuite!.HeadBranch = Constants.MasterBranchName;

            // Act
            await _actionHandler.HandleAsync(aSuccessfulMasterForPRBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Never);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WithNullPullRequests_WhenHandling_ThenTheActionIsHandled()
        {
            // Arrange
            var aSuccessfulMasterBuild = _fixture.Create<GitHubAction>();
            aSuccessfulMasterBuild.CheckRun!.Status = Constants.GitHubBuildCompleted;
            aSuccessfulMasterBuild.CheckRun.Conclusion = Constants.GitHubBuildSuccess;
            aSuccessfulMasterBuild.CheckRun.CheckSuite!.HeadBranch = Constants.MasterBranchName;
            aSuccessfulMasterBuild.CheckRun.CheckSuite.PullRequests = null;

            // Act
            await _actionHandler.HandleAsync(aSuccessfulMasterBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenAnCRActionHandler_WhenHandling_ThenTheActionIsHandled()
        {
            // Arrange
            var aSuccessfulMasterBuild = _fixture.Create<GitHubAction>();
            aSuccessfulMasterBuild.CheckRun!.Status = Constants.GitHubBuildCompleted;
            aSuccessfulMasterBuild.CheckRun.Conclusion = Constants.GitHubBuildSuccess;
            aSuccessfulMasterBuild.CheckRun.CheckSuite!.HeadBranch = Constants.MasterBranchName;
            aSuccessfulMasterBuild.CheckRun.CheckSuite.PullRequests = new PullRequest[0];

            // Act
            await _actionHandler.HandleAsync(aSuccessfulMasterBuild);

            // Assert
            _automationUpdaterMock.Verify(m => m.UpdateAsync(), Times.Once);

        }
    }
}
