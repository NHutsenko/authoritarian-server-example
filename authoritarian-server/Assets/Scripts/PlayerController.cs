using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    private void Start() {
        ClientObj.Instantce.ConnectToServer();
    }

    private void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ClientObj.Instantce.SendRequest(Command.PlayerMoveTop);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ClientObj.Instantce.SendRequest(Command.PlayerMoveBot);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ClientObj.Instantce.SendRequest(Command.PlayerMoveLeft);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ClientObj.Instantce.SendRequest(Command.PlayerMoveRight);
        }
    }

    private void OnApplicationQuit() {
        ClientObj.Instantce.Disconnect();
    }
}
