using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TimerTriggerReceiver;

namespace FunctionAppTimerTriggerReceiver
{
    public class TimerTriggerReceiver
    {
        private readonly ILogger<TimerTriggerReceiver> _logger;
        private readonly IServerRepository _serverRepository;
        private readonly QueueClient _queueClient;
        private string _connectionString;

        public TimerTriggerReceiver(ILogger<TimerTriggerReceiver> logger, IServerRepository serverRepository)
        {
            _logger = logger;
            _serverRepository = serverRepository;
            _connectionString = Environment.GetEnvironmentVariable("QueueConnection", EnvironmentVariableTarget.Process);
            _queueClient = new QueueClient(_connectionString, "cola1",new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });
        }

        [FunctionName("TimerTriggerReceiver")]
        public async Task Run([TimerTrigger("%TimerSchedule%")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await ReadQueue();
        }

        private async Task ReadQueue()
        {

            if (_queueClient.Exists())
            {
                _logger.LogInformation("Reading Queue at: {time}", DateTimeOffset.Now);
                var queueMessages = await _queueClient.ReceiveMessagesAsync();
                if (queueMessages.Value != null)
                {

                    foreach (QueueMessage message in _queueClient.ReceiveMessages(maxMessages: 10).Value)
                    {
                        var accountsDto = JsonSerializer.Deserialize<List<AccountDto>>(message.Body.ToString());
                        DisplayAccountInformation(accountsDto);
                        await SendToDatabase(accountsDto);
                        _queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                    }
                }
            }
        }
        private void DisplayAccountInformation(List<AccountDto> accountsDtos)
        {
            accountsDtos?.ForEach(account =>
            {
                Console.WriteLine($"Received: Account Information: \t{account.Cbu}" +
                    $"\t{account.Alias} \t{account.Balance}");
                _logger.LogInformation("\nMessage received at: {time}", DateTimeOffset.Now);
            });
        }
        private async Task SendToDatabase(List<AccountDto> accountsDtos)
        {
            foreach (var accountDto in accountsDtos)
            {
                _logger.LogInformation("Sending to database at: {time}", DateTimeOffset.Now); ;
                await _serverRepository.PostAccount(new Account
                {
                    Alias = accountDto.Alias,
                    Balance = accountDto.Balance,
                    Cbu = accountDto.Cbu,
                });
            }
        }
    }
}
