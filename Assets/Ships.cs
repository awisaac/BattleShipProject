﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ships : MonoBehaviour {

	public GameObject[] totalShips;
	GameObject smallShip;
	GameObject optionsCanvas;
	GameObject ocean;
	GameObject DockButton;
	Draggable[] allShips;
	public int shipCount;
	public bool stickyShip;
	bool pointerOverPanel;

	Vector3 ToTarget;
	Vector3 FromPosition;
	float CurrentLerpTime;
	float TotalLerpTime;
	float perc;

	// Use this for initialization
	void Start () 
	{
		totalShips = new GameObject[5];
		shipCount = 0;
		stickyShip = false;
		pointerOverPanel = false;
		optionsCanvas = GameObject.Find ("OptionsCanvas");
		ocean = GameObject.Find ("Ocean");
		DockButton = GameObject.Find ("DockButton");
		allShips = GameObject.Find ("ShipPanel").GetComponentsInChildren<Draggable> ();

		perc = 1.0f;

		ToTarget = Camera.main.transform.position;
		FromPosition = Camera.main.transform.position;
		CurrentLerpTime = 0;
		TotalLerpTime = 1.0f; // 1/2 second to transition camera

		DockButton.GetComponent<CanvasGroup> ().alpha = 0;
		DockButton.GetComponent<Button> ().interactable = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);

		// Follow cursor until user does second click
		if (stickyShip && !Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point))
		{
			totalShips[shipCount].transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, 0);		
		}

		// Hide ship when cursor goes out of bounds
		else if (stickyShip && !Input.GetMouseButtonDown(0) && !WithinBounds(hitInfo.point))
		{
			totalShips[shipCount].transform.position = new Vector3(-100, -100, 0);
		}

		// A valid place to set down the ship
		else if (stickyShip && Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point) && !pointerOverPanel)
		{
			totalShips[shipCount].transform.position = SnapToGrid(hitInfo.point);
			stickyShip = false;

			optionsCanvas.SetActive(true);
			optionsCanvas.transform.position = totalShips[shipCount].transform.position;
			perc = 0;
			CurrentLerpTime = 0;
			FromPosition = Camera.main.transform.position;
			ToTarget = new Vector3(totalShips[shipCount].transform.position.x, totalShips[shipCount].transform.position.y, -10);

			// freeze camera until choice is made
			ocean.GetComponent<ZoomClick>().enabled = false;
			GameObject.Find("ZoomButton").GetComponent<Button>().interactable = false;

			// lock dock until choice is made
			foreach (Draggable drag in allShips)
			{
				drag.enabled = false;
			}

			DockButton.GetComponent<CanvasGroup> ().alpha = 0;
			DockButton.GetComponent<Button> ().interactable = false;
		}

		// Return the ship to the dock
		else if (stickyShip && Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point) && pointerOverPanel)
		{
			Destroy(totalShips[shipCount]);
			stickyShip = false;
			smallShip.SetActive(true);
			smallShip.transform.SetParent(GameObject.Find ("ShipPanel").transform);
		}

		// Invalid place to set down ship - hide ship until cursor returns to bounds
		else if (stickyShip && Input.GetMouseButtonDown(0) && !WithinBounds(hitInfo.point))
		{
			totalShips[shipCount].transform.position = new Vector3(-100, -100, 0);
		}

		// Right click returns current ship to panel
		if (stickyShip && Input.GetMouseButtonDown(1))
		{
			Destroy(totalShips[shipCount]);
			stickyShip = false;
			smallShip.SetActive(true);
			smallShip.transform.SetParent(GameObject.Find ("ShipPanel").transform);

			DockButton.GetComponent<CanvasGroup> ().alpha = 0;
			DockButton.GetComponent<Button> ().interactable = false;
		}

		//increment timer once per frame
		CurrentLerpTime += Time.deltaTime;
		if (CurrentLerpTime > TotalLerpTime) {
			CurrentLerpTime = TotalLerpTime;
		}
		
		if (perc < 1.0f) 
		{
			perc = CurrentLerpTime / TotalLerpTime;
			Camera.main.transform.position = Vector3.Lerp (FromPosition, ToTarget, perc);
		}
	}

	public void PlaceShip(GameObject selectedShip, GameObject smallShip)
	{
		RaycastHit hitInfo = new RaycastHit ();

		// keep this small ship in case it is needed again
		this.smallShip = smallShip;

		// Place ship where drag ended
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) 
		{
			totalShips[shipCount] = Instantiate(selectedShip, new Vector3(hitInfo.point.x, hitInfo.point.y, 0), Quaternion.identity) as GameObject;
			stickyShip = true;
			DockButton.GetComponent<CanvasGroup> ().alpha = 1;
			DockButton.GetComponent<Button> ().interactable = true;
		}
	}

	public bool WithinBounds(Vector3 point)
	{
		if (point.x > 0 && point.x < 1000 && point.y > 0 && point.y < 1000)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public Vector3 SnapToGrid(Vector3 dropPoint)
	{
		// get most significant digits
		float x = ((int)(dropPoint.x / 10)) * 10;
		float y = ((int)(dropPoint.y / 10)) * 10;

		// round up if needed
		if (dropPoint.x - x >= 5) 
		{
			x += 10;
		}

		if (dropPoint.y - y >= 5) 
		{
			y += 10;
		}

		// check if at edge and move ship over to where it will fit

		// width is height and height is width
		if ((int)(totalShips[shipCount].transform.eulerAngles.z) % 180 == 90)
		{
			float width = totalShips[shipCount].transform.localScale.y;
			float height = totalShips[shipCount].transform.localScale.x;

			if (x + (width / 2) > 1000)
			{
				x = 1000 - (width / 2);
			}
			if (y + (height / 2) > 1000)
			{
				y = 1000 - (height / 2);
			}
			if (x - (width / 2) < 0)
			{
				x = width / 2;
			}
			if (y - (height / 2) < 0)
			{
				y = height / 2;
			}
		}
		else
		{
			float height = totalShips[shipCount].transform.localScale.y;
			float width = totalShips[shipCount].transform.localScale.x;

			if (x + (width / 2) > 1000)
			{
				x = 1000 - (width / 2);
			}
			if (y + (height / 2) > 1000)
			{
				y = 1000 - (height / 2);
			}
			if (x - (width / 2) < 0)
			{
				x = width / 2;
			}
			if (y - (height / 2) < 0)
			{
				y = height / 2;
			}
		}

		return new Vector3(x, y, 0);
	}

	public void PanelPointerEnter()
	{
		pointerOverPanel = true;
	}
	public void PanelPointerExit()
	{
		pointerOverPanel = false;
	}

	public void DockShip()
	{
		Destroy(totalShips[shipCount]);
		stickyShip = false;
		smallShip.SetActive(true);
		smallShip.transform.SetParent(GameObject.Find ("ShipPanel").transform);

		DockButton.GetComponent<CanvasGroup> ().alpha = 0;
		DockButton.GetComponent<Button> ().interactable = false;
	}

	public void Rotate()
	{
		totalShips [shipCount].transform.Rotate (new Vector3 (0, 0, 90));
		totalShips[shipCount].transform.position = SnapToGrid(totalShips[shipCount].transform.position);
		optionsCanvas.transform.position = totalShips [shipCount].transform.position;
		FromPosition = Camera.main.transform.position;
		ToTarget = new Vector3(totalShips [shipCount].transform.position.x, totalShips [shipCount].transform.position.y, -10);
		
		perc = 0;
		CurrentLerpTime = 0;
	}
}
