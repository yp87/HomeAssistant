using System.Threading.Tasks;

namespace Supervisor.FilesUpdater
{
    public interface ISourceController
    {
        Task<string> GetCurrentBranchNameAsync();

        Task<bool> HasUnsynchronizedChangesAsync();

        Task<string> UpdateRepositoryAsync();
    }
}
