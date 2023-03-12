using System;

namespace RaspiConsumer.Models;

public record DeviceLifeCycleEvent : DeviceEvent
{
    public DeviceLifeCycleEvent(Guid Id, DateTime EventTime, EventType EventType, string DeviceId, string ConnectionState, string Etag, string Status) 
        : base(Id, EventTime, EventType, DeviceId)
    {
        this.ConnectionState = ConnectionState;
        this.Etag = Etag;
        this.Status = Status;
    }

    public string ConnectionState { get; init; }

    public string Etag { get; init; }

    public string Status { get; init; }
}
