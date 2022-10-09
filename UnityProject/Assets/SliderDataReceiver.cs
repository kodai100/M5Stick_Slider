using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SliderDataReceiver : MonoBehaviour
{

    [SerializeField] private int port = 10100;

    private bool running;

    private Queue<byte> queue = new();

    private LowPassFilter filter = new(2);
    
    private CancellationTokenSource cts = new();

    public float CurrentValue
    {
        get;
        private set;
    }
    
    private void Start()
    {
        running = true;
        ReceiveUdpLoop(cts.Token);
    }

    private void Update()
    {
        while (queue.Count > 0)
        {
            var floatValue = queue.Dequeue() / 256f;
            
            var value = filter.Append(floatValue);
            
            CurrentValue = value ;
            
            Debug.Log(value);
        }
    }

    private void OnDestroy()
    {
        running = false;
        cts.Cancel();
    }

    private async void ReceiveUdpLoop(CancellationToken token)
    {

        var ip = new IPEndPoint(IPAddress.Any, port);

        try
        {
            await Task.Run(() =>
            {
                using var udpClient = new UdpClient(ip);
                
                // 1フレームに複数のUniverseパケットが受信される可能性があるので、とりあえずバッファに積む工程
                while (running)
                {
                    try
                    {
                        var result = udpClient.ReceiveAsync().WithCancellation(token);

                        if (result.Result.Buffer.Length > 0)
                        {
                            var buffer = result.Result.Buffer;
                            if (buffer.Length == sizeof(ushort))
                            {
                                var value = BitConverter.ToUInt16(buffer);
                                queue.Enqueue((byte)((value/4096f) * 256));
                            }
                            
                        }
                    }
                    catch (Exception e)
                    {
                        running = false;

                        switch (e)
                        {
                            case AggregateException _:
                            {
                                if (e.InnerException is TaskCanceledException)
                                {
                                    Debug.Log("UDP Receive Task canceled on destroy");
                                }

                                break;
                            }
                            case TaskCanceledException _:
                                Debug.Log("UDP Receive Task canceled on destroy");
                                break;
                            case SocketException _:
                                throw;
                            default:
                                Debug.LogException(e);
                                break;
                        }
                    }
                }
            }, token);
            
        }
        catch (SocketException e)
        {
            running = false;
            Debug.LogError($"ポート{port}が他のアプリケーションによって専有されています");
        }
        
        Debug.Log("UDP Server finished");
    }
}
