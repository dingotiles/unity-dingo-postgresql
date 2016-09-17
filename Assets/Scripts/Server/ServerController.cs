using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerController : MonoBehaviour {
	public TileSlotModel[] runningPorts;
	Dictionary<int, TileSlotController> currentRunningPorts;

	public int highlightPort;
	int currentHighlightPort;

	TileSlotController[] tileSlots;

	void Awake()
	{
		tileSlots = gameObject.GetComponentsInChildren<TileSlotController> ();
		currentRunningPorts = new Dictionary<int, TileSlotController>();
	}

	void Update() {
		// Reallocate any tile slots that are no longer required
		foreach(KeyValuePair<int, TileSlotController> entry in currentRunningPorts)
		{
			bool found = false;
			for (int i = 0; i < runningPorts.Length; i++) {
				int port = runningPorts [i].port;
				if (port == entry.Key) {
					found = true;
					break;
				}
			}
			if (!found) {
				DeallocatePort(entry.Key, entry.Value);
			}
		}

		// Allocate tile slots that are now required
		for (int i = 0; i < runningPorts.Length; i++) {
			int port = runningPorts [i].port;
			if (port > 0) {
				if (!currentRunningPorts.ContainsKey (port)) {
					AllocatePort (port);
				}
			}
		}

		HighlightPort ();
	}

	void AllocatePort(int port)
	{
		TileSlotController nextSlot = NextTileSlot ();
		if (nextSlot == null) {
			Debug.LogError ("No available tile slots to allocate port " + port);
			return;
		}
		AllocateTileSlot (nextSlot);
		currentRunningPorts [port] = nextSlot;
		Debug.Log ("Allocate tile for port " + port);
	}

	void DeallocatePort(int port, TileSlotController tileSlot)
	{
		DeallocateTileSlot (tileSlot);
		currentRunningPorts.Remove (port);
		Debug.Log ("Deallocate tile for port " + port);
	}

	void HighlightPort()
	{
		if (highlightPort > 0) {
			TileSlotController tileSlot = TileSlotForPort (highlightPort);
			if (tileSlot != null) {
				tileSlot.SetContentsHighlight (true);
				currentHighlightPort = highlightPort;
				return;
			}
		} else if (currentHighlightPort > 0) {
			TileSlotForPort (currentHighlightPort).SetContentsHighlight (false);
		}
		currentHighlightPort = 0;
	}

	void AllocateTileSlot(TileSlotController tileSlot)
	{
		if (tileSlot != null) {
			tileSlot.visible = true;
		}
	}

	void DeallocateTileSlot(TileSlotController tileSlot)
	{
		if (tileSlot != null) {
			tileSlot.visible = false;
		}
	}

	TileSlotController TileSlot(int index)
	{
		if (index >= 0 && index < tileSlots.Length) {
			return tileSlots [index];
		}
		return null;
	}

	TileSlotController TileSlotForPort(int port)
	{
		foreach(KeyValuePair<int, TileSlotController> entry in currentRunningPorts)
		{
			if (entry.Key == port) {
				return entry.Value;
			}
		}
		return null;
	}


	TileSlotController NextTileSlot()
	{
		for (int i = 0; i < tileSlots.Length; i++) {
			TileSlotController tileSlot = tileSlots [i];
			if (!tileSlot.visible) {
				return tileSlot;
			}
		}
		return null;
	}
}
