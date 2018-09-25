using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ClientObj : MonoBehaviour {

    private Client _client;
    private string _id;
    private string _msg;

    void Start() {
        _id = Guid.NewGuid().ToString().Substring(0, 12);
        _client = Client.ConnectTo(Network.player.ipAddress, 32123);
        _client.Send(Encoding.UTF8.GetBytes(_id));

        Task.Factory.StartNew(async () => {
            while (true) {
                try {
                    var received = await _client.Receive();
                    _msg = Encoding.UTF8.GetString(received.Data, 0, received.Data.Length);
                    //debug
                    //Logger.LogMessage(_msg);
                    if (_msg == "quit")
                        break;
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
        string msg = String.Format(_id + " time {0}:{1}:{2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        byte[] data = Encoding.UTF8.GetBytes(msg);
        _client.Send(data);
    }
}
