using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SocketClient
{
    private Socket socketClient;
    private Thread thread;
    private byte[] data = new byte[1024];

    public bool isTrigger;
    public float x;
    public float y;
    public float z;
    public float w;
    public int p;

    public SocketClient(string hostIP, int port) {
        thread = new Thread(() => {
            // while the status is "Disconnect", this loop will keep trying to connect.
            while (true) {
                try {
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketClient.Connect(new IPEndPoint(IPAddress.Parse(hostIP), port));
                    // while the connection
                    while (true) {
                        /*********************************************************
                         * TODO: you need to modify receive function by yourself *
                         *********************************************************/
                        if (socketClient.Available < 100) {
                            Thread.Sleep(1);
                            continue;
                        }
                        int length = socketClient.Receive(data);
                        string message = Encoding.UTF8.GetString(data, 0, length);
                        // Debug.Log("Recieve message: " + message);
                        // */
                        string[] values = message.Split(' ');
                        if (values.Length >= 6)
                        {
                            try {
                                isTrigger = bool.Parse(values[0]);
                            }
                            catch{
                                isTrigger = false;
                            }
                            p = int.Parse(values[1]);
                            w = float.Parse(values[2]);
                            x = float.Parse(values[3]);
                            y = float.Parse(values[4]);
                            z = float.Parse(values[5]);
                            Debug.Log("p" + values[1] + " x " + values[3] + " y " + values[4] + " z " + values[5]);
                        }
                        else
                        {
                            // Debug.LogWarning(data);
                        }

                    }
                } catch (Exception ex) {
                    if (socketClient != null) {
                        socketClient.Close();
                    }
                    // Debug.Log(ex.Message);
                }
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }

    public void Close() {
        thread.Abort();
        if (socketClient != null) {
            socketClient.Close();
        }
    }
}
