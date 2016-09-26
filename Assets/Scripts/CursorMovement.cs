using UnityEngine;
using System.Collections;
using thelab.mvc;


public class CursorMovement : Controller<DingoApplication> {
	LayerMask tileSlotMask;
	float camRayLength = 100f;

	void Awake()
	{
		tileSlotMask = LayerMask.GetMask ("Tile");
	}

	void Update()
	{
		ShowCursor ();
	}

	void ShowCursor()
	{
        RaycastHit tileSlotHit;
#if UNITY
        Ray cameraRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        var raycast = Physics.Raycast (cameraRay, out tileSlotHit, camRayLength, tileSlotMask);
#else
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        var raycast = Physics.Raycast(headPosition, gazeDirection, out tileSlotHit, camRayLength, tileSlotMask);
#endif

        if (raycast) {
			TileSlotView tileSlot = tileSlotHit.collider.gameObject.GetComponentInParent<TileSlotView> ();
			if (tileSlot != null) {
				tileSlot.EnableCursor (true);
			}
			ServerCursor server = tileSlotHit.collider.gameObject.GetComponent<ServerCursor> ();
			if (server != null) {
				server.EnableCursor (true);
			}
		} else {
			app.view.ClearCursors ();
		}
	}
}
