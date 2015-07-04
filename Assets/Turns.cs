using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This class handles the users choices after ships are placed
public class Turns : MonoBehaviour {

	public bool turnStarted;
	public bool shipSelected;
	public GameObject currentShip;
	GameObject OptionsPanel;
	GameObject[] ships;

	// Use this for initialization
	void Start () {

		turnStarted = false;
		shipSelected = false;
		OptionsPanel = GameObject.Find ("OptionsPanel");
	}
	
	// Update is called once per frame
	void Update () {

		// make any ship selectable, have it do its move, and make it no longer selectable until next turn
		if (turnStarted) 
		{
			RaycastHit hitInfo = new RaycastHit ();

			if (Input.GetMouseButtonUp(0))
			{

				Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
				hitInfo.point = new Vector3(hitInfo.point.x, hitInfo.point.y, 0);

				foreach (GameObject ship in ships)
				{
					if (ship.GetComponent<MeshRenderer>().bounds.Contains(hitInfo.point) && GetComponent<MoveShip>().moveDone)
					{
						currentShip = ship;
						shipSelected = true;

						OptionsPanel.GetComponent<Options>().MenuShow(62, true);

						GetComponent<Ships>().currentShip = currentShip; // makes rotate work

						if (GetComponent<ZoomClick>().ZoomedIn())
						{
							GetComponent<ZoomClick>().CenterCamera(currentShip.transform.position);
						}
					}
				}
			}

		}
	
	}

	public void StartTurn()
	{
		turnStarted = true;
		ships = GameObject.FindGameObjectsWithTag ("ship");
	}
}
