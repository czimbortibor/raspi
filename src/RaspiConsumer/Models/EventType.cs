using System;

namespace RaspiConsumer.Models;

public enum EventType : byte
{
    DeviceCreated = 0,
    DeviceDeleted,
    DeviceConnected,
    DeviceDisconnected
}

public static class EventTypeExtensions
{
    public static EventType ParseRaw(string eventTypeStr)
    {
        switch (eventTypeStr)
        {
            case "Microsoft.Devices.DeviceCreated": return EventType.DeviceCreated;
            case "Microsoft.Devices.DeviceDeleted": return EventType.DeviceDeleted;
            case "Microsoft.Devices.DeviceConnected": return EventType.DeviceConnected;
            case "Microsoft.Devices.DeviceDisconnected": return EventType.DeviceDisconnected;

            default: throw new InvalidEventTypeException($"The '{eventTypeStr}' event type is not known.");
        }
    }
}

public class InvalidEventTypeException : Exception
{
    public InvalidEventTypeException(string message)
        : base(message)
    {
    }
}
