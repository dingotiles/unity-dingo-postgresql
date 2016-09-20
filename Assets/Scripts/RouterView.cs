using UnityEngine;
using System.Collections;
using thelab.mvc;

public class RouterView : View<DingoApplication>
{
	public ServerView RouterServerView { get { return p_server_view = gameObject.GetComponent<ServerView> (); } }
	ServerView p_server_view;

	public void EnableRoutingViews(ServiceInstanceModel[] serviceInstances)
	{
		for (int j = 0; j < serviceInstances.Length; j++) {
			TileSlotView allocated = RouterServerView.FindOrAllocateTileSlot (serviceInstances[j]);
			if (allocated != null) {
				allocated.isRouter = true;
				Debug.Log (allocated);
			}
		}
	}
}
