using AutoFixture;
using Xunit;
using Moq;
using Supervisor.FilesUpdater;
using System;
using System.Threading.Tasks;

namespace Supervisor.UnitTest.FilesUpdater
{
    public class SourceControlFilesUpdaterTests
    {
        private readonly Fixture _fixture = new Fixture();

        private readonly SourceControlFilesUpdater _sourceControlFilesUpdater;

        private readonly Mock<ISourceController> _sourceControllerMock;

        public SourceControlFilesUpdaterTests()
        {
            _sourceControllerMock = new Mock<ISourceController>(MockBehavior.Strict);
            _sourceControlFilesUpdater = new SourceControlFilesUpdater(_sourceControllerMock.Object);
        }

        [Fact]
        public async Task GivenAFilesUpdater_WithSourceControlNotInMaster_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            string aBranchName = _fixture.Create<string>();
            _sourceControllerMock.Setup(m => m.GetCurrentBranchNameAsync())
                .ReturnsAsync(aBranchName);

            // Act
            Func<Task> executeAsync = () => _sourceControlFilesUpdater.UpdateFilesAsync();

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(executeAsync);
            exception.Message.Contains(aBranchName);
        }

        [Fact]
        public async Task GivenAFilesUpdater_WithUnsynchronizedChangesInSourceControl_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            _sourceControllerMock.Setup(m => m.GetCurrentBranchNameAsync())
                .ReturnsAsync(Constants.MasterBranchName);

            _sourceControllerMock.Setup(m => m.HasUnsynchronizedChangesAsync())
                .ReturnsAsync(true);

            // Act
            Func<Task> executeAsync = () => _sourceControlFilesUpdater.UpdateFilesAsync();

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(executeAsync);
        }

        [Fact]
        public async Task GivenAFilesUpdater_WithFailingRepositoryUpdate_WhenUpdating_ThenTheUpdateIsNotDone()
        {
            // Arrange
            _sourceControllerMock.Setup(m => m.GetCurrentBranchNameAsync())
                .ReturnsAsync(Constants.MasterBranchName);

            _sourceControllerMock.Setup(m => m.HasUnsynchronizedChangesAsync())
                .ReturnsAsync(false);

            _sourceControllerMock.Setup(m => m.UpdateRepositoryAsync())
                .ThrowsAsync(new Exception());

            // Act
            Func<Task> executeAsync = () => _sourceControlFilesUpdater.UpdateFilesAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(executeAsync);
        }

        [Fact]
        public async Task GivenAFilesUpdater_WhenUpdating_ThenTheUpdateIsDone()
        {
            // Arrange
            string aBranchName = _fixture.Create<string>();
            _sourceControllerMock.Setup(m => m.GetCurrentBranchNameAsync())
                .ReturnsAsync(Constants.MasterBranchName);

            _sourceControllerMock.Setup(m => m.HasUnsynchronizedChangesAsync())
                .ReturnsAsync(false);

            _sourceControllerMock.Setup(m => m.UpdateRepositoryAsync())
                .ReturnsAsync(string.Empty);

            // Act
            await _sourceControlFilesUpdater.UpdateFilesAsync();

            // Assert
            _sourceControllerMock.Verify(m => m.UpdateRepositoryAsync(), Times.Once);
        }
    }
}
