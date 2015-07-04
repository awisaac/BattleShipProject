using UnityEngine;
using System.Collections;

// This class handles the move option during game play
public class MoveShip : MonoBehaviour {

	public bool isMoving;
	public bool moveDone;
	public Vector3 initPos;
	LineRenderer line;
	float hitDistance;
	float moveDistance;
	float minDistance;
	public Material black;
	Vector3 shipExtents;
	GameObject OptionsPanel;

	// Use this for initialization
	void Start () 
	{
		isMoving = false;
		line = gameObject.AddComponent<LineRenderer> ();
		line.material = black;
		moveDone = true;
		OptionsPanel = GameObject.Find ("OptionsPanel");
	}
	
	// Update is called once per frame
	void Update () 
	{
		// renders line to new ship position
		if (isMoving)
		{
			bool isValid;
			RaycastHit hitInfo = new RaycastHit ();
			Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
			hitInfo.point = new Vector3(hitInfo.point.x, hitInfo.point.y, 0);

			if (GetComponent<Ships>().WithinBounds(hitInfo.point))
			{
				GetComponent<Turns>().currentShip.transform.position = hitInfo.point;
				line.enabled = true;

				if (ValidMove (initPos, hitInfo.point))
				{
					line.SetPosition(0, initPos);
					line.SetPosition(1, hitInfo.point);
					isValid = true;
				
				}
				else
				{
					float distance = (hitInfo.point - initPos).magnitude;
					Vector3 normalizedDirection = (hitInfo.point - initPos) / distance;
					Vector3 endpoint = initPos + normalizedDirection * minDistance;

					line.SetPosition(0,initPos);
					line.SetPosition(1, endpoint);
					isValid = false;
				}
			}

			// Hide ship when cursor goes out of bounds
			else 
			{
				GetComponent<Turns>().currentShip.transform.position = new Vector3(-100, -100, 0);
				line.enabled = false;
			}			

			if (Input.GetMouseButton(0) && ValidMove (initPos, hitInfo.point) && !GetComponent<Ships>().pointerOverZoom && !GetComponent<Ships>().pointerOverPanel) // and have to add doesn't intersect islands or other ships
			{
				isMoving = false;
				GetComponent<Turns>().currentShip.transform.position = GetComponent<Ships>().SnapToGrid(GetComponent<Turns>().currentShip, hitInfo.point);

				// hide move button, make cancel button return ship to original location and confirm zoom out
				OptionsPanel.GetComponent<Options>().MenuShow(7,true);

				// center camera on ship if already zoomed in, else let zoomclick do it

				if (GetComponent<ZoomClick>().ZoomedIn())
				{
					GetComponent<ZoomClick>().CenterCamera(GetComponent<Turns>().currentShip.transform.position);
				}
			}

			if (Input.GetMouseButton (1))
			{
				CancelMove();
			}
		}	
	}

	public void CancelMove()
	{		
		GetComponent<Turns>().currentShip.transform.position = initPos;
		line.enabled = false;
		
		if (!GetComponent<ZoomClick>().ZoomedOut())
		{
			GetComponent<ZoomClick>().ZoomOutClick();
		}
		
		isMoving = false;
		moveDone = true;
		GetComponent<FogOfWar>().ShipSight(GetComponent<Turns>().currentShip);
		OptionsPanel.GetComponent<Options>().MenuShow (0,false);
	}

	public void OnMoveClick() 
	{
		isMoving = true;
		moveDone = false;
		initPos = GetComponent<Turns> ().currentShip.transform.position;
		shipExtents = GetComponent<Turns> ().currentShip.GetComponent<MeshRenderer>().bounds.extents;

		OptionsPanel.GetComponent<Options>().MenuShow(320,false);
		GetComponent<FogOfWar> ().ShipLeave(GetComponent<Turns> ().currentShip);
	}

	// This method will determine if a ray drawn from the center of the ship to the cursor does not intersect any islands
	// and determine that a ship is not being moved onto a island or another ship
	public bool ValidMove(Vector3 startPos, Vector3 endPos)
	{
		Vector3 direction = endPos - startPos;
		moveDistance = Vector3.Distance (startPos, endPos);
		GameObject currentShip = GetComponent<Turns> ().currentShip;

		Ray moveLine = new Ray (startPos, direction);
		bool valid = true;
		minDistance = moveDistance;

		foreach (BoxCollider2D box in GameObject.Find ("Islands").GetComponentsInChildren<BoxCollider2D>())
		{
			// if an island is hit, check distance to compare to distance between start and end
			// if distance to hit is greater than distance between start and end, it is a valid move
			if (box.GetComponent<BoxCollider2D>().bounds.IntersectRay(moveLine, out hitDistance))
			{
				if (hitDistance < minDistance)
				{
					valid = false;
					minDistance = hitDistance;
				}
			}

			if (box.GetComponent<BoxCollider2D>().bounds.Intersects(currentShip.GetComponent<MeshRenderer>().bounds))
			{
				valid = false;
			}
		}

		foreach (MeshRenderer m in GameObject.Find("Ships").GetComponentsInChildren<MeshRenderer>())
		{
			if (m != GetComponent<Turns> ().currentShip.GetComponent<MeshRenderer>() && m.bounds.Intersects(GetComponent<Turns> ().currentShip.GetComponent<MeshRenderer>().bounds))
			{
				return false;
			}
		}

		return valid;
	}

	public void CompleteMove()
	{
		moveDone = true;
		line.enabled = false;
		GetComponent<Turns>().shipSelected = false;
		GetComponent<ZoomClick> ().ZoomOutClick ();
		GetComponent<FogOfWar> ().ShipSight (GetComponent<Turns> ().currentShip);
	}
}
