using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

public class Server : UdpBase {
    private const int Port = 32123;

    public Server() : this(new IPEndPoint(IPAddress.Parse(Host.HOST), Port)) { }

    public Server(IPEndPoint endPoint)
    {
        try {
            Logger.LogMessage("Server IP is" + Dns.GetHostName());
            Udp = new UdpClient(endPoint);
        } catch (Exception e) {
            Logger.LogError(e);
            throw;
        }
    }

    public void Reply(byte[] data, IPEndPoint endPoint) {
        try {
            Udp.Send(data, data.Length, endPoint);
        } catch (Exception e) {
            Logger.LogError(e);
            throw;
        }
    }
}