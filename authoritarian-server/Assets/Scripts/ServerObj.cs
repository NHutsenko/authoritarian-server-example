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
                using (var ms = new MemoryStream(received.Data)) {
                    PlayerData deserialized = Serializer.Deserialize<PlayerData>(ms);
                    // checking if client not in list of clients
                    if (Clients.Find(c => c.ClientId == deserialized.Id) == null) {
                        Clients.Add(new ClientData(deserialized.Id, received.Sender));
                    }
                    UserQuerys.Add(new UserQuery(received.Sender, received.Data));
                    
                    foreach (var client in Clients)
                    {
                        //TODO: add validation data here
                        PlayerData deserializedPlayerData = DeserializeUserData();
                        Logger.LogMessage(deserializedPlayerData.Id + deserializedPlayerData.PositionX + deserializedPlayerData.PositionY);
                        _server.Reply(received.Data, client.ClientEndPoint);
                    }
                }
            }
        });
    }

    private void OnApplicationQuit() {
        byte[] data = Encoding.UTF8.GetBytes("quit");
        foreach (var client in Clients) {
            _server.Reply(data, client.ClientEndPoint);
        }
        _server.Udp.Close();
    }

    private PlayerData DeserializeUserData() {
        UserQuery currentQuery = UserQuerys.First();
        UserQuerys.Remove(currentQuery);
        PlayerData deserialized;
        using (MemoryStream ms = new MemoryStream(currentQuery.QueryData)) {
            deserialized = Serializer.Deserialize<PlayerData>(ms);
        }

        return deserialized;
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
