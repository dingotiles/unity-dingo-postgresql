using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class DingoController : Controller<DingoApplication> {
	public class TileSlotReference {
		public TileSlotView leader;
		public TileSlotView replica;
		public TileSlotView router;
	}

	public Dictionary<ServiceInstanceModel, TileSlotReference> tileSlotCache;

	void Awake() {
		app.view.AvailabilityZones.ConstructServerViews (app.model.Servers.servers);
		app.view.AvailabilityZones.EnableServerViews (app.model.Servers.servers);
		InitializeTileSlotCache ();
	}

	public override void OnNotification(string p_event, Object p_target, params object[] p_data) {
		ServiceInstanceModel serviceInstanceModel;
		switch (p_event) {
		case "service-instance.create.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnCreated (serviceInstanceModel);
			break;
		case "service-instance.update.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnUpdated (serviceInstanceModel);
			break;
		case "service-instance.delete.request":
			serviceInstanceModel = (ServiceInstanceModel)p_target;
			OnDeleted (serviceInstanceModel);
			break;

		}
	}

	void OnCreated(ServiceInstanceModel serviceInstanceModel) {
		serviceInstanceModel.AssignServers ();
		app.view.AvailabilityZones.EnableServerContentsViews (app.model.ServiceInstances.ServiceInstances);
		app.view.Router.EnableRoutingViews (app.model.ServiceInstances.ServiceInstances);
		InitializeTileSlotCache ();
		// Ensure highlighting kicks in if necessary
		app.Notify ("service-instance.change.highlight", serviceInstanceModel);
	}

	void OnUpdated(ServiceInstanceModel serviceInstanceModel) {
		app.view.AvailabilityZones.EnableServerContentsViews (app.model.ServiceInstances.ServiceInstances);
		app.view.Router.EnableRoutingViews (app.model.ServiceInstances.ServiceInstances);
		InitializeTileSlotCache ();
		// Ensure highlighting kicks in if necessary
		app.Notify ("service-instance.change.highlight", serviceInstanceModel);
	}

	void OnDeleted(ServiceInstanceModel serviceInstanceModel) {
		TileSlotView fromTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "leader");
		TileSlotView toTileSlotView = app.view.FindTileSlot (serviceInstanceModel, "replica");
		if (fromTileSlotView == null) {
			Debug.Log ("Cannot recreate: cannot find leader tile slot for " + serviceInstanceModel);
			return;
		} else {
			fromTileSlotView.allocatedServiceInstance = null;
		}
		if (toTileSlotView == null) {
			Debug.Log ("Cannot recreate: cannot find replica tile slot for " + serviceInstanceModel);
			return;
		} else {
			toTileSlotView.allocatedServiceInstance = null;
		}
		InitializeTileSlotCache ();
	}

	void InitializeTileSlotCache() {
		tileSlotCache = new Dictionary<ServiceInstanceModel, TileSlotReference> ();
		for (int i = 0; i < app.view.AvailabilityZones.Zones.Length; i++) {
			AvailabilityZoneView zone = app.view.AvailabilityZones.Zones [i];
			for (int j = 0; j < zone.ServerViews.Length; j++) {
				ServerView server = zone.ServerViews [j];
				for (int k = 0; k < server.TileSlots.Length; k++) {
					TileSlotView tileSlot = server.TileSlots [k];
					if (tileSlot.allocatedServiceInstance != null) {
						if (!tileSlotCache.ContainsKey(tileSlot.allocatedServiceInstance)) {
							tileSlotCache [tileSlot.allocatedServiceInstance] = new TileSlotReference ();
						}
						TileSlotReference cache = tileSlotCache [tileSlot.allocatedServiceInstance];
						ServersModel.Server leader = tileSlot.allocatedServiceInstance.leaderServer;
						ServersModel.Server replica = tileSlot.allocatedServiceInstance.replicaServer;
						if (leader != null && leader.az == zone.az && leader.serverLabel == server.label) {
							cache.leader = tileSlot;
							cache.leader.isLeader = true;
						} else if (replica != null && replica.az == zone.az && replica.serverLabel == server.label) {
							cache.replica = tileSlot;
						}
					}
				}
			}
		}
		for (int i = 0; i < app.view.Router.RouterServerView.TileSlots.Length; i++) {
			TileSlotView tileSlot = app.view.Router.RouterServerView.TileSlots [i];
			if (tileSlot.allocatedServiceInstance != null) {
				if (!tileSlotCache.ContainsKey(tileSlot.allocatedServiceInstance)) {
					tileSlotCache [tileSlot.allocatedServiceInstance] = new TileSlotReference ();
				}
				TileSlotReference cache = tileSlotCache [tileSlot.allocatedServiceInstance];
				cache.router = tileSlot;
			}
		}

	}
}
