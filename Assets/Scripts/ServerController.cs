using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServerController : Controller<DingoApplication>
{
	public float waitToRecreate = 3f;

	public override void OnNotification(string p_event, Object p_target, params object[] p_data) {
		ServerView serverView = null;
		TileSlotView tileSlot = null;
		switch (p_event) {
		case "server.recreate.request":
			serverView = (ServerView)p_target;
			RecreateServer (serverView);
			break;
		case "tileslot.delete.request":
			tileSlot = (TileSlotView)p_target;
			DeleteTileSlot (tileSlot);
			break;
		case "tileslot.restore.request":
			tileSlot = (TileSlotView)p_target;
			RestoreTileSlot (tileSlot);
			break;
		}
	}

	void RecreateServer(ServerView serverView)
	{
		for (int i = 0; i < serverView.TileSlots.Length; i++) {
			TileSlotView tileSlot = serverView.TileSlots [i];
			if (tileSlot.allocatedServiceInstance != null) {
				StartCoroutine(RecreateTileSlot (tileSlot));
			}
		}
	}

	IEnumerator RecreateTileSlot(TileSlotView tileSlot) {
		app.Notify ("tileslot.delete.request", tileSlot);
		yield return new WaitForSeconds(waitToRecreate);
		app.Notify ("tileslot.restore.request", tileSlot);
	}

	void DeleteTileSlot(TileSlotView tileSlot) {
		ServiceInstanceModel serviceInstance = tileSlot.allocatedServiceInstance;
		if (tileSlot.isLeader) {
			Debug.Log ("Failing over leader " + tileSlot.allocatedServiceInstance);
			serviceInstance.leaderServer = serviceInstance.replicaServer;
//			serviceInstance.replicaServer = serviceInstance.leaderServer;
//			serviceInstance.replicaServer.available = false;
			serviceInstance.replicaServer = null;
		} else {
			Debug.Log ("Recreating replica " + tileSlot.allocatedServiceInstance);
//			serviceInstance.replicaServer.available = false;
			serviceInstance.replicaServer = null;
		}
		tileSlot.allocatedServiceInstance = null;
		app.Notify("service-instance.update.request", serviceInstance);
	}

	void RestoreTileSlot(TileSlotView tileSlot) {
	}
}
