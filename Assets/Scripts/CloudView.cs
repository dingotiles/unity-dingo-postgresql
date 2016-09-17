using UnityEngine;
using System.Collections;
using thelab.mvc;

public class CloudView : View<DingoApplication> {
	float waitForDestroy { get { return app != null ? app.view.cloudWaitForDeath : 5f; } }
	float timeLeftForDestroy;

	ArrayList incomingParentObjects;

	void Awake() {
		incomingParentObjects = new ArrayList ();
		timeLeftForDestroy = waitForDestroy;
	}

	void Update() {
		timeLeftForDestroy -= Time.deltaTime;
		if (timeLeftForDestroy <= 0.0f)
		{
			Destroy (this.gameObject);
		}
	}

	void OnTriggerEnter(Collider collider) {
		DataFlowMover parent = collider.GetComponentInParent<DataFlowMover> ();
		if (parent != null) {
			Destroy (collider.gameObject);
		}
		if (incomingParentObjects.Contains(collider.gameObject)) {
			incomingParentObjects.Remove (collider.gameObject);
		}
		if (incomingParentObjects.Count == 0) {
			RestartDeathTimer ();
		}
	}

	// Do not self destruct until all incoming objects have been received/destroy,
	// and waited a pause period.
	public void IncomingObject(GameObject parentObj) {
		incomingParentObjects.Add (parentObj);
		RestartDeathTimer ();
	}

	void RestartDeathTimer() {
		timeLeftForDestroy = waitForDestroy;
	}
}
