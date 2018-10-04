using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

public class Server : UdpBase {
    private const int Port = 32123;

    public Server() : this(new IPEndPoint(IPAddress.Parse(Network.player.ipAddress), Port)) { }

    public Server(IPEndPoint endPoint)
    {
        try {
            Logger.LogMessage("Server IP is" + Network.player.ipAddress);
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