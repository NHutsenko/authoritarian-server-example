using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] private GameObject _playerPrefab;

    void Start() {
        ClientObj.Instantce.ConnectToServer();
        Instantiate(_playerPrefab);
    }

    void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ClientObj.Instantce.SendRequest(Command.PlayerMoveTop);

        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ClientObj.Instantce.SendRequest(Command.PlayerMoveBot);
        }
    }

    void OnApplicationQuit() {
        ClientObj.Instantce.Disconnect();
    }
}
