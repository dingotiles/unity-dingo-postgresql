using UnityEngine;
using System.Collections;
using thelab.mvc;

public class RecreateServiceInstanceController : Controller<DingoApplication> 
{
	public float waitToRecreate = 5f;

	public override void OnNotification(string p_event, Object p_target, params object[] p_data)
	{
		ServiceInstanceModel serviceInstanceModel;
		switch (p_event) {
		case "service-instance.recreate.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			StartCoroutine(OnRecreate (serviceInstanceModel));
			break;

		}
	}

	// Reset/lower TileSlotViews in AZs
	// Recreate TileSlotViews again across AZs
	// Show Cloud
	// Lower Database Backup down to leader
	// Lower WAL segments down to leader
	// Leader -> cloud new base backup
	// Leader -> replica new base backup
	IEnumerator OnRecreate(ServiceInstanceModel serviceInstanceModel)
	{
		app.Notify ("service-instance.delete.request", serviceInstanceModel);
		yield return new WaitForSeconds(waitToRecreate);
		app.Notify ("service-instance.create.request", serviceInstanceModel);
		yield return new WaitForSeconds(1f);
		app.Notify ("data-flow.restoration.request", serviceInstanceModel);
		float waitToBackup = serviceInstanceModel.walSegments * 1.5f + 2f;
		yield return new WaitForSeconds(waitToBackup);
		app.Notify ("data-flow.base-backup.request", serviceInstanceModel);
		yield return new WaitForSeconds(1f);
		app.Notify ("data-flow.replica-backup.request", serviceInstanceModel);
	}
}
