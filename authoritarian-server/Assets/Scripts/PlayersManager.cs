using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


    private void MakeAction() {
        if (ClientActions.Count <= 0)
            return;
        var clientAction = ClientActions.First();
        ClientActions.Remove(clientAction);
        switch (clientAction.Action) {
            case Command.Connect:
                _clients.Add(new Clients(clientAction.Id, Instantiate(_playerPrefab)));
                _clients.Last().PlayerGameObject.transform.position = clientAction.Position;
                break;
            case Command.Disconnect:
                var toRemove = GetClientIndex(clientAction.Id);
                Destroy(_clients[toRemove].PlayerGameObject);
                _clients.Remove(_clients[toRemove]);
                break;
            case Command.PlayerMove:
                var toMove = GetClientIndex(clientAction.Id);
                _clients[toMove].PlayerGameObject.transform.position = clientAction.Position;
                break;
            case Command.AddExistPlayer:
                _clients.Add(new Clients(clientAction.Id, Instantiate(_playerPrefab)));
                var toChangePosition = GetClientIndex(clientAction.Id);
                _clients[toChangePosition].PlayerGameObject.transform.position = clientAction.Position;
                break;
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
