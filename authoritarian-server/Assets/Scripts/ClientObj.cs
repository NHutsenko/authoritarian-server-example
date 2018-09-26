using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ProtoBuf;

public class ClientObj : MonoBehaviour {

    private Client _client;
    private string _id;

    void Start() {
        _id = Guid.NewGuid().ToString().Substring(0, 12);
        _client = Client.ConnectTo(Network.player.ipAddress, 32123);

        Task.Factory.StartNew(async () => {
            while (true) {
                try {
                    var received = await _client.Receive();
                    using (var ms = new MemoryStream(received.Data))
                    {
                        var deserialized = Serializer.Deserialize<PlayerData>(ms);
                        Logger.LogMessage(deserialized.Id);
                    }
                } catch (Exception e) {
                    Logger.LogError(e);
                    throw;
                }
            }
        });
    }

    private void OnApplicationQuit()
    {
        _client.Udp.Close();
    }

    private void LateUpdate() {
        var playerDataToSend = new PlayerData
        {
            Id = _id,
            PositionX = .5f,
            PositionY = .7f
        };
        byte[] data;

        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, playerDataToSend);
            data = ms.ToArray();
        }
        _client.Send(data);
    }
}
