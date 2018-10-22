using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ServerInfo : MonoBehaviour
{

    private ServerObj server;
    [SerializeField]
    private Text _users;
    [SerializeField]
    private Text _querys;

    private void Start()
    {
        server = GameObject.Find("Server").GetComponent<ServerObj>();
    }

	private void LateUpdate ()
	{
	    _users.text = "Connected users:" + server.Clients.Count;
	    _querys.text = "Current querys to server: " + server.UserQuerys.Count;
	}
}
