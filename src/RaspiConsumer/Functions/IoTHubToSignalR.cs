using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using System.Text;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace RaspiConsumer.Functions;

public class IoTHubToSignalR
{
    [FunctionName("ForwardIotHubMessageToSignalR")]
    public static Task ForwardIotHubMessageToSignalR(
        [IoTHubTrigger("%iotHubName%", Connection = "IotHubConnectionString")] EventData message,
        [SignalR(HubName = "interface")] IAsyncCollector<SignalRMessage> signalRMessages,
        ILogger log)
    {
        var rawMessage = Encoding.UTF8.GetString(message.EventBody.ToArray());
        log.LogDebug($"IoT Hub message: {rawMessage}");

        return signalRMessages.AddAsync(
            new SignalRMessage
            {
                Target = "NewMessage",
                Arguments = new[] { rawMessage }
            });
    }
}