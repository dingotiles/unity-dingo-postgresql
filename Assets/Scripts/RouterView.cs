using UnityEngine;
using System.Collections;
using thelab.mvc;

public class RouterView : View<DingoApplication>
{
	public ServerView RouterServerView;

	void Awake()
	{ 
		RouterServerView = gameObject.GetComponent<ServerView> ();
	}

	public void EnableRoutingViews(ServiceInstanceModel[] serviceInstances)
	{
		for (int j = 0; j < serviceInstances.Length; j++) {
			TileSlotView allocated = RouterServerView.FindOrAllocateTileSlot (serviceInstances[j]);
			Debug.Log (allocated);
		}
	}
}
