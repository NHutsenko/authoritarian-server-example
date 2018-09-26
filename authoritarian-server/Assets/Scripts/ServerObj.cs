using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ProtoBuf;

public class ServerObj : MonoBehaviour {

    private Server _server;
    private List<ClientData> _clients;
    void Start() {
        _clients = new List<ClientData>();
        _server = new Server();
        Logger.LogMessage("Server Started");
        Task.Factory.StartNew(async () => {
            while (true) {
                var received = await _server.Receive();
                using (var ms = new MemoryStream(received.Data)) {
                    PlayerData deserialized = Serializer.Deserialize<PlayerData>(ms);
                    // checking if client not in list of clients
                    if (_clients.Find(c => c.ClientId == deserialized.Id) == null) {
                        _clients.Add(new ClientData(deserialized.Id, received.Sender));
                    }
                    Logger.LogMessage(deserialized.Id + " " + deserialized.PositionX + " " + deserialized.PositionY);
                    foreach (var client in _clients)
                    {
                       _server.Reply(received.Data, client.ClientEndPoint);
                    }
                }
            }
        });
    }

    private void OnApplicationQuit() {
        byte[] data = Encoding.UTF8.GetBytes("quit");
        foreach (var client in _clients) {
            _server.Reply(data, client.ClientEndPoint);
        }
        _server.Udp.Close();
    }

    public class ClientData {
        public string ClientId { get; private set; }
        public IPEndPoint ClientEndPoint { get; private set; }

        public ClientData(string id, IPEndPoint clientEndPoint) {
            ClientId = id;
            ClientEndPoint = clientEndPoint;
        }
    }
}
