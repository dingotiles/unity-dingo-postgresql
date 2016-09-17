using UnityEngine;
using System.Collections;
using thelab.mvc;

public class AvailabilityZoneView : View<DingoApplication>
{
	public ServersModel.AvailabilityZone az;

	public ServersGroupView ServersGroup { get { return p_servers_group = base.Assert<ServersGroupView> (p_servers_group); } }
	ServersGroupView p_servers_group;

	public ServerView[] ServerViews { 
		get {
			if (p_server_views != null) {
				return p_server_views;
			}
			return p_server_views = transform.GetComponentsInChildren<ServerView> ();
		}
	}
	ServerView[] p_server_views;

	public void ConstructServerViews(ServersModel.Server[] servers) {
		Transform serverViewsParent = ServersGroup.transform;
		int serverNum = 0;
		for (int i = 0; i < servers.Length; i++) {
			if (servers [i].az != az) {
				continue;
			}
			GameObject serverViewObj = Instantiate (app.view.svrPrefab, serverViewsParent) as GameObject; 
			serverViewObj.transform.localPosition = ServerViewLayoutPosition (serverNum);
			ServerView view = serverViewObj.GetComponent<ServerView> ();
			view.label = servers [i].serverLabel;
			serverNum++;
		}
	}

	public void EnableServerViews(ServersModel.Server[] servers) {
		for (int i = 0; i < ServerViews.Length; i++) {
			ServerView serverView = ServerViews [i];
			bool serverEnabled = false;
			for (int j = 0; j < servers.Length; j++) {
				ServersModel.Server server = servers[j];
				if (MatchesServerModel(server, serverView)) {
					serverEnabled = true;
					break;
				}
			}
			serverView.gameObject.SetActive (serverEnabled);
		}
	}

	public void EnableServerContentsViews(ServiceInstanceModel[] serviceInstances) {
		for (int i = 0; i < ServerViews.Length; i++) {
			ServerView serverView = ServerViews [i];
			if (!serverView.isActiveAndEnabled) {
				continue;
			}
			ServiceInstanceModel serviceInstance = null;
			for (int j = 0; j < serviceInstances.Length; j++) {
				serviceInstance = serviceInstances[j];
				if (MatchesServerModel(serviceInstance.leaderServer, serverView)) {
					serverView.FindOrAllocateTileSlot (serviceInstance);
				}
				if (MatchesServerModel(serviceInstance.replicaServer, serverView)) {
					serverView.FindOrAllocateTileSlot (serviceInstance);
				}
			}
		}
	}

	bool MatchesServerModel(ServersModel.Server model, ServerView view) {
		return model.az == az && model.serverLabel == view.label;
	}

	// TODO - move into AvailabilityZoneLayout14ServerView class 
	Vector3 ServerViewLayoutPosition(int index) {
		switch (index) {
		case 0:
			return new Vector3 (2.5f, 0f, 4.2f);
		case 1:
			return new Vector3 (5f, 0f, 0f);
		case 2:
			return new Vector3 (-2.5f, 0f, 4.2f);
		case 3:
			return new Vector3 (0f, 0f, 0f);
		case 4:
			return new Vector3 (2.5f, 0f, -4.2f);
		case 5:
			return new Vector3 (-5f, 0f, 0f);
		case 6:
			return new Vector3 (-2.5f, 0f, -4.2f);
		}
		return new Vector3 (0f, 0f, 0f);
	}
}