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
			StartCoroutine( RecreateServer (serverView));
			break;
		}
	}

	IEnumerator RecreateServer(ServerView serverView)
	{
		ArrayList inProgressServiceInstances = new ArrayList ();
		for (int i = 0; i < serverView.TileSlots.Length; i++) {
			TileSlotView tileSlot = serverView.TileSlots [i];
			if (tileSlot.allocatedServiceInstance != null) {
				inProgressServiceInstances.Add (tileSlot.allocatedServiceInstance);
				DeleteTileSlot (tileSlot);
			}
		}
		yield return new WaitForSeconds(waitToRecreate);
		RestoreServer (serverView, inProgressServiceInstances);
	}

	void DeleteTileSlot(TileSlotView tileSlot) {
		ServiceInstanceModel serviceInstance = tileSlot.allocatedServiceInstance;
		if (tileSlot.isLeader) {
			Debug.Log ("Failing over leader " + tileSlot.allocatedServiceInstance);
			serviceInstance.recreationInProgressServer = serviceInstance.leaderServer;
			serviceInstance.leaderServer = serviceInstance.replicaServer;
			serviceInstance.replicaServer = null;
		} else {
			Debug.Log ("Recreating replica " + tileSlot.allocatedServiceInstance);
			serviceInstance.recreationInProgressServer = serviceInstance.replicaServer;
			serviceInstance.replicaServer = null;
		}
		tileSlot.allocatedServiceInstance = null;
		app.Notify("service-instance.update.request", serviceInstance);
	}

	void RestoreServer(ServerView serverView, ArrayList inProgressServiceInstances) {
		for (int i = 0; i < inProgressServiceInstances.Count; i++) {
			ServiceInstanceModel serviceInstance = inProgressServiceInstances [i] as ServiceInstanceModel;
			serviceInstance.replicaServer = serviceInstance.recreationInProgressServer;
			Debug.Log ("restoring " + serviceInstance);
			serviceInstance.recreationInProgressServer = null;
			app.Notify("service-instance.update.request", serviceInstance);
		}
	}
}
