using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This class handles which buttons are shown at various stages of the game and handles confirm and cancel actions
public class Options : MonoBehaviour {

	GameObject ocean;
	GameObject shipPanel;
	Draggable[] allShips;
	GameObject DockButton;
	GameObject ZoomButton;
	GameObject ConfirmButton;
	GameObject CancelButton;
	GameObject ControlPanel;
	GameObject FireButton;
	GameObject ScanButton;
	GameObject MoveButton;
	GameObject RotateButton;

	// Use this for initialization
	void Start () {

		ocean = GameObject.Find("Ocean");
		shipPanel = GameObject.Find ("ShipPanel");
		ZoomButton = GameObject.Find("ZoomButton");
		allShips = shipPanel.GetComponentsInChildren<Draggable> ();	
		DockButton = GameObject.Find ("DockButton");
		ConfirmButton = GameObject.Find ("ConfirmButton");
		CancelButton = GameObject.Find ("CancelButton");
		DockButton = GameObject.Find ("DockButton");
		ControlPanel = GameObject.Find ("ControlPanel");
		FireButton = GameObject.Find ("FireButton");
		ScanButton = GameObject.Find ("ScanButton");
		MoveButton = GameObject.Find ("MoveButton");
		RotateButton = GameObject.Find ("RotateButton");

		MenuShow (128,false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void MenuShow(int options, bool locked)
	{
		// options are 0 through 511 to determine which buttons/panels need to be shown to clean up the craziness
		// 8 - Dock Button, 7 - ControlPanel, 6 - ZoomButton
		// 5 - Fire Button, 4 - Scan Button, 3 - MoveButton
		// 2 - Cancel Button, 1 - Rotate Button, 0 - ConfirmButton

		if (options >= 256)
		{
			DockButton.GetComponent<CanvasGroup>().alpha = 1;
			DockButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 256;
		}

		else
		{
			DockButton.GetComponent<CanvasGroup>().alpha = 0;
			DockButton.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 128)
		{
			ControlPanel.GetComponent<CanvasGroup>().alpha = 1;
			ControlPanel.GetComponent<CanvasGroup>().interactable = true;
			options -= 128;
		}
		
		else
		{
			ControlPanel.GetComponent<CanvasGroup>().alpha = 0;
			ControlPanel.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 64)
		{
			ZoomButton.GetComponent<CanvasGroup>().alpha = 1;
			ZoomButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 64;
		}
		
		else
		{
			ZoomButton.GetComponent<CanvasGroup>().alpha = 0;
			ZoomButton.GetComponent<CanvasGroup>().interactable = false;
		}



		if (options >= 32)
		{
			FireButton.GetComponent<CanvasGroup>().alpha = 1;
			FireButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 32;
		}
		
		else
		{
			FireButton.GetComponent<CanvasGroup>().alpha = 0;
			FireButton.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 16)
		{
			ScanButton.GetComponent<CanvasGroup>().alpha = 1;
			ScanButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 16;
		}
		
		else
		{
			ScanButton.GetComponent<CanvasGroup>().alpha = 0;
			ScanButton.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 8)
		{
			MoveButton.GetComponent<CanvasGroup>().alpha = 1;
			MoveButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 8;
		}
		
		else
		{
			MoveButton.GetComponent<CanvasGroup>().alpha = 0;
			MoveButton.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 4)
		{
			CancelButton.GetComponent<CanvasGroup>().alpha = 1;
			CancelButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 4;
		}
		
		else
		{
			CancelButton.GetComponent<CanvasGroup>().alpha = 0;
			CancelButton.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 2)
		{
			RotateButton.GetComponent<CanvasGroup>().alpha = 1;
			RotateButton.GetComponent<CanvasGroup>().interactable = true;
			options -= 2;
		}
		
		else
		{
			RotateButton.GetComponent<CanvasGroup>().alpha = 0;
			RotateButton.GetComponent<CanvasGroup>().interactable = false;
		}

		if (options >= 1)
		{
			ConfirmButton.GetComponent<CanvasGroup>().alpha = 1;
			ConfirmButton.GetComponent<CanvasGroup>().interactable = true;
		}
		
		else
		{
			ConfirmButton.GetComponent<CanvasGroup>().alpha = 0;
			ConfirmButton.GetComponent<CanvasGroup>().interactable = false;
		}

		ocean.GetComponent<ZoomClick> ().menuLocked = locked;
	}

	public void OnConfirm()
	{
		if (ocean.GetComponent<Ships>().shipCount >= 5)
		{
			ocean.GetComponent<MoveShip>().CompleteMove();
			MenuShow (0, false);
		}

		else
		{
			// unlock dock when choice is made
			foreach (Draggable drag in allShips)
			{
				drag.enabled = true;
			}

			//ocean.GetComponent<Ships> ().RemoveShipCollision ();

			ocean.GetComponent<Ships>().shipCount++;

			// Remove dock when all ships are placed
			if (ocean.GetComponent<Ships>().shipCount >= 5) 
			{
				MenuShow (0,false);
				ocean.GetComponent<Turns>().StartTurn();
			}

			// Bring dock back for remaining ships
			else
			{
				MenuShow (128,false);
			}

			ocean.GetComponent<ZoomClick> ().ZoomOutClick ();
		}

		GameObject currentShip = ocean.GetComponent<Ships> ().currentShip;
		ocean.GetComponent<FogOfWar>().ShipSight(currentShip);
	}

	public void OnCancel()
	{
		// if cancel after select, hide menu and zoom out
		if (ocean.GetComponent<Turns>().shipSelected && ocean.GetComponent<MoveShip>().moveDone)
		{
			ocean.GetComponent<Turns>().shipSelected = false;

			MenuShow (0, false);
			ocean.GetComponent<ZoomClick>().ZoomOutClick();
			
		}

		// continue moving ship
		else if (ocean.GetComponent<Turns>().shipSelected && !ocean.GetComponent<MoveShip>().moveDone)
		{
			ocean.GetComponent<MoveShip>().isMoving = true;

			// Only Zoomout/Reset buttons need to show at this time
			MenuShow (320,false);   
		}
		
		// cancel initial ship placement
		else
		{
			// unlock dock when choice is made
			foreach (Draggable drag in allShips)
			{
				drag.enabled = true;
			}

			ocean.GetComponent<Ships>().stickyShip = true;

			MenuShow (448,false);
			ocean.GetComponent<Ships> ().pointerOverPanel = false;
		}

	}
}
