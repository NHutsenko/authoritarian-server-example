using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBuf;
using UnityEngine;


public class PlayersManager : MonoBehaviour {

    public List<ClientAction> ClientActions { get; private set; }
    private List<Clients> _clients;
    [SerializeField] private GameObject _playerPrefab;

    private void Awake() {
        ClientActions = new List<ClientAction>();
        _clients = new List<Clients>();
    }

    private void LateUpdate() {
        MakeAction();
    }


    private void MakeAction()
    {
        var data = ClientObj.Instantce.GetRespond();
        
        if(data == null) 
            return;

        using (var ms = new MemoryStream(data)) {
            var deserialized = Serializer.Deserialize<Packet<ServerDataRespond>>(ms);


            switch (deserialized.OpCode) {
                case (int)Command.Connect:
                    _clients.Add(new Clients(deserialized.Data.Id, Instantiate(_playerPrefab)));
                    _clients.Last().PlayerGameObject.transform.position = new Vector2(deserialized.Data.PositionX,
                        deserialized.Data.PositionY);
                    Logger.LogMessage(deserialized.Data.Id +" connected");
                    break;
                case (int)Command.Disconnect:
                    var toRemove = GetClientIndex(deserialized.Data.Id);
                    Destroy(_clients[toRemove].PlayerGameObject);
                    _clients.Remove(_clients[toRemove]);
                    Logger.LogMessage(deserialized.Data.Id + " disconnected");
                    break;
                case (int)Command.PlayerMove:
                    Logger.LogMessage(deserialized.Data.Id);
                    var toMove = GetClientIndex(deserialized.Data.Id);
                    
                    _clients[toMove].PlayerGameObject.transform.position = new Vector2(deserialized.Data.PositionX,
                        deserialized.Data.PositionY);
                    Logger.LogMessage(deserialized.Data.PositionX + " " + deserialized.Data.PositionY);
                    break;
                case (int)Command.AddExistPlayer:
                    _clients.Add(new Clients(deserialized.Data.Id, Instantiate(_playerPrefab)));
                    var toChangePosition = GetClientIndex(deserialized.Data.Id);
                    _clients[toChangePosition].PlayerGameObject.transform.position = new Vector2(deserialized.Data.PositionX,
                        deserialized.Data.PositionY);
                    break;
            }
        }
    }

    private int GetClientIndex(string id) {
        var client = _clients.Find(tr => tr.Id == id);
        var index = _clients.IndexOf(client);
        return index;
    }

    public class ClientAction {
        public string Id { get; set; }
        public Command Action { get; set; }
        public Vector2 Position { get; set; }
    }

    private class Clients {
        public string Id { get; }
        public GameObject PlayerGameObject { get; }

        public Clients(string id, GameObject playerGameObject) {
            Id = id;
            PlayerGameObject = playerGameObject;
        }
    }
}
