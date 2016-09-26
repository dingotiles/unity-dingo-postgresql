using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DingoView : View<DingoApplication> {

	public float sphereLowY = 0.9f;
	public float sphereHighY = 1.74f;
	public float cloudY = 40f;
	public float cloudWaitForDeath = 5f;
	public GameObject svrContentsPrefab;
	public float scale { get { return transform.localScale.x; } }
	public float svrContentsTopY { get { return 0.2f; } }
	public float svrContentsHiddenY { get { return -1f; } }
	public float svrContentsLiftRate { get { return 1.5f; } }
	public GameObject svrPrefab;
	public GameObject cloudPrefab;

	public Material serverCursor;
	public Material tileSlotDefault;
	public Material tileSlotCursor;

	public AvailabilityZonesView AvailabilityZones { get { return p_azs = base.Assert<AvailabilityZonesView> (p_azs); } }
	AvailabilityZonesView p_azs;

	public RouterView Router { get { return p_router = base.Assert<RouterView> (p_router); } }
	RouterView p_router;

	void Update()
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.x);
	}

	public TileSlotView FindTileSlot(ServiceInstanceModel serviceInstance, string role)
	{
		TileSlotView[] slots = transform.GetComponentsInChildren<TileSlotView> ();
		for (int i = 0; i < slots.Length; i++) {
			if (slots [i].allocatedServiceInstance != null && 
				slots [i].allocatedServiceInstance.Equals(serviceInstance) &&
				slots [i].isRole(role)) {
				return slots [i];
			}
		}
		return null;
	}

	public GameObject ShowCloudOverLeader (TileSlotView tileSlot)
	{
		return tileSlot.ShowCloud ();
	}

	public void ClearCursors() 
	{
		TileSlotView[] slots = transform.GetComponentsInChildren<TileSlotView> ();
		for (int i = 0; i < slots.Length; i++) {
			slots [i].EnableCursor (false);
		}
		ServerCursor[] servers = transform.GetComponentsInChildren<ServerCursor> ();
		for (int i = 0; i < servers.Length; i++) {
			servers [i].EnableCursor (false);
		}
	}
}
