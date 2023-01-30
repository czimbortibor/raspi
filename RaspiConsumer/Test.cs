using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using Microsoft.Azure.WebJobs;
using System.Text;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;

namespace RaspiConsumer
{
    public class Test
    {
        private const string EventHubName = "raspi";

        [FunctionName("Test")]
        public void Run([IoTHubTrigger(EventHubName, Connection = "EventHubConnectionString")]EventData message, ILogger log)
        {
            log.LogInformation($"IoT Hub message: {Encoding.UTF8.GetString(message.EventBody.ToArray())}");
        }
    }
}