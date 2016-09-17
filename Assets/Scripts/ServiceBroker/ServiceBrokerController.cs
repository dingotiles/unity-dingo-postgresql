using UnityEngine;
using System.Collections;

public class ServiceBrokerController : MonoBehaviour
{
	public int highlightPort;
	int currentHighlightPort;

	ServerController[] servers;

	void Awake()
	{
		RefreshServers ();
	}

	void Update()
	{
		HighlightPort ();
	}

	void HighlightPort()
	{
		if (highlightPort != currentHighlightPort) {
			for (int i = 0; i < servers.Length; i++) {
				servers [i].highlightPort = highlightPort;
			}
			currentHighlightPort = highlightPort;
		}
	}

	void RefreshServers()
	{
		GameObject[] serverObjects = GameObject.FindGameObjectsWithTag ("Server");
		servers = new ServerController[serverObjects.Length];
		for (int i = 0; i < serverObjects.Length; i++) {
			servers [i] = serverObjects [i].GetComponent<ServerController> ();
		}
	}
}
