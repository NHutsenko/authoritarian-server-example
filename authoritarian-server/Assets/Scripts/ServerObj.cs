using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ProtoBuf;

public class ServerObj : MonoBehaviour {

    private Server _server;
    public List<ClientData> Clients { get; private set; }

    public List<UserQuery> UserQuerys { get; private set; }
    void Start() {
        Clients = new List<ClientData>();
        UserQuerys = new List<UserQuery>();
        _server = new Server();
        Logger.LogMessage("Server Started");
        Task.Factory.StartNew(async () => {
            while (true) {
                var received = await _server.Receive();
                UserQuerys.Add(new UserQuery(received.Sender, received.Data));

                Respond();
            }
        });
    }

    private void OnApplicationQuit() {
        byte[] data;
        using (var ms = new MemoryStream()) {
            ServerDataRespond stopServer = new ServerDataRespond() {
                Command = (int)Command.Disconnect,
            };
            Serializer.Serialize(ms, stopServer);
            data = ms.ToArray();
        }
        foreach (var client in Clients) {
            _server.Reply(data, client.ClientEndPoint);
        }
        _server.Udp.Close();
    }

    private void Respond() {
        UserQuery currentQuery = UserQuerys.First();
        UserQuerys.Remove(currentQuery);
        ClientDataRequest deserialized;
        using (var ms = new MemoryStream(currentQuery.QueryData)) {
            deserialized = Serializer.Deserialize<ClientDataRequest>(ms);
            Logger.LogMessage(deserialized.Id + (Command)deserialized.Command);
        }
        

        byte[] data = null;
        switch (deserialized.Command) {
            case (int)Command.Connect:
                using (var ms = new MemoryStream()) {
                    Clients.Add(new ClientData(deserialized.Id, currentQuery.UserEndPoint));
                    ServerDataRespond respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = (int)Command.Connect
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
            case (int)Command.Disconnect:
                using (var ms = new MemoryStream()) {
                    ServerDataRespond respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = (int)Command.Disconnect
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                    ClientData client = Clients.Find(c => c.ClientId == deserialized.Id);
                    Clients.Remove(client);
                }
                break;
            case (int)Command.PlayerMoveTop:
                using (var ms = new MemoryStream()) {
                    ServerDataRespond respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = (int)Command.PlayerMoveTop,
                        PositionX = 0,
                        PositionY = 0
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
            case (int)Command.PlayerMoveBot:
                using (var ms = new MemoryStream()) {
                    ServerDataRespond respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = (int)Command.PlayerMoveBot,
                        PositionX = 0,
                        PositionY = 0
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
        }
        if (Clients.Count > 0)
            foreach (var client in Clients)
            {
                _server.Reply(data, client.ClientEndPoint);
            }
    }

    public class ClientData {
        public string ClientId { get; private set; }
        public IPEndPoint ClientEndPoint { get; private set; }

        public ClientData(string id, IPEndPoint clientEndPoint) {
            ClientId = id;
            ClientEndPoint = clientEndPoint;
        }
    }

    public class UserQuery {
        public IPEndPoint UserEndPoint { get; private set; }
        public byte[] QueryData { get; private set; }

        public UserQuery(IPEndPoint userEndPoint, byte[] data) {
            if (userEndPoint != null) {
                UserEndPoint = userEndPoint;
                QueryData = data;
            }
        }
    }
}
