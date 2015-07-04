using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This class handles the initial placement of ships at the beginning of the game
// It also provides additional method handling for snap to grid, ship rotation, and valid ship placement
public class Ships : MonoBehaviour {

	GameObject smallShip;
	GameObject ocean;
	Draggable[] allShips;

	public GameObject currentShip;
	GameObject OptionsPanel;
	public int shipCount;
	public bool stickyShip;
	public bool pointerOverPanel;
	public bool pointerOverZoom;

	// Use this for initialization
	void Start () 
	{
		shipCount = 0;
		stickyShip = false;
		pointerOverPanel = false;
		pointerOverZoom = false;
		ocean = GameObject.Find ("Ocean");
		OptionsPanel = GameObject.Find ("OptionsPanel");
		allShips = GameObject.Find ("ShipPanel").GetComponentsInChildren<Draggable> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);

		// Follow cursor until user does second click
		if (stickyShip && !Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point))
		{
			currentShip.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, 0);		
		}

		// Hide ship when cursor goes out of bounds
		else if (stickyShip && !Input.GetMouseButtonDown(0) && !WithinBounds(hitInfo.point))
		{
			currentShip.transform.position = new Vector3(-100, -100, 0);
		}

		// A valid place to set down the ship
		else if (stickyShip && Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point) && !pointerOverPanel && !pointerOverZoom && !IntersectsIsland(currentShip) && !ShipIntersects(currentShip))
		{
			currentShip.transform.position = SnapToGrid(currentShip, hitInfo.point);
			stickyShip = false;

			OptionsPanel.GetComponent<Options>().MenuShow(135,true); // dockpanel, confirm, rotate, cancel

			GetComponent<ZoomClick>().CenterCamera(currentShip.transform.position);

			// lock dock until choice is made
			foreach (Draggable drag in allShips)
			{
				drag.enabled = false;
			}
		}

		// Return the ship to the dock
		else if (stickyShip && Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point) && pointerOverPanel)
		{
			Destroy(currentShip);
			stickyShip = false;
			smallShip.SetActive(true);
			smallShip.transform.SetParent(GameObject.Find ("ShipPanel").transform);

			if (!gameObject.GetComponent<ZoomClick>().ZoomedOut()) 
			{
				gameObject.GetComponent<ZoomClick>().ZoomOutClick();
			}

			OptionsPanel.GetComponent<Options>().MenuShow(128, false);

			// unlock dock when choice is made
			foreach (Draggable drag in allShips)
			{
				drag.enabled = true;
			}
		}

		else if (stickyShip && Input.GetMouseButtonDown(0) && WithinBounds(hitInfo.point) && pointerOverZoom)
		{
			pointerOverZoom = false;
		}

		// Invalid place to set down ship - hide ship until cursor returns to bounds
		else if (stickyShip && Input.GetMouseButtonDown(0) && !WithinBounds(hitInfo.point))
		{
			currentShip.transform.position = new Vector3(-100, -100, 0);
		}

		// Right click returns current ship to panel
		if (stickyShip && Input.GetMouseButtonDown(1))
		{
			Destroy(currentShip);
			stickyShip = false;
			smallShip.SetActive(true);
			smallShip.transform.SetParent(GameObject.Find ("ShipPanel").transform);

			if (!gameObject.GetComponent<ZoomClick>().ZoomedOut()) 
			{
				gameObject.GetComponent<ZoomClick>().ZoomOutClick();
			}

			OptionsPanel.GetComponent<Options>().MenuShow(128,false);
		}
	}

	public void PlaceShip(GameObject selectedShip, GameObject smallShip)
	{
		RaycastHit hitInfo = new RaycastHit ();
		OptionsPanel.GetComponent<Options>().MenuShow(448, false); // zoom, control panel, and dock button only

		// keep this small ship in case it is needed again
		this.smallShip = smallShip;

		// Place ship where drag ended
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) 
		{
			currentShip = Instantiate(selectedShip, new Vector3(hitInfo.point.x, hitInfo.point.y, 0), Quaternion.identity) as GameObject;
			currentShip.transform.SetParent(GameObject.Find ("Ships").transform);
			currentShip.tag = "ship";
			stickyShip = true;

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

	public Vector3 SnapToGrid(GameObject ship, Vector3 dropPoint)
	{
		// get most significant digits
		float x = ((int)(dropPoint.x / 10)) * 10;
		float y = ((int)(dropPoint.y / 10)) * 10;
		float width;
		float height;

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
		if ((int)(ship.transform.eulerAngles.z) % 180 == 90)
		{
			width = ship.transform.localScale.y;
			height = ship.transform.localScale.x;

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
			height = ship.transform.localScale.y;
			width = ship.transform.localScale.x;

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

	public void ZoomButtonEnter()
	{
		pointerOverZoom = true;
	}

	public void ZoomButtonExit()
	{
		pointerOverZoom = false;
	}

	// Undo move function - same as left click
	public void DockShip()
	{
		if (shipCount < 5)
		{
			Destroy(currentShip);
			stickyShip = false;
			smallShip.SetActive(true);
			smallShip.transform.SetParent(GameObject.Find ("ShipPanel").transform);

			OptionsPanel.GetComponent<Options>().MenuShow(128,false);
		}

		else
		{
			GetComponent<MoveShip>().CancelMove();
		}
	}

	public void Rotate()
	{

		GetComponent<FogOfWar>().ShipLeave(currentShip);

		currentShip.transform.Rotate (new Vector3 (0, 0, 90));
		currentShip.transform.position = SnapToGrid(currentShip, currentShip.transform.position);

		// If rotate doesnt fit, put it back
		if (ShipIntersects(currentShip) || IntersectsIsland(currentShip))
		{
			currentShip.transform.Rotate (new Vector3 (0, 0, -90));
			currentShip.transform.position = SnapToGrid(currentShip, currentShip.transform.position);
		}

		// Requires confirmation to get initial ship site
		if (shipCount >= 5)
		{
			GetComponent<FogOfWar>().ShipSight(currentShip);
		}
	}

	public void RemoveShipCollision ()
	{
		bool right = true;
		bool up = true;

		// vertical ship
		if ((int)(currentShip.transform.eulerAngles.z) % 180 == 90)
		{				
			while ((ShipIntersects(currentShip) || IntersectsIsland(currentShip)) && right)
			{
				if (currentShip.transform.position.x < 990)
				{
					currentShip.transform.position += new Vector3(10,0,0);
				}
				else 
				{
					right = false;
				}
			}

			while ((ShipIntersects(currentShip) || IntersectsIsland(currentShip)) && !right)
			{
				currentShip.transform.position += new Vector3(-10,0,0);
			}
		}

		//horizontal ship
		else
		{
			while ((ShipIntersects(currentShip) || IntersectsIsland(currentShip)) && up)
			{
				if (currentShip.transform.position.y < 990)
				{
					currentShip.transform.position += new Vector3(0,10,0);
				}
				else 
				{
					up = false;
				}
			}
			
			while ((ShipIntersects(currentShip) || IntersectsIsland(currentShip)) && !up)
			{
				currentShip.transform.position += new Vector3(0,-10,0);
			}
		}
	}

	public bool ShipIntersects(GameObject ship)
	{
		foreach (MeshRenderer m in GameObject.Find ("Ships").GetComponentsInChildren<MeshRenderer>()) 
		{
			if (m != ship.GetComponent<MeshRenderer>() && ship.GetComponent<MeshRenderer>().bounds.Intersects(m.bounds))
			{
				return true;
			}
		}
		return false;
	}

	bool IntersectsIsland(GameObject ship)
	{

		int counter = 0;

		foreach (BoxCollider2D box in GameObject.Find ("Islands").GetComponentsInChildren<BoxCollider2D>())
		{
			if (ship.GetComponent<MeshRenderer>().bounds.Intersects(box.bounds))
			{
				return true;
			}
		}

		return false;			
	}
}

