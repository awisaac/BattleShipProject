using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options : MonoBehaviour {

	GameObject ocean;
	GameObject shipPanel;
	GameObject controlPanel;
	Draggable[] allShips;
	GameObject DockButton;
	GameObject ZoomButton;

	// Use this for initialization
	void Start () {

		ocean = GameObject.Find("Ocean");
		shipPanel = GameObject.Find ("ShipPanel");
		ZoomButton = ocean.GetComponent<ZoomClick> ().ZoomButton;
		allShips = shipPanel.GetComponentsInChildren<Draggable> ();	
		DockButton = GameObject.Find ("DockButton");
		controlPanel = GameObject.Find ("ControlPanel");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnConfirm()
	{
		// unfreeze camera when choice is made
		ZoomButton.GetComponent<Button>().interactable = true;
		
		// unlock dock when choice is made
		foreach (Draggable drag in allShips)
		{
			drag.enabled = true;
		}

		ocean.GetComponent<Ships> ().RemoveShipCollision ();
		ocean.GetComponent<Ships>().shipCount++;

		// Remove dock when all ships are placed
		if (ocean.GetComponent<Ships>().shipCount == 5) 
		{
			HideDock();
		}

		// Bring dock back for remaining ships
		else
		{
			controlPanel.SetActive (true);
		}

		GameObject currentShip = ocean.GetComponent<Ships> ().totalShips [ocean.GetComponent<Ships> ().shipCount - 1];
		Vector3 extents = currentShip.GetComponent<MeshRenderer> ().bounds.extents;

		ocean.GetComponent<FogOfWar> ().ShipSight (currentShip.transform.position, extents);
		ocean.GetComponent<ZoomClick> ().ZoomOutClick ();
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().interactable = false;	
	}

	public void OnCancel()
	{
		// unfreeze camera when choice is made
		ZoomButton.GetComponent<Button>().interactable = true;
		
		// unlock dock when choice is made
		foreach (Draggable drag in allShips)
		{
			drag.enabled = true;
		}

		ocean.GetComponent<Ships>().stickyShip = true;
		controlPanel.SetActive (true);
		DockButton.GetComponent<CanvasGroup> ().alpha = 1;
		DockButton.GetComponent<Button> ().interactable = true;
		ocean.GetComponent<Ships> ().pointerOverPanel = false;
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().interactable = false;
	}

	public void OnMove()
	{
		GameObject currentShip = ocean.GetComponent<Ships> ().totalShips [ocean.GetComponent<Ships> ().shipCount - 1];
		Vector3 extents = currentShip.GetComponent<MeshRenderer> ().bounds.extents;

		ocean.GetComponent<FogOfWar> ().ShipSight (currentShip.transform.position, extents);


	}

	void HideDock()
	{
		ZoomButton.transform.parent = GameObject.Find ("Canvas").transform;

		controlPanel.SetActive (false);
	}
}
