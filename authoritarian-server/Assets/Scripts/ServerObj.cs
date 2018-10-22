using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;
using ProtoBuf;

public class ServerObj : MonoBehaviour {

    private Server _server;
    public List<ClientData> Clients { get; private set; }
    public List<UserQuery> UserQuerys { get; private set; }

    private bool _isAlive;
    private Thread _listenThread;

    public static ServerObj Instance;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _isAlive = true;
        Clients = new List<ClientData>();
        UserQuerys = new List<UserQuery>();
        _server = new Server();
        Logger.LogMessage("Server Started");
        _listenThread = new Thread(Listen);
        _listenThread.Start();
    }

    private void OnApplicationQuit() {
        byte[] data;
        using (var ms = new MemoryStream()) {
            var stopServer = new Packet<ServerDataRespond>() {
                OpCode = (int)Command.Disconnect
            };
            Serializer.Serialize(ms, stopServer);
            data = ms.ToArray();
        }
        foreach (var client in Clients) {
            _server.Reply(data, client.ClientEndPoint);
        }

        _isAlive = false;
        _listenThread.Abort();
        _server.Udp.Close();
    }

    private async void Listen() {
        while (_isAlive) {
            var received = await _server.Receive();

            UserQuerys.Add(new UserQuery(received.Sender, received.Data));
        }
    }

    public UserQuery GetQuery()
    {
        if (UserQuerys.Count <= 0)
            return null;
        var currentQuery = UserQuerys.First();
        UserQuerys.Remove(currentQuery);
        return currentQuery;
    }

    public void SendRespond(Packet<ServerDataRespond> packet, IPEndPoint endPoint)
    {
        try
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, packet);
                _server.Reply(ms.ToArray(), endPoint);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message + " " + e.StackTrace);
            throw;
        }
    }

    public class ClientData {
        public string ClientId { get; }
        public IPEndPoint ClientEndPoint { get; }
        public Vector2 Position { get; set; }

        public ClientData(string id, IPEndPoint clientEndPoint, Vector2 position) {
            ClientId = id;
            ClientEndPoint = clientEndPoint;
            Position = position;
        }
    }

    public class UserQuery {
        public IPEndPoint UserEndPoint { get; }
        public byte[] QueryData { get; }

        public UserQuery(IPEndPoint userEndPoint, byte[] data) {
            if (userEndPoint == null)
                return;
            UserEndPoint = userEndPoint;
            QueryData = data;
        }
    }
}
