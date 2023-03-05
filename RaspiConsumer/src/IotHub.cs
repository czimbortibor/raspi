using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using System.Text;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace RaspiConsumer
{
    public class IotHub
    {
        private const string IoTHubName = "%iotHubName%";
        private const string IoTHubConnectionStringConfigName = "IotHubConnectionString";

        private const string SignalRHubName = "interface";
        private const string NewMessageTarget = "NewMessage";

        [FunctionName("ForwardToSignalR")]
        public Task ForwardToSignalR(
            [IoTHubTrigger(IoTHubName, Connection = IoTHubConnectionStringConfigName)]EventData message, 
            [SignalR(HubName = SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages, 
            ILogger log)
        {
            var rawMessage = Encoding.UTF8.GetString(message.EventBody.ToArray());
            log.LogDebug($"IoT Hub message: {rawMessage}");

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = NewMessageTarget,
                    Arguments = new[] { rawMessage }
                });
        }
    }
}