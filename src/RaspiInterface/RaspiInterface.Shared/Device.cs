namespace RaspiInterface.Shared;

public class Device
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public GeoLocation Location { get; set; } = null!;
}
