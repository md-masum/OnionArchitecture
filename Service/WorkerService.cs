using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class WorkerService : IWorkerService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkerService(ILogger<WorkerService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("DoWork work");
                using var scope = _serviceScopeFactory.CreateScope();
                await Task.Delay(1000 * 60 * 60, cancellationToken);
            }
        }
    }
}
