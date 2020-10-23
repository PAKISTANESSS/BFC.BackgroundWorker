# BFC.BackgroundWorker

BFC.BackgroundWorker is a background worker for .net core 3.1, with a queue system.

## Demo

You can download the project, and run the web project to see it working. Or just explore the files, it's all there.

## Installation

Use the package manager [nuget](https://www.nuget.org/) to install foobar.

```bash
Install-Package BFC.BackgroundWorker
```

## Usage

First, we need to define your identifier class.
```
 public class StudentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
```

Second, you need to create a BackgroundService for your worker.

```
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
```

Third, we need to add all of this configurations to the startup
```
services.AddHostedService<StudentWorker>()
                .AddSingleton<IBackgroundQueue<StudentDTO>, BackgroundQueue<StudentDTO>>();
```

Right now its all connected because of the StudentDTO, now we need to make the last step, to add job to the queue.

```
_queue.Enqueue(new StudentDTO()
            {
                Name = "Bernardo",
                Id = 7
            });
```

And its done. Versy easy to use. 


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)