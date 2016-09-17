using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DataFlowController : Controller<DingoApplication> {
	public GameObject baseBackupPrefab;
	public GameObject walPrefab;

	public override void OnNotification(string p_event, Object p_target, params object[] p_data) {
		ServiceInstanceModel serviceInstanceModel;
		switch (p_event) {
		case "data-flow.base-backup.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnRequest (serviceInstanceModel, baseBackupPrefab);
			break;
		case "data-flow.wal.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnRequest (serviceInstanceModel, walPrefab);
			break;
		}
	}

	void OnRequest(ServiceInstanceModel serviceInstanceModel, GameObject prefab)
	{
		TileSlotView fromTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "leader");

		if (fromTileSlotView != null) {
			GameObject cloudObj = app.view.ShowCloudOverLeader (fromTileSlotView);

			GameObject dataFlowObj = Instantiate (prefab) as GameObject;
			dataFlowObj.transform.position = fromTileSlotView.database.sphere.gameObject.transform.position;
			dataFlowObj.transform.localRotation = Quaternion.Euler (0f, 0f, 20f);
			DataFlowMover mover = dataFlowObj.GetComponent<DataFlowMover> ();
			mover.to = cloudObj.transform;

			cloudObj.GetComponent<CloudView> ().IncomingObject (dataFlowObj);
		} else {
			Debug.Log ("Cannot show data-flow: cannot find leader for " + serviceInstanceModel);
		}
	}

}
