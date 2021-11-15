using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Interfaces.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Common
{
    public class BackgroundProcessor : IHostedService
    {
        private readonly ILogger<BackgroundProcessor> _logger;
        private readonly IWorkerService _workerService;

        public BackgroundProcessor(ILogger<BackgroundProcessor> logger, IWorkerService workerService)
        {
            _logger = logger;
            _workerService = workerService;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Background process start");
                await _workerService.DoWork(cancellationToken);
            }
            catch (Exception e)
            {
                throw new CustomException("background Processor error", e);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background process stop");
            await Task.CompletedTask;
        }
    }
}
