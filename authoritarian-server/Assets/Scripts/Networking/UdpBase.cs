using System.Net.Sockets;
using System.Threading.Tasks;

public abstract class UdpBase {
    public UdpClient Udp {
        get;
        protected set;
    }

    protected UdpBase() {
        Udp = new UdpClient();
    }

    public async Task<ReceivedData> Receive() {
        var result = await Udp.ReceiveAsync();
        return new ReceivedData() {
            Data = result.Buffer,
            Sender = result.RemoteEndPoint
        };
    }
}