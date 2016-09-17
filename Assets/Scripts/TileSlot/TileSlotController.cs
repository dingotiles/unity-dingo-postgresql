using UnityEngine;
using System.Collections;

public class TileSlotController : MonoBehaviour {

	public float contentsHiddenY;
	public float contentsTopY;
	public float contentsLiftRate = 1.5f;
	public GameObject contentsPrefab;

	public bool visible;
	public bool highlightContents;

	GameObject contents;

	void Update () {
		if (visible) {
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
			DatabaseHighlight databaseHighlight = contents.GetComponent<DatabaseHighlight> ();
			databaseHighlight.SetHighlight (highlight);
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
}
