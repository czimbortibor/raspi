using RaspiInterface.Shared;

namespace RaspiInterface.Client.Services;

public interface IDeviceClient
{
    Task<IEnumerable<Device>> GetDevices();

    Task<DeviceDetails?> GetDevice(string deviceId);
}
