using System;
using System.Threading.Tasks;

namespace Supervisor.FilesUpdater
{
    public class SourceControlFilesUpdater : IFilesUpdater
    {
        private readonly ISourceController _sourceController;

        public SourceControlFilesUpdater(ISourceController sourceController)
        {
            _sourceController = sourceController;
        }

        public async Task UpdateFilesAsync()
        {
            var branchName = await _sourceController.GetCurrentBranchNameAsync();
            if (!branchName.Equals(Constants.MasterBranchName, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new InvalidOperationException($"Could not update repository because the current branch is {branchName}.");
            }

            if (await _sourceController.HasUnsynchronizedChangesAsync())
            {
                throw new InvalidOperationException($"Could not update repository because there are non synchronized changes.");
            }

            await _sourceController.UpdateRepositoryAsync();
        }
    }
}
