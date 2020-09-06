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

        private readonly Mock<IAutomationClient> _automationClientMock;

        private readonly Mock<IAutomationDeployer> _automationDeployerMock;

        public AutomationUpdaterTests()
        {
            _filesUpdaterMock = new Mock<IFilesUpdater>(MockBehavior.Strict);
            _automationClientMock = new Mock<IAutomationClient>(MockBehavior.Strict);
            _automationDeployerMock = new Mock<IAutomationDeployer>(MockBehavior.Strict);
            _automationUpdater = new AutomationUpdater(
                _filesUpdaterMock.Object,
                _automationClientMock.Object,
                _automationDeployerMock.Object);

            _automationClientMock.Setup(m => m.NotifyAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task GivenAnUpdater_WithFailedFilesUpdater_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            string errorString = _fixture.Create<string>();
            _filesUpdaterMock.Setup(m => m.UpdateFilesAsync())
                .Throws(new InvalidOperationException(errorString));

            _automationClientMock.Setup(m => m.NotifyAsync(errorString))
                .Returns(Task.CompletedTask);

            // Act
            await _automationUpdater.UpdateAsync();

            // Assert
            _automationClientMock.Verify(m => m.NotifyAsync(errorString), Times.Once);
        }

        [Fact]
        public async Task GivenAnUpdater_WithFailedAutomationDeployer_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            _filesUpdaterMock.Setup(m => m.UpdateFilesAsync())
                .ReturnsAsync("hass/ Suppervisor/");

            string errorString = _fixture.Create<string>();
            _automationDeployerMock.Setup(m => m.DeployAsync(It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new InvalidOperationException(errorString));

            _automationClientMock.Setup(m => m.NotifyAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _automationUpdater.UpdateAsync();

            // Assert
            _automationClientMock.Verify(m => m.NotifyAsync(errorString), Times.Once);
        }

        [Theory]
        [InlineData("", false, false)]
        [InlineData("README.md travis.yml .editorConfig", false, false)]
        [InlineData("WebhookProxy/Dockerfile README.md travis.yml .editorConfig", true, false)]
        [InlineData("README.md alarm/Dockerfile .editorConfig", true, false)]
        [InlineData("docker-compose.yaml", true, false)]
        [InlineData("README.md docker-compose.yaml hass/groups.yaml", true, true)]
        [InlineData("alarm/Dockerfile travis.yml hass/groups.yaml", true, true)]
        public async Task GivenAnUpdater_WithModifiedFiles_WhenUpdating_ThenOnlyTheMatchingSystemIsDeployed(string updatedFiles, bool infrastructureShouldDeploy, bool automationShouldDeploy)
        {
            // Arrange
            _filesUpdaterMock.Setup(m => m.UpdateFilesAsync())
                .ReturnsAsync(updatedFiles);

            _automationDeployerMock.Setup(m => m.DeployAsync(It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            _automationClientMock.Setup(m => m.NotifyAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _automationUpdater.UpdateAsync();

            // Assert
            if (infrastructureShouldDeploy || automationShouldDeploy)
            {
                _automationDeployerMock.Verify(m => m.DeployAsync(infrastructureShouldDeploy, automationShouldDeploy), Times.Once);
            }
            else
            {
                _automationDeployerMock.Verify(m => m.DeployAsync(It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            }
        }
    }
}
