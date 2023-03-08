namespace RaspiInterface.Shared;

public class DeviceDetails
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public GeoLocation Location { get; set; } = null!;

    public DeviceStatus Status { get; set; }

    public string? ETag { get; set; }

    public DateTime? LastActivityTime { get; set; }
}
