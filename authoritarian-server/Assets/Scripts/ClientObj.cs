using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ProtoBuf;
using UnityEngine.UI;

public class ClientObj : MonoBehaviour {

    private Client _client;
    private string _id;
    private List<byte[]> _serverResponds;
    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private PlayersManager _playersManager;

    public static ClientObj Instantce { get; private set; }

    private void Awake() {
        Instantce = this;
        _serverResponds = new List<byte[]>();
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
                    _serverResponds.Add(received.Data);

                    DeserializeRespond();
                } catch (Exception e) {
                    Logger.LogError(e);
                    throw;
                }
            }
        });
    }

    private void DeserializeRespond() {
        if (_serverResponds.Count > 0) {
            var data = _serverResponds.First();
            _serverResponds.Remove(data);

            using (var ms = new MemoryStream(data)) {
                var deserialized = Serializer.Deserialize<ServerDataRespond>(ms);

                switch (deserialized.Command) {
                    case Command.Connect:
                        _playersManager.ClientActions.Add(new PlayersManager.ClientAction {
                            Id = deserialized.Id,
                            Action = Command.Connect,
                            Position = new Vector2(deserialized.PositionX, deserialized.PositionY)
                        });
                        break;
                    case Command.Disconnect:
                        _playersManager.ClientActions.Add(new PlayersManager.ClientAction {
                            Id = deserialized.Id,
                            Action = Command.Disconnect
                        });
                        Logger.LogMessage(deserialized.Id + " disconnected");
                        break;
                    case Command.PlayerMove:
                        Logger.LogMessage(deserialized.PositionX + deserialized.PositionY);
                        _playersManager.ClientActions.Add(new PlayersManager.ClientAction {
                            Id = deserialized.Id,
                            Action = Command.PlayerMove,
                            Position = new Vector2(deserialized.PositionX, deserialized.PositionY)
                        });
                        break;
                    case Command.AddExistPlayer:
                        _playersManager.ClientActions.Add(new PlayersManager.ClientAction
                        {
                            Id = deserialized.Id,
                            Action = Command.AddExistPlayer,
                            Position = new Vector2(deserialized.PositionX, deserialized.PositionY)
                        });
                        break;
                }
            }
        }
    }

    public void Disconnect() {
        SendRequest(Command.Disconnect);
        _client.Udp.Close();
    }

    public void SendRequest(Command command) {
        var requestData = new ClientDataRequest() {
            Id = _id,
            Command = command
        };
        byte[] data;

        using (var ms = new MemoryStream()) {
            Serializer.Serialize(ms, requestData);
            data = ms.ToArray();
        }
        _client.Send(data);
    }


}
