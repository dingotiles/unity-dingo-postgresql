using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServerCursor : View<DingoApplication>
{

	MeshRenderer meshRenderer;
	float activateActionTimer;
	float timeBetweenActivateAction = 0.15f;
	Material defaultMaterial;

	void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer> ();
		defaultMaterial = meshRenderer.sharedMaterial;
	}

	void Update ()
	{
		activateActionTimer += Time.deltaTime;
	}

	public void EnableCursor(bool enable)
	{
		if (enable) {
			if (meshRenderer.sharedMaterial != app.view.serverCursor) {
				app.view.ClearCursors ();
				meshRenderer.sharedMaterial = app.view.serverCursor;
                app.Notify("gesture.focused-object.changed", gameObject);
            }
            if (Input.GetButton ("Fire1") && activateActionTimer >= timeBetweenActivateAction && Time.timeScale != 0)
			{
				ActivateAction ();
			}
		} else {
			meshRenderer.sharedMaterial = defaultMaterial;
		}
	}

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
