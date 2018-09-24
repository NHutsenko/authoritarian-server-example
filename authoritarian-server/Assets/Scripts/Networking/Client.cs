﻿using System;

public class Client : UdpBase {
    private Client() { }

    public static Client ConnectTo(string hostName, int port) {
        try {
            var connection = new Client();
            connection._udp.Connect(hostName, port);
            return connection;
        } catch (Exception e) {
            Logger.LogError(e);
            throw;
        }
    }

    public void Send(byte[] data) {
        try {
            _udp.Send(data, data.Length);
        } catch (Exception e) {
            Logger.LogError(e);
            throw;
        }
    }

}
