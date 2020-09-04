using System.Threading.Tasks;

namespace Supervisor.FilesUpdater
{
    public interface IFilesUpdater
    {
        Task UpdateFilesAsync();
    }
}
