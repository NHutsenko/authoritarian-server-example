using ProtoBuf;


[ProtoContract]
public class ClientDataRequest
{
    [ProtoMember(1)]
    public string Id { get; set; }
    [ProtoMember(2)]
    public Command Command { get; set; }
}

[ProtoContract]
public class ServerDataRespond
{
    [ProtoMember(1)]
    public string Id { get; set; }

    [ProtoMember(2)]
    public Command Command { get; set; }

    [ProtoMember(3)]
    public float PositionX { get; set; }

    [ProtoMember(4)]
    public float PositionY { get; set; }
}