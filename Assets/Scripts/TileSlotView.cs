using UnityEngine;
using System.Collections;
using thelab.mvc;

public class TileSlotView : View<DingoApplication> {

	public bool visible;
	public bool highlightContents;
	// If this TileSlowView contains a leader, then trafficFlowTarget contains the target replica
	// If this TileShowView is a router, then trafficFlowTarget is leader
	public TileSlotView trafficFlowTarget;
	public bool isLeader;
	public bool isRouter;
	public bool isRole(string role)
	{
		if (role == "leader") {
			return isLeader;
		}
		if (role == "router") {
			return isRouter;
		}
		if (role == "replica") {
			return !isLeader && !isRouter;
		}
		Debug.Log ("Unknown role: " + role);
		return false;
	}

	public DatabaseHighlight database { get { return p_database = base.Assert<DatabaseHighlight> (p_database); } }
	DatabaseHighlight p_database;

	float contentsHiddenY { get { return app.view.svrContentsHiddenY; } }
	float contentsTopY { get { return app.view.svrContentsTopY; } }
	float contentsLiftRate { get { return app.view.svrContentsLiftRate; } }
	GameObject contentsPrefab { get { return app.view.svrContentsPrefab; } }

	GameObject contents;

	GameObject cloudPrefab { get { return app.view.cloudPrefab; } }
	float cloudY { get { return app.view.cloudY; } }
	GameObject cloudObject;

	public ServiceInstanceModel allocatedServiceInstance = null;
	MeshRenderer meshRenderer;

	float activateActionTimer;
	float timeBetweenActivateAction = 0.15f;

	void Awake() {
		meshRenderer = GetComponentInChildren<MeshRenderer> ();
	}

	void Update () {
		activateActionTimer += Time.deltaTime;
		if (visible || allocatedServiceInstance != null) {
			ShowContents ();
		} else {
			HideContents ();
		}
		SetContentsHighlight (highlightContents);
	}

	void HideContents()
	{
		if (contents) {
			if (contents.transform.localPosition.y > contentsHiddenY) {
				contents.transform.Translate (-Vector3.up * contentsLiftRate * Time.deltaTime);
			} else {
				Destroy (this.contents);
			}
		}
	}

	void ShowContents()
	{
		CreateContentsIfMissing ();
		contents.SetActive (true);
		if (contents.transform.localPosition.y < contentsTopY) {
			contents.transform.Translate (Vector3.up * contentsLiftRate * Time.deltaTime);
		}
	}

	public void SetContentsHighlight(bool highlight)
	{
		if (contents != null) {
			if (trafficFlowTarget != null) {
				database.replica = trafficFlowTarget.database;
			}
			database.SetHighlight (highlight);
		}
	}

	void CreateContentsIfMissing ()
	{
		if (contents == null) {
			contents = Instantiate (contentsPrefab, transform) as GameObject;
			contents.transform.localPosition = new Vector3 (0f, contentsHiddenY, 0f);
			contents.transform.localRotation = Quaternion.identity;
			contents.transform.localScale = contentsPrefab.transform.localScale;
		}
	}

	public bool IsAllocatedServiceInstance(ServiceInstanceModel serviceInstance) {
		return (allocatedServiceInstance != null && allocatedServiceInstance.port == serviceInstance.port);
	}

	public GameObject ShowCloud()
	{
		if (cloudObject == null) {
			cloudObject = Instantiate (cloudPrefab, transform) as GameObject;
			cloudObject.transform.localPosition = new Vector3 (0, cloudY, 0);
			Debug.Log (cloudPrefab.transform.localScale);
			cloudObject.transform.localScale = new Vector3(2f, 1.5f, 2f);
		}
		return cloudObject;
	}

	public void EnableCursor(bool enable)
	{
		if (enable) {
			if (meshRenderer.sharedMaterial != app.view.tileSlotCursor) {
				app.view.ClearCursors ();
				meshRenderer.sharedMaterial = app.view.tileSlotCursor;
			}
			if(Input.GetButton ("Fire1") && activateActionTimer >= timeBetweenActivateAction && Time.timeScale != 0)
			{
				ActivateAction ();
			}

		} else {
			meshRenderer.sharedMaterial = app.view.tileSlotDefault;
		}
	}

	// Generic TileSlotView can capture clicks, but defers to the parent ServerView's controller to determine
	// what a click means for a blank or allocated TileSlotView
	void ActivateAction()
	{
		activateActionTimer = 0f;
		RouterView router = GetComponentInParent<RouterView> ();
		if (router != null) {
			router.ActivateAction (this);
		} else {
			ServerView server = GetComponentInParent<ServerView> ();
			server.ActivateAction (this);
		}

	}
}
