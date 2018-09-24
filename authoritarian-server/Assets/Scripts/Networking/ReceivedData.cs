using System.Net;

public struct ReceivedData
{
    public IPEndPoint Sender { get; set; }
    public byte[] Data { get; set; }
}
