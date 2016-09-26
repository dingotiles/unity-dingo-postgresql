using UnityEngine;
using System.Collections;
using thelab.mvc;

public class ServerTilePlacement : View<DingoApplication>
{
	public GameObject tileSlotPrefab;

	void Awake() {
		for (int i = 0; i < 14; i++) {
			Vector3 tileSlotPosition = TileSlotLayoutPosition (i);
			GameObject tileSlot = Instantiate (tileSlotPrefab, transform) as GameObject;
			tileSlot.transform.localPosition = tileSlotPosition;
			tileSlot.transform.localScale = tileSlotPrefab.transform.localScale;
			tileSlot.name = "Tile " + i;
		}
	}
		

	Vector3 TileSlotLayoutPosition(int index) {
		float localY = -0.4f;
		switch (index) {
		case 0:
			return new Vector3 (0.51f, localY, 2.38f);
		case 1:
			return new Vector3 (1.56f, localY, 2.38f);
		case 2:
			return new Vector3 (0f, localY, 1.587f);
		case 3:
			return new Vector3 (1.05f, localY, 1.587f);
		case 4:
			return new Vector3 (2.11f, localY, 1.587f);
		case 5:
			return new Vector3 (-0.55f, localY, 0.795f);
		case 6:
			return new Vector3 (0.51f, localY, 0.795f);
		case 7:
			return new Vector3 (1.57f, localY, 0.795f);
		case 8:
			return new Vector3 (2.64f, localY, 0.795f);
		case 9:
			return new Vector3 (0f, localY, 0f);
		case 10:
			return new Vector3 (1.03f, localY, 0f);
		case 11:
			return new Vector3 (2.08f, localY, 0f);
		case 12:
			return new Vector3 (0.51f, localY, -0.81f);
		case 13:
			return new Vector3 (1.56f, localY, -0.81f);
		}
		Debug.Log ("TileSlotLayoutPosition(" + index + ") is not valid; valid range 0 to 13");
		return new Vector3 (0f, 0f, 0f);
	}
}
