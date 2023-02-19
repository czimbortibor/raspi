using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using System.Text;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace RaspiConsumer
{
    public class Test
    {
        private const string IoTHubName = "raspi";
        private const string IoTHubConnectionStringConfigName = "EventHubConnectionString";

        private const string SignalRHubName = "interface";
        private const string NewMessageTarget = "NewMessage";

        [FunctionName("Test")]
        public Task Run(
            [IoTHubTrigger(IoTHubName, Connection = IoTHubConnectionStringConfigName)]EventData message, 
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages, 
            ILogger log)
        {
            var rawMessage = Encoding.UTF8.GetString(message.EventBody.ToArray());
            log.LogInformation($"IoT Hub message: {rawMessage}");

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = NewMessageTarget,
                    Arguments = new[] { rawMessage }
                });
        }
    }
}