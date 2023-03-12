using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.EventGrid;
using Microsoft.Extensions.Logging;
using RaspiConsumer.Models;
using System;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace RaspiConsumer.Factories;

public static class DeviceEventFactory
{
    public static DeviceEvent Create(EventGridEvent eventGridEvent, ILogger log)
    {
        var id = Guid.NewGuid();
        var eventTime = eventGridEvent.EventTime;
        var eventType = EventTypeExtensions.ParseRaw(eventGridEvent.EventType);

        switch (eventType)
        {
            // both events have indentical props
            case EventType.DeviceCreated:
            case EventType.DeviceDeleted:
                IotHubDeviceCreatedEventData lifeCycleEventData;
                try
                {
                    lifeCycleEventData = ((JObject)eventGridEvent.Data).ToObject<IotHubDeviceCreatedEventData>();
                }
                catch (Exception ex)
                {
                    log.LogError($"Could not deserialize the Device Created/Deleted event data.", ex);
                    return null;
                }

                var lifeCycleEvent = 
                    new DeviceLifeCycleEvent(id, eventTime, eventType, lifeCycleEventData.DeviceId,
                                            lifeCycleEventData.Twin.ConnectionState, lifeCycleEventData.Twin.Etag, lifeCycleEventData.Twin.Status);
                return lifeCycleEvent;

            case EventType.DeviceConnected:
            case EventType.DeviceDisconnected:
                IotHubDeviceConnectedEventData connectionStateEventData;
                try
                {
                    connectionStateEventData = ((JObject)eventGridEvent.Data).ToObject<IotHubDeviceConnectedEventData>();
                }
                catch (Exception ex)
                {
                    log.LogError($"Could not deserialize the Device Connected/Disconnected event data.", ex);
                    return null;
                }

                var connectionStateEvent = 
                    new DeviceConnectionStateEvent(id, eventTime, eventType, connectionStateEventData.DeviceId, 
                    connectionStateEventData.ModuleId, connectionStateEventData.DeviceConnectionStateEventInfo.SequenceNumber);
                return connectionStateEvent;

            default: throw new InvalidEventTypeException($"The '{eventType}' event type is not supported.");
        }
    }
}
