using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class ServerView : View<DingoApplication> {
	public string label;

	public TileSlotView[] TileSlots { 
		get {
			if (p_tile_slots != null)
				return p_tile_slots;
			return p_tile_slots = transform.GetComponentsInChildren<TileSlotView> ();
		}
	}
	TileSlotView[] p_tile_slots;

	public TileSlotView FindOrAllocateTileSlot(ServiceInstanceModel serviceInstance)
	{
		TileSlotView nextUnallocated = null;
		TileSlotView[] slots = transform.GetComponentsInChildren<TileSlotView> ();
		for (int i = 0; i < slots.Length; i++) {
			if (slots [i].allocatedServiceInstance != null && slots [i].allocatedServiceInstance.port == serviceInstance.port) {
				return slots [i];
			}
			if (nextUnallocated == null && slots [i].allocatedServiceInstance == null) {
				nextUnallocated = slots [i];
			}
		}
		if (nextUnallocated != null) {
			nextUnallocated.allocatedServiceInstance = serviceInstance;
		}
		return nextUnallocated;
	}
}
