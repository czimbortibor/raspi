import os
import asyncio
import uuid
from azure.iot.device import Message
from azure.iot.device.aio import IoTHubDeviceClient


async def main():
    conn_str = os.getenv("IOTHUB_DEVICE_CONNECTION_STRING")

    device_client = IoTHubDeviceClient.create_from_connection_string(conn_str)

    await device_client.connect()

    data = {
        "message": "Yo!",
        "humidity": 42.0
    }
    telemetry = Message(str(data), uuid.uuid4(), "utf-8", "application/json")

    print("Sending telemetry...")
    await device_client.send_message(telemetry)
    print("Telemetry successfully sent")

    await device_client.shutdown()


if __name__ == "__main__":
    asyncio.run(main())
