using System;

namespace RaspiConsumer.Models;

public record DeviceConnectionStateEvent : DeviceEvent
{
    public DeviceConnectionStateEvent(Guid Id, DateTime EventTime, EventType EventType, string DeviceId, string ModuleId, string SequenceNumber) 
        : base(Id, EventTime, EventType, DeviceId)
    {
        this.ModuleId = ModuleId;
        this.SequenceNumber = SequenceNumber;
    }

    public string ModuleId { get; init; }

    public string SequenceNumber { get; init; }
}
