using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ProtoBuf;
using UnityEngine;

public class ClientsManager : MonoBehaviour {

    public List<ClientData> Clients { get; private set; }

    public static ClientsManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Clients = new List<ClientData>();
    }

    private void Update() {
        Respond();
    }

    private void Respond() {
        var query = ServerObj.Instance.GetQuery();
        if (query == null)
            return;

        Packet<ClientDataRequest> deserialized;
        using (var ms = new MemoryStream(query.QueryData)) {
            deserialized = Serializer.Deserialize<Packet<ClientDataRequest>>(ms);
        }


        Packet<ServerDataRespond> data = null;
        switch (deserialized.OpCode) {
            case (int)Command.Connect:
                using (var ms = new MemoryStream()) {
                    if (Clients.Count > 0) {
                        foreach (var connectedClient in Clients) {
                            using (var memStream = new MemoryStream()) {
                                var toSend = new Packet<ServerDataRespond>() {
                                    OpCode = (int)Command.AddExistPlayer,
                                    Data = new ServerDataRespond {
                                        Id = connectedClient.ClientId,
                                        PositionX = connectedClient.Position.x,
                                        PositionY = connectedClient.Position.y
                                    }
                                };

                                Serializer.Serialize(memStream, toSend);
                                ServerObj.Instance.SendRespond(toSend, query.UserEndPoint);
                            }
                        }
                    }

                    Clients.Add(new ClientData(deserialized.Data.Id, query.UserEndPoint, new Vector2(0, 0)));
                    var respond = new Packet<ServerDataRespond>() {
                        OpCode = (int)Command.Connect,
                        Data = new ServerDataRespond {
                            Id = deserialized.Data.Id,
                            PositionX = 0,
                            PositionY = 0
                        }
                    };

                    Serializer.Serialize(ms, respond);
                    data = respond;
                }
                break;
            case (int)Command.Disconnect:
                using (var ms = new MemoryStream()) {
                    var respond = new Packet<ServerDataRespond>() {
                        OpCode = (int)Command.Disconnect,
                        Data = new ServerDataRespond {
                            Id = deserialized.Data.Id
                        }
                    };
                    Serializer.Serialize(ms, respond);
                    data = respond;
                    var client = Clients.Find(c => c.ClientId == deserialized.Data.Id);
                    Clients.Remove(client);
                }
                break;
            case (int)Command.PlayerMoveTop:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Data.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x,
                        Clients[clientIndex].Position.y + 0.2f);


                    var respond = new Packet<ServerDataRespond>() {
                        OpCode = (int)Command.PlayerMove,
                        Data = new ServerDataRespond {
                            Id = deserialized.Data.Id,
                            PositionX = Clients[clientIndex].Position.x,
                            PositionY = Clients[clientIndex].Position.y
                        }
                    };

                    Serializer.Serialize(ms, respond);
                    data = respond;
                }

                break;
            case (int)Command.PlayerMoveBot:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Data.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x,
                        Clients[clientIndex].Position.y - 0.2f);

                    var respond = new Packet<ServerDataRespond>() {
                        OpCode = (int)Command.PlayerMove,
                        Data = new ServerDataRespond {
                            Id = deserialized.Data.Id,
                            PositionX = Clients[clientIndex].Position.x,
                            PositionY = Clients[clientIndex].Position.y
                        }
                    };
                    Serializer.Serialize(ms, respond);
                    data = respond;
                }
                break;
            case (int)Command.PlayerMoveLeft:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Data.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x - 0.2f,
                        Clients[clientIndex].Position.y);

                    var respond = new Packet<ServerDataRespond>() {
                        OpCode = (int)Command.PlayerMove,
                        Data = new ServerDataRespond {
                            Id = deserialized.Data.Id,
                            PositionX = Clients[clientIndex].Position.x,
                            PositionY = Clients[clientIndex].Position.y
                        }
                    };
                    Serializer.Serialize(ms, respond);
                    data = respond;
                }
                break;
            case (int)Command.PlayerMoveRight:
                using (var ms = new MemoryStream()) {
                    var client = Clients.Find(c => c.ClientId == deserialized.Data.Id);
                    var clientIndex = Clients.IndexOf(client);

                    Clients[clientIndex].Position = new Vector2(Clients[clientIndex].Position.x + 0.2f,
                        Clients[clientIndex].Position.y);

                    var respond = new Packet<ServerDataRespond>() {
                        OpCode = (int)Command.PlayerMove,
                        Data = new ServerDataRespond {
                            Id = deserialized.Data.Id,
                            PositionX = Clients[clientIndex].Position.x,
                            PositionY = Clients[clientIndex].Position.y
                        }
                    };
                    Serializer.Serialize(ms, respond);
                    data = respond;
                }
                break;
        }

        if (Clients.Count <= 0)
            return;
        foreach (var client in Clients) {
            ServerObj.Instance.SendRespond(data, client.ClientEndPoint);
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
}
