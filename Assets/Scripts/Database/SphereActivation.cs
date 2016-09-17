using UnityEngine;
using System.Collections;

public class SphereActivation : MonoBehaviour {
	public bool visible;
	public float lowY, highY;
	public GameObject outboundTarget;

	LineRenderer lineWaveRenderer;
	LineWave lineWave;

	void Awake()
	{
		lineWave = GetComponentInChildren<LineWave> ();
		if (lineWave != null) {
			lineWaveRenderer = lineWave.gameObject.GetComponent<LineRenderer> ();
		}
	}

	void Update ()
	{
		if (visible) {
			if (transform.localPosition.y < highY) {
				transform.Translate (Vector3.up * Time.deltaTime);
			} else {
				if (lineWaveRenderer != null && outboundTarget != null) {
					lineWaveRenderer.enabled = true;
					lineWave.targetOptional = outboundTarget;
				}
			}
		} else {
			if (transform.localPosition.y > lowY) {
				transform.Translate (-Vector3.up * Time.deltaTime);
			}
			if (lineWaveRenderer != null && outboundTarget != null) {
				lineWaveRenderer.enabled = false;
				lineWave.targetOptional = null;
			}
		}
	}
}
