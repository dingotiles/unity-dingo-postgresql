using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DatabaseSphereView : View<DingoApplication> {
	public bool visible;
	public GameObject outboundTarget;

	float lowY { get { return app.view.sphereLowY; } }
	float highY { get { return app.view.sphereHighY; } }

	LineRenderer lineWaveRenderer;
	LineWave lineWave;

	ArrayList incomingParentObjects;

	void Awake()
	{
		lineWave = GetComponentInChildren<LineWave> ();
		if (lineWave != null) {
			lineWaveRenderer = lineWave.gameObject.GetComponent<LineRenderer> ();
		}
		incomingParentObjects = new ArrayList ();
	}

	void Update ()
	{
		if (visible) {
			if (transform.localPosition.y < highY) {
				transform.localPosition += Vector3.up * Time.deltaTime;
			} else {
				if (lineWaveRenderer != null && outboundTarget != null) {
					lineWaveRenderer.enabled = true;
					lineWave.targetOptional = outboundTarget;
					lineWave.traceWidth = 0.5f * app.view.scale;
					lineWave.transform.localScale = new Vector3(1/(app.view.scale*0.5f), 1f, 1/(app.view.scale*0.5f));
				}
			}
		} else {
			if (transform.localPosition.y > lowY) {
				transform.Translate (-Vector3.up * Time.deltaTime);
			}
			lineWaveRenderer.enabled = false;
			lineWave.targetOptional = null;
		}
		if (outboundTarget == null) {
			lineWaveRenderer.enabled = false;
			lineWave.targetOptional = null;
		}
	}

	// Destroy inbound data flow (backups) objects
	void OnTriggerEnter(Collider collider) {
		DataFlowMover parent = collider.GetComponentInParent<DataFlowMover> ();
		if (parent != null) {
			if (incomingParentObjects.Contains (parent.gameObject)) {
				Destroy (parent.gameObject);
				incomingParentObjects.Remove (parent.gameObject);
			}
		} else {
			if (incomingParentObjects.Contains (collider.gameObject)) {
				Destroy (collider.gameObject);
				incomingParentObjects.Remove (collider.gameObject);
			}
		}
	}

	// Do not self destruct until all incoming objects have been received/destroy,
	// and waited a pause period.
	public void IncomingObject(GameObject parentObj) {
		incomingParentObjects.Add (parentObj);
	}
}
