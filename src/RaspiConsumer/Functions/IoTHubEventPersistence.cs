using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using RaspiConsumer.Models;
using RaspiConsumer.Factories;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace RaspiConsumer.Functions;

public class IoTHubEventPersistence
{
    [FunctionName("PersistEvent")]
    public static Task PersistEvent(
        [EventGridTrigger] EventGridEvent eventGridEvent,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        log.LogDebug($"Event Grid event: {eventGridEvent}");

        return starter.StartNewAsync("EventPersistenceOrchestrator", eventGridEvent);
    }

    [FunctionName("EventPersistenceOrchestrator")]
    public static async Task EventPersistenceOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var eventGridEvent = context.GetInput<EventGridEvent>();

        (var deviceEvent, var eventType) = await context.CallActivityAsync<(object, EventType)>("PersistInCosmosDbActivity", eventGridEvent);
        await context.CallActivityAsync("ForwardToSignalRActivity", (deviceEvent, eventType));
    }

    [FunctionName("PersistInCosmosDbActivity")]
    public static Task<(object, EventType)> PersistInCosmosDbActivity(
        [ActivityTrigger] IDurableActivityContext inputs,
        [CosmosDB(
            databaseName: "raspi",
            containerName: "devices",
            Connection = "CosmosDBConnectionString")] ICollector<string> deviceEvents,
        ILogger log)
    {
        var eventGridEvent = inputs.GetInput<EventGridEvent>();

        var deviceEvent = DeviceEventFactory.Create(eventGridEvent, log);
        var dbOutput = DeviceEventToCosmosDbFormat(deviceEvent);

        deviceEvents.Add(dbOutput);
        return Task.FromResult<(object, EventType)>((deviceEvent, deviceEvent.EventType));
    }

    private static string DeviceEventToCosmosDbFormat(DeviceEvent deviceEvent)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };
        return JsonConvert.SerializeObject(deviceEvent, serializerSettings);
    }

    [FunctionName("ForwardToSignalRActivity")]
    public static Task ForwardToSignalRActivity(
        [ActivityTrigger] IDurableActivityContext inputs,
        [SignalR(HubName = "interface")] IAsyncCollector<SignalRMessage> signalRMessages)
    {
        (var deviceEvent, var eventType) = inputs.GetInput<(object, EventType)>();
        if (deviceEvent == null)
        {
            return Task.CompletedTask;
        }

        var signalRTarget = GetSignalRTargetName(eventType);

        return signalRMessages.AddAsync(
            new SignalRMessage
            {
                Target = signalRTarget,
                Arguments = new[] { deviceEvent }
            });
    }

    private static string GetSignalRTargetName(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.DeviceCreated:
                return "DeviceCreated";
            case EventType.DeviceDeleted:
                return "DeviceDeleted";
            case EventType.DeviceConnected:
                return "DeviceConnected";
            case EventType.DeviceDisconnected:
                return "DeviceDisconnected";

            default: throw new InvalidEventTypeException($"Not supported event type for SignalR messaging: {eventType}");
        }
    }
}