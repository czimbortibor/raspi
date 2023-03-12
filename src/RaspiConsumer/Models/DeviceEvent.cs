using System;

namespace RaspiConsumer.Models;

public record DeviceEvent(Guid Id, DateTime EventTime, EventType EventType, string DeviceId) { }
