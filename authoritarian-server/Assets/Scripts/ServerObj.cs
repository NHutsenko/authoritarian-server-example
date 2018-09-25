using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                var msg = Encoding.UTF8.GetString(received.Data, 0, received.Data.Length);
                if (msg.Length == 12)
                {

                    ClientData client = new ClientData(msg, received.Sender);
                    _clients.Add(client);
                    Logger.LogMessage("Adding");
                }

                msg += "copy";
                foreach (var client in _clients) {
                    if (client.ClientEndPoint != null) {
                        _server.Reply(Encoding.UTF8.GetBytes(msg), client.ClientEndPoint);
                        Logger.LogMessage("Sedned " + msg + " to " + client.ClientId);
                    }
                    else
                    {
                        Logger.LogError(null);
                    }
                }

            }
        });
    }

    private void OnApplicationQuit()
    {
        byte[] data = Encoding.UTF8.GetBytes("quit");
        foreach(var client in _clients)
        {
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
