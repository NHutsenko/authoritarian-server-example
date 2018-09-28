using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ServerInfo : MonoBehaviour
{
    private ServerObj server;
    [SerializeField] private Text users;
    [SerializeField] private Text querys;
	void Start ()
	{
	    server = GameObject.Find("Server").GetComponent<ServerObj>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    users.text = "Connected users:" + server.Clients.Count.ToString();
	    querys.text = "Cuurent querys to server: " + server.UserQuerys.Count.ToString();
	}
}
