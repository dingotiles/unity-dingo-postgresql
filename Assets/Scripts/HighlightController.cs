using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class HighlightController : Controller<DingoApplication> {
	ArrayList latestHighlighted;

	void Awake() {
		latestHighlighted = new ArrayList ();
	}

	public override void OnNotification(string p_event, Object p_target, params object[] p_data) {
		ServiceInstanceModel serviceInstanceModel;
		switch (p_event) {
		case "service-instance.change.highlight":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnChangeHighlight (serviceInstanceModel);
			break;
		case "highlight.reset-all":
			ResetAllHighlights();
			break;
		}
	}

	void OnChangeHighlight(ServiceInstanceModel model) {
		Dictionary<ServiceInstanceModel, DingoController.TileSlotReference> tileSlotCache = app.controller.tileSlotCache;
		if (!tileSlotCache.ContainsKey(model)) {
			Debug.LogError ("Tile slot cache does not include " + model);
			Debug.Log ("Cache keys: " + tileSlotCache.Keys.Count);
			return;
		}
		DingoController.TileSlotReference tileSlots = tileSlotCache [model];
		if (tileSlots != null) {
			if (tileSlots.leader != null) {
				tileSlots.leader.allocatedServiceInstance = model;
				tileSlots.leader.highlightContents = model.highlight;
				tileSlots.leader.trafficFlowTarget = tileSlots.replica;
			}
			if (tileSlots.replica != null) {
				tileSlots.replica.allocatedServiceInstance = model;
				tileSlots.replica.highlightContents = model.highlight;
			}
			if (tileSlots.router != null) {
				tileSlots.router.allocatedServiceInstance = model;
				tileSlots.router.highlightContents = model.highlight;
				tileSlots.router.trafficFlowTarget = tileSlots.leader;
			}
		}
		if (model.highlight) {
			if (latestHighlighted.Contains (model)) {
				latestHighlighted.Remove (model);
			}
			latestHighlighted.Add (model);
			if (latestHighlighted.Count > app.model.ServiceInstances.maxHighlighted) {
				ServiceInstanceModel oldestHighlight = latestHighlighted [0] as ServiceInstanceModel;
				oldestHighlight.highlight = false;
			}
		} else {
			latestHighlighted.Remove (model);
		}
	}

	void ResetAllHighlights()
	{
		latestHighlighted = new ArrayList ();
		for (int i = 0; i < app.model.ServiceInstances.ServiceInstances.Length; i++) {
			app.model.ServiceInstances.ServiceInstances [i].highlight = false;
		}
	}
}
