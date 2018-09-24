using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class ClientObj : MonoBehaviour {

    private Client _client;
    [SerializeField] private Text _text;

    void Start() {
        _client = Client.ConnectTo(Host.HOST, 32123);

        Task.Factory.StartNew(async () => {
            while (true) {
                try {
                    var received = await _client.Receive();
                    string msg = Encoding.UTF8.GetString(received.Data, 0, received.Data.Length);
                    _text.text = msg;
                    if (msg == "quit")
                        break;
                } catch (Exception e) {
                    Logger.LogError(e);
                    throw;
                }
            }
        });
    }

    private void LateUpdate() {
        string msg = String.Format("Client time {0}:{1}:{2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        byte[] data = Encoding.UTF8.GetBytes(msg);
        _client.Send(data);
    }
}
