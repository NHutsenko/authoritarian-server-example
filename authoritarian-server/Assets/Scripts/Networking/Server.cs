using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

public class Server : UdpBase {
    private IPEndPoint _listenOn;
    private const int PORT = 32123;

    public Server() : this(new IPEndPoint(IPAddress.Parse(Network.player.ipAddress), PORT)) { }

    public Server(IPEndPoint endPoint) {
        try {
            Logger.LogMessage("Server IP is" + Network.player.ipAddress);
            _listenOn = endPoint;
            Udp = new UdpClient(_listenOn);
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