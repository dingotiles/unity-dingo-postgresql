using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DataFlowController : Controller<DingoApplication> {
	public GameObject baseBackupPrefab;
	public GameObject walPrefab;
	public GameObject dataPrefab;
	public GameObject replicaBackupPrefab { get { return baseBackupPrefab; } }
	public GameObject replicaWALPrefab { get { return walPrefab; } }

	public override void OnNotification(string p_event, Object p_target, params object[] p_data) {
		ServiceInstanceModel serviceInstanceModel;
		switch (p_event) {
		case "data-flow.base-backup.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnRequestToCloud (serviceInstanceModel, baseBackupPrefab);
			OnRequestToReplica (serviceInstanceModel, replicaBackupPrefab);
			break;
		case "data-flow.restoration.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnRequestFromCloud (serviceInstanceModel, baseBackupPrefab);
			break;
		case "data-flow.wal.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnRequestToCloud (serviceInstanceModel, walPrefab);
			OnRequestToReplica (serviceInstanceModel, replicaWALPrefab);
			break;
		case "data-flow.data.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnRequestToSendData (serviceInstanceModel, dataPrefab);
			break;
		}
	}

	void OnRequestToCloud(ServiceInstanceModel serviceInstanceModel, GameObject prefab)
	{
		TileSlotView fromTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "leader");

		if (fromTileSlotView == null) {
			Debug.Log ("Cannot show data-flow: cannot find leader for " + serviceInstanceModel);
			return;
		}
		GameObject toCloud = app.view.ShowCloudOverLeader (fromTileSlotView);

		GameObject dataFlowObj = Instantiate (prefab) as GameObject;
		dataFlowObj.transform.position = fromTileSlotView.database.sphere.gameObject.transform.position;
		dataFlowObj.transform.localRotation = Quaternion.Euler (0f, 0f, 20f);

		DataFlowMover mover = dataFlowObj.GetComponent<DataFlowMover> ();
		mover.to = toCloud.transform;

		toCloud.GetComponent<CloudView> ().IncomingObject (dataFlowObj);
	}

	void OnRequestFromCloud(ServiceInstanceModel serviceInstanceModel, GameObject prefab)
	{
		TileSlotView toTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "leader");

		if (toTileSlotView == null) {
			Debug.Log ("Cannot show data-flow: cannot find leader for " + serviceInstanceModel);
			return;
		}
		GameObject toSphere = toTileSlotView.database.sphere.gameObject;
		GameObject fromCloud = app.view.ShowCloudOverLeader (toTileSlotView);

		GameObject dataFlowObj = Instantiate (prefab) as GameObject;
		dataFlowObj.transform.position = fromCloud.gameObject.transform.position;
		dataFlowObj.transform.localRotation = Quaternion.Euler (0f, 0f, 20f);

		DataFlowMover mover = dataFlowObj.GetComponent<DataFlowMover> ();
		mover.to = toSphere.transform;

		toSphere.GetComponent<DatabaseSphereView> ().IncomingObject (dataFlowObj);
	}

	void OnRequestToReplica(ServiceInstanceModel serviceInstanceModel, GameObject prefab)
	{
		TileSlotView fromTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "leader");
		TileSlotView toTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "replica");

		if (fromTileSlotView == null) {
			Debug.Log ("Cannot show data-flow: cannot find leader tile slot for " + serviceInstanceModel);
			return;
		}
		if (toTileSlotView == null) {
			Debug.Log ("Cannot show data-flow: cannot find replica tile slot for " + serviceInstanceModel);
			return;
		}

		GameObject fromSphere = fromTileSlotView.database.sphere.gameObject;
		GameObject toSphere = toTileSlotView.database.sphere.gameObject;

		GameObject dataFlowObj = Instantiate (prefab) as GameObject;
		dataFlowObj.transform.position = fromSphere.transform.position;
		dataFlowObj.transform.localRotation = Quaternion.Euler (0f, 0f, 20f);

		DataFlowMover mover = dataFlowObj.GetComponent<DataFlowMover> ();
		mover.to = toSphere.transform;

		toSphere.GetComponent<DatabaseSphereView> ().IncomingObject (dataFlowObj);
	}

	void OnRequestToSendData(ServiceInstanceModel serviceInstanceModel, GameObject prefab)
	{
		TileSlotView fromTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "router");
		TileSlotView toTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "leader");

		if (fromTileSlotView == null) {
			Debug.Log ("Cannot show data-flow: cannot find router tile slot for " + serviceInstanceModel);
			return;
		}
		if (toTileSlotView == null) {
			Debug.Log ("Cannot show data-flow: cannot find leader tile slot for " + serviceInstanceModel);
			return;
		}

		GameObject fromSphere = fromTileSlotView.database.sphere.gameObject;
		GameObject toSphere = toTileSlotView.database.sphere.gameObject;

		GameObject dataFlowObj = Instantiate (prefab) as GameObject;
		dataFlowObj.transform.position = fromSphere.transform.position;
		dataFlowObj.transform.localRotation = Quaternion.Euler (0f, 0f, 20f);

		DataFlowMover mover = dataFlowObj.GetComponent<DataFlowMover> ();
		mover.to = toSphere.transform;

		toSphere.GetComponent<DatabaseSphereView> ().IncomingObject (dataFlowObj);
	}

}
