using UnityEngine;
using System.Collections;
using thelab.mvc;

public class CloudView : View<DingoApplication> {
	float waitForDestroy { get { return app != null ? app.view.cloudWaitForDeath : 5f; } }
	ArrayList incomingParentObjects;

	void Awake() {
		incomingParentObjects = new ArrayList ();
	}

	void OnTriggerEnter(Collider collider) {
		Destroy (collider.gameObject);
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
		Debug.Log ("TODO: implement CloudView.RestartDeathTimer");
	}
}
