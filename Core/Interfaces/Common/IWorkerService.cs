using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces.Common
{
    public interface IWorkerService
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}