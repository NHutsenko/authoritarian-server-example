using ProtoBuf;


[ProtoContract]
public class ClientDataRequest : IPlayerData
{
    [ProtoMember(1)]
    public string Id { get; set; }
}

[ProtoContract]
public class ServerDataRespond : IPlayerData
{
    [ProtoMember(1)]
    public string Id { get; set; }

    [ProtoMember(2)]
    public float PositionX { get; set; }

    [ProtoMember(3)]
    public float PositionY { get; set; }
}

[ProtoContract]
public class Packet<T> where T : IPlayerData
{
    [ProtoMember(1)]
    public int OpCode { get; set; }

    [ProtoMember(2)]
    public T Data { get; set; }
}