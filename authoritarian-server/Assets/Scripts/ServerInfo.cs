using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ServerInfo : MonoBehaviour {
    [SerializeField]
    private Text _users;
    [SerializeField]
    private Text _querys;

    private void LateUpdate() {
        _users.text = "Connected users:" + ClientsManager.Instance.Clients.Count;
        _querys.text = "Current querys to server: " + ServerObj.Instance.UserQuerys.Count;
    }
}
