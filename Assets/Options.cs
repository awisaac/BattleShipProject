using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options : MonoBehaviour {

	GameObject ocean;
	Draggable[] allShips;
	GameObject DockButton;

	// Use this for initialization
	void Start () {

		ocean = GameObject.Find("Ocean");
		allShips = GameObject.Find("ShipPanel").GetComponentsInChildren<Draggable> ();	
		DockButton = GameObject.Find ("DockButton");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnConfirm()
	{
		// unfreeze camera when choice is made
		ocean.GetComponent<ZoomClick>().enabled = true;
		GameObject.Find("ZoomButton").GetComponent<Button>().interactable = true;
		
		// unlock dock when choice is made
		foreach (Draggable drag in allShips)
		{
			drag.enabled = true;
		}

		ocean.GetComponent<Ships>().shipCount++;
		gameObject.SetActive (false);
	}

	public void OnCancel()
	{
		// unfreeze camera when choice is made
		ocean.GetComponent<ZoomClick>().enabled = true;
		GameObject.Find("ZoomButton").GetComponent<Button>().interactable = true;
		
		// unlock dock when choice is made
		foreach (Draggable drag in allShips)
		{
			drag.enabled = true;
		}

		ocean.GetComponent<Ships>().stickyShip = true;
		gameObject.SetActive (false);
		DockButton.GetComponent<CanvasGroup> ().alpha = 1;
		DockButton.GetComponent<Button> ().interactable = true;
	}
}
