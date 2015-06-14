using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public void OnBeginDrag(PointerEventData eventData)
	{
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		transform.SetParent(GameObject.Find ("CanvasPanel").transform);
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		ZoomClick zoom = GameObject.Find ("Ocean").GetComponent<ZoomClick> ();
		RaycastHit hitInfo = new RaycastHit ();

		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo) && hitInfo.transform == GameObject.Find ("Ocean").transform && zoom.ZoomedOut) {
			
			zoom.StartLerpIn (hitInfo);			
			zoom.ZoomedOut = false;
			zoom.ZoomButton.SetActive (true);
			gameObject.SetActive (false);
		} 

		else if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo) && hitInfo.transform == GameObject.Find ("Ocean").transform && !zoom.ZoomedOut) {
		
			gameObject.SetActive (false);
		}

		else {
			transform.SetParent(GameObject.Find ("ShipPanel").transform);
		}

		GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}
}
