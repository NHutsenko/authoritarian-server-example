using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ProtoBuf;

public class ClientObj : MonoBehaviour {

    private Client _client;
    private string _id;
    private List<byte[]> _serverResponds;
    [SerializeField]
    private PlayerController _player;

    [SerializeField]
    private PlayersManager _playersManager;

    private Thread _listenThread;

    public static ClientObj Instantce { get; private set; }

    private bool isAlive = false;

    private void Awake() {
        Instantce = this;
        _serverResponds = new List<byte[]>();
    }

    public void ConnectToServer() {
        isAlive = true;
        _id = Guid.NewGuid().ToString().Substring(0, 12);
        _client = Client.ConnectTo(Host.HOST, 32123);

        _listenThread = new Thread(Listen);
        _listenThread.Start();

        SendRequest((int)Command.Connect);
        Logger.LogMessage("Request sended");  
    }

    private async void Listen() {
        while (isAlive) {
            try {
                var received = await _client.Receive();
                _serverResponds.Add(received.Data);

            } catch (Exception e) {
                Logger.LogError(e);
                throw;
            }
        }
    }

    public byte[] GetRespond()
    {
        if (_serverResponds.Count <= 0)
            return null;
        var data = _serverResponds.First();
        _serverResponds.Remove(data);

        return data;
    }

    public void Disconnect() {
        SendRequest((int)Command.Disconnect);
        isAlive = false;
        _listenThread.Abort();
        _client.Udp.Close();
    }

    public void SendRequest(int opCode)
    {
        var requestData = new Packet<ClientDataRequest>
        {
            OpCode = opCode,
            Data = new ClientDataRequest { Id = _id }
        };
        byte[] data;

        using (var ms = new MemoryStream()) {
            Serializer.Serialize(ms, requestData);
            data = ms.ToArray();
        }
        _client.Send(data);
    }
}
