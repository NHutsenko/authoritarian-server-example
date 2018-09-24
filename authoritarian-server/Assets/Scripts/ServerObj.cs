using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerObj : MonoBehaviour
{

    private Server _server;
	void Start ()
	{
	    _server = new Server();
        Logger.LogMessage("Server Started");
	    Task.Factory.StartNew(async () =>
	    {
	        while (true)
	        {
	            var received = await _server.Receive();
	            var msg = Encoding.UTF8.GetString(received.Data, 0, received.Data.Length);
	            Logger.LogMessage(msg);
	            msg += "copy";
	            _server.Reply(Encoding.UTF8.GetBytes(msg), received.Sender);
	        }
	    });
	}
}
