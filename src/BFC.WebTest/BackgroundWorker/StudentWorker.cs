using BFC.BackgroundWorker.Domain.Abstractions;
using BFC.WebTest.BackgroundWorker.Models;
using BFC.WebTest.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BFC.WebTest.BackgroundWorker
{
    public class StudentWorker : BackgroundService
    {
        private readonly ILogger<StudentWorker> _logger;
        private readonly IBackgroundQueue<StudentDTO> _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public StudentWorker(ILogger<StudentWorker> logger, IBackgroundQueue<StudentDTO> queue, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical($"The {nameof(StudentDTO)} is  stopping due to a host shutdown, queued items might not be processed anymore");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"The {nameof(StudentWorker)} is now running in background");

            await Process(stoppingToken);
        }

        private async Task Process(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(600, stoppingToken);

                    //Go get the next object in queue
                    var student = _queue.Dequeue();
                    if (student != null)
                    {
                        //Needed if you need to inject some services
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            //Get the required service we need
                            var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                            studentService.SaveStudent(student.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("Something went wrong while running the job", ex);
                }
            }
        }
    }
}
