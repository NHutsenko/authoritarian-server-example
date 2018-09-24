using System.Net.Sockets;
using System.Threading.Tasks;

public abstract class UdpBase {
    protected UdpClient _udp;

    protected UdpBase() {
        _udp = new UdpClient();
    }

    public async Task<ReceivedData> Receive() {
        var result = await _udp.ReceiveAsync();
        return new ReceivedData() {
            Data = result.Buffer,
            Sender = result.RemoteEndPoint
        };
    }
}