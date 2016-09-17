using UnityEngine;
using System.Collections;
using thelab.mvc;

public class DataFlowMover : View<DingoApplication> {
	public Transform to;
	public float speed = 1f;

	void Update()
	{
		if (to == null) {
			Destroy (this.gameObject);
			return;
		}

		transform.position = Vector3.MoveTowards(transform.position, to.transform.position, Time.deltaTime * speed);
	}

}
