﻿using System.Diagnostics;
using System.Net.Sockets;

namespace Capture;

public class Program
{
    static async Task Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        var sender = new Shared.Sender();
        const int DelayBetweenSendsMs = 100; // 100ms delay between screen captures

        while (true)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync("192.168.0.5", 8088);
                    using NetworkStream stream = client.GetStream();

                    while (true)
                    {
                        var data = sender.CaptureScreen();
                        Debug.WriteLine($"Sending data: {data.Length} bytes");

                        await stream.WriteAsync(BitConverter.GetBytes(data.Length), 0, 4); // Send the length of the data as 4 bytes
                        await stream.WriteAsync(data, 0, data.Length);

                        await Task.Delay(DelayBetweenSendsMs);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data: {ex.Message}");
            }

            await Task.Delay(1000);
        }
    }
}