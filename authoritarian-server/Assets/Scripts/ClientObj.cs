using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ProtoBuf;

public class ClientObj : MonoBehaviour {

    private Client _client;
    private string _id;
    private CancellationTokenSource _stopReceiveToken;

    public static ClientObj Instantce { get; private set; }

    void Awake() {
        Instantce = this;
    }

    public void ConnectToServer() {
        _id = Guid.NewGuid().ToString().Substring(0, 12);
        _client = Client.ConnectTo(Network.player.ipAddress, 32123);

        SendRequest(Command.Connect);
        Logger.LogMessage("Request sended");

        Task.Factory.StartNew(async () => {
            while (true) {
                try {
                    var received = await _client.Receive();
                    using (var ms = new MemoryStream(received.Data)) {
                        var deserialized = Serializer.Deserialize<ServerDataRespond>(ms);
                        Logger.LogMessage(deserialized.Id + (Command)deserialized.Command);
                    }
                } catch (Exception e) {
                    Logger.LogError(e);
                    throw;
                }
            }
        });
    }

    public void Disconnect() {
        SendRequest(Command.Disconnect);
        _client.Udp.Close();
    }

    public void SendRequest(Command command) {
        var requestData = new ClientDataRequest() {
            Id = _id,
            Command = (int)command
        };
        byte[] data;

        using (var ms = new MemoryStream()) {
            Serializer.Serialize(ms, requestData);
            data = ms.ToArray();
        }
        _client.Send(data);
    }
}
