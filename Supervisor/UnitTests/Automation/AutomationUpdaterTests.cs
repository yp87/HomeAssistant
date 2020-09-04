using AutoFixture;
using Xunit;
using Moq;
using Supervisor.Automation;
using Supervisor.FilesUpdater;
using System;
using System.Threading.Tasks;

namespace Supervisor.UnitTest.Automation
{
    public class AutomationUpdaterTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly AutomationUpdater _automationUpdater;

        private readonly Mock<IFilesUpdater> _filesUpdaterMock;

        private readonly Mock<IAutomationNotifier> _automationNotifierMock;

        private readonly Mock<IAutomationDeployer> _automationDeployerMock;

        public AutomationUpdaterTests()
        {
            _filesUpdaterMock = new Mock<IFilesUpdater>(MockBehavior.Strict);
            _automationNotifierMock = new Mock<IAutomationNotifier>(MockBehavior.Strict);
            _automationDeployerMock = new Mock<IAutomationDeployer>(MockBehavior.Strict);
            _automationUpdater = new AutomationUpdater(
                _filesUpdaterMock.Object,
                _automationNotifierMock.Object,
                _automationDeployerMock.Object);
        }

        [Fact]
        public async Task GivenAnUpdater_WithFailedFilesUpdater_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            string errorString = _fixture.Create<string>();
            _filesUpdaterMock.Setup(m => m.UpdateFilesAsync())
                .Throws(new InvalidOperationException(errorString));

            _automationNotifierMock.Setup(m => m.SendNotificationAsync(errorString))
                .Returns(Task.CompletedTask);

            // Act
            await _automationUpdater.UpdateAsync();

            // Assert
            _automationNotifierMock.Verify(m => m.SendNotificationAsync(errorString), Times.Once);
        }

        [Fact]
        public async Task GivenAnUpdater_WithFailedAutomationDeployer_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            _filesUpdaterMock.Setup(m => m.UpdateFilesAsync())
                .Returns(Task.CompletedTask);

            string errorString = _fixture.Create<string>();
            _automationDeployerMock.Setup(m => m.DeployAsync())
                .Throws(new InvalidOperationException(errorString));

            _automationNotifierMock.Setup(m => m.SendNotificationAsync(errorString))
                .Returns(Task.CompletedTask);

            // Act
            await _automationUpdater.UpdateAsync();

            // Assert
            _automationNotifierMock.Verify(m => m.SendNotificationAsync(errorString), Times.Once);
        }
    }
}
