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

    private void Start() {
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
            var stopServer = new ServerDataRespond() {
                Command = Command.Disconnect,
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
        var currentQuery = UserQuerys.First();
        UserQuerys.Remove(currentQuery);
        ClientDataRequest deserialized;
        using (var ms = new MemoryStream(currentQuery.QueryData)) {
            deserialized = Serializer.Deserialize<ClientDataRequest>(ms);
            Logger.LogMessage(deserialized.Id + deserialized.Command);
        }


        byte[] data = null;
        switch (deserialized.Command) {
            case Command.Connect:
                using (var ms = new MemoryStream()) {
                    foreach (var connectedClient in Clients) {
                        using (var memStream = new MemoryStream()) {
                            var toSend = new ServerDataRespond {
                                Id = connectedClient.ClientId,
                                Command = Command.AddExistPlayer,
                                PositionX = connectedClient.Position.x,
                                PositionY = connectedClient.Position.y
                            };

                            Serializer.Serialize(memStream, toSend);
                            _server.Reply(memStream.ToArray(), currentQuery.UserEndPoint);
                        }
                    }

                    Clients.Add(new ClientData(deserialized.Id, currentQuery.UserEndPoint, new Vector2(0, 0)));
                    var respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = (int)Command.Connect,
                        PositionX = 0,
                        PositionY = 0
                    };

                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
            case Command.Disconnect:
                using (var ms = new MemoryStream()) {
                    var respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = Command.Disconnect
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                    var client = Clients.Find(c => c.ClientId == deserialized.Id);
                    Clients.Remove(client);
                }
                break;
            case Command.PlayerMoveTop:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x,
                        Clients[clientIndex].Position.y + 0.2f);

                    var respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = Command.PlayerMove,
                        PositionX = Clients[clientIndex].Position.x,
                        PositionY = Clients[clientIndex].Position.y
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
            case Command.PlayerMoveBot:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x,
                        Clients[clientIndex].Position.y - 0.2f);

                    var respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = Command.PlayerMove,
                        PositionX = Clients[clientIndex].Position.x,
                        PositionY = Clients[clientIndex].Position.y
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
            case Command.PlayerMoveLeft:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x - 0.2f,
                        Clients[clientIndex].Position.y);

                    var respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = Command.PlayerMove,
                        PositionX = Clients[clientIndex].Position.x,
                        PositionY = Clients[clientIndex].Position.y
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
            case Command.PlayerMoveRight:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x + 0.2f,
                        Clients[clientIndex].Position.y);

                    var respond = new ServerDataRespond() {
                        Id = deserialized.Id,
                        Command = Command.PlayerMove,
                        PositionX = Clients[clientIndex].Position.x,
                        PositionY = Clients[clientIndex].Position.y
                    };
                    Serializer.Serialize(ms, respond);
                    data = ms.ToArray();
                }
                break;
        }

        if (Clients.Count <= 0)
            return;
        foreach (var client in Clients) {
            _server.Reply(data, client.ClientEndPoint);
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
