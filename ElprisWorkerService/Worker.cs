using Confluent.Kafka;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using ElprisWorkerService.Models;

namespace ElprisWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
                ClientId = Dns.GetHostName()
            };


            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {

                const string topic = "elpris";

                int numProduced = 0;
                string checktime = "";
                DateTime checkTime = DateTime.Now;

                while (!stoppingToken.IsCancellationRequested)
                {

                    Record latestRecord = await GetElpris.ReturnRecordAsync();

                    //  if (checktime != latestRecord.HourUTC.ToString())
                    if (checkTime.Minute != latestRecord.HourDK.Minute)
                    {
                        checkTime = DateTime.Now;
                        checktime = latestRecord.HourUTC.ToString();

                        string message = JsonSerializer.Serialize<Record>(latestRecord);

                        producer.Produce(topic, new Message<string, string> { Key = "1", Value = message },
                                (deliveryReport) =>
                                {
                                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                                    {
                                        _logger.LogInformation($"Failed to deliver message: {deliveryReport.Error.Reason}");
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"Produced event to topic {topic}: key = {"1",-10} value = {message}");
                                        numProduced += 1;
                                    }
                                });

                    }

                    // delay indtil næste hele time plus et minut
                    int milisecondsToWait =  Convert.ToInt32 ((latestRecord.HourDK.AddHours(1).AddMinutes(1) - DateTime.Now).TotalMilliseconds);
                    _logger.LogInformation($"Miliseconds to wait: {milisecondsToWait} ");

                    // vent indtil næste hele time+1minut
                    await Task.Delay(milisecondsToWait, stoppingToken);

                }
                producer.Flush(TimeSpan.FromSeconds(10));
            }

        }


    }
}