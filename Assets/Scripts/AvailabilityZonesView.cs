using UnityEngine;
using System.Collections;
using thelab.mvc;

public class AvailabilityZonesView : View<DingoApplication> {
	public AvailabilityZoneView[] Zones { 
		get {
			if (p_zones != null)
				return p_zones;
			return p_zones = transform.GetComponentsInChildren<AvailabilityZoneView> ();
		}
	}
	AvailabilityZoneView[] p_zones;

	public void ConstructServerViews(ServersModel.Server[] servers) {
		for (int i = 0; i < app.view.AvailabilityZones.Zones.Length; i++) {
			app.view.AvailabilityZones.Zones [i].ConstructServerViews (servers);
		}
	}

	public void EnableServerViews(ServersModel.Server[] servers) {
		for (int i = 0; i < app.view.AvailabilityZones.Zones.Length; i++) {
			app.view.AvailabilityZones.Zones [i].EnableServerViews (servers);
		}
	}

	public void EnableServerContentsViews(ServiceInstanceModel[] serviceInstances) {
		for (int i = 0; i < app.view.AvailabilityZones.Zones.Length; i++) {
			app.view.AvailabilityZones.Zones [i].EnableServerContentsViews (serviceInstances);
		}
	}

}
