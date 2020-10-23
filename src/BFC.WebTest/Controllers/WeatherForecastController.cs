using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BFC.BackgroundWorker.Domain.Abstractions;
using BFC.WebTest.BackgroundWorker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BFC.WebTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IBackgroundQueue<StudentDTO> _queue;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBackgroundQueue<StudentDTO> queue)
        {
            _logger = logger;
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _queue.Enqueue(new StudentDTO()
            {
                Name = "Bernardo",
                Id = 7
            });

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
