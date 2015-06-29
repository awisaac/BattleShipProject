using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public GameObject carrierPrefab;
	public GameObject battleshipPrefab;
	public GameObject destroyerPrefab;
	public GameObject seekerPrefab;

	GameObject selectedShip;
	GameObject ocean;
	GameObject shipPanel;
	GameObject[] shipDropped;

	GameObject smallCarrier;
	GameObject smallBattleship;
	GameObject smallDestroyer;
	GameObject smallSeeker1;
	GameObject smallSeeker2;

	void Start()
	{
		smallCarrier = GameObject.Find ("Carrier");
		smallBattleship = GameObject.Find ("Battleship");
		smallDestroyer = GameObject.Find ("Destroyer");
		smallSeeker1 = GameObject.Find ("Seeker1");
		smallSeeker2 = GameObject.Find ("Seeker2");

		ocean = GameObject.Find ("Ocean");
		shipPanel = GameObject.Find ("ShipPanel");
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		transform.SetParent(GameObject.Find ("CanvasPanel").transform);
		GetSelectedShip ();

		if (!GameObject.Find ("Ocean").GetComponent<ZoomClick> ().ZoomedOut)
		{
			GameObject.Find ("Ocean").GetComponent<ZoomClick> ().ZoomOutClick ();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		ZoomClick zoom = ocean.GetComponent<ZoomClick> ();
		RaycastHit hitInfo = new RaycastHit ();

		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo) && hitInfo.transform == ocean.transform && zoom.ZoomedOut) {
			
			zoom.StartLerpIn (hitInfo);			
			zoom.ZoomedOut = false;
			zoom.ZoomButton.SetActive (true);
			gameObject.SetActive (false);

			// place ship in initial drop position
			ocean.GetComponent<Ships>().PlaceShip(selectedShip, gameObject);
		}

		else if (!zoom.ZoomedOut && ocean.GetComponent<Ships>().WithinBounds(hitInfo.point))
		{
			gameObject.SetActive(false);

			// place ship in initial drop position
			ocean.GetComponent<Ships>().PlaceShip(selectedShip, gameObject);		
		}

		else {
			transform.SetParent(GameObject.Find ("ShipPanel").transform);
		}

		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	void GetSelectedShip()
	{
		if (gameObject == smallCarrier) 
		{
			selectedShip = carrierPrefab;
		} 
		else if (gameObject == smallBattleship) 
		{
			selectedShip = battleshipPrefab;
		}
		else if (gameObject == smallDestroyer) 
		{
			selectedShip = destroyerPrefab;
		}
		else if (gameObject == smallSeeker1 || gameObject == smallSeeker2) 
		{
			selectedShip = seekerPrefab;
		}
	}
}
