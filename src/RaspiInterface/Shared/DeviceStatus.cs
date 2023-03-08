using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace RaspiInterface.Shared;

public enum DeviceStatus
{
    [EnumMember(Value = "enabled")]
    Enabled = 0,

    [EnumMember(Value = "disabled")]
    Disabled,
}
