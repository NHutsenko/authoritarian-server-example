using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class PlayerData
{
    [ProtoMember(1)]
    public string Id { get; set; }

    [ProtoMember(2)]
    public float PositionX { get; set; }

    [ProtoMember(3)]
    public float PositionY;
}