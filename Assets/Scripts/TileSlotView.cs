﻿using UnityEngine;
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
	public float timeBetweenActivateAction = 0.2f;

	void Awake() {
		meshRenderer = GetComponent<MeshRenderer> ();
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
			this.contents = Instantiate (contentsPrefab, transform) as GameObject;
			this.contents.transform.localPosition = new Vector3 (0.006f, contentsHiddenY, -0.006f);;
			this.contents.transform.localRotation = Quaternion.identity;
			this.contents.transform.localScale = new Vector3 (0.92f, 6f, 0.92f);
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
		}
		return cloudObject;
	}

	public void EnableCursor(bool enable)
	{
		if (enable) {
			if (meshRenderer.sharedMaterial != app.view.tileSlotCursor) {
				app.view.ClearTileSlotCursors ();
				meshRenderer.sharedMaterial = app.view.tileSlotCursor;
			}
			if(Input.GetButton ("Fire1") && activateActionTimer >= timeBetweenActivateAction && Time.timeScale != 0)
			{
				ActivateActionOnServiceInstance ();
			}

		} else {
			meshRenderer.sharedMaterial = app.view.tileSlotDefault;
		}
	}

	void ActivateActionOnServiceInstance()
	{
		activateActionTimer = 0f;
		if (allocatedServiceInstance != null) {
			Debug.Log ("Activate action on: " + allocatedServiceInstance);
		} else {
			Debug.Log ("No allocated service instance model");
		}
	}
}
