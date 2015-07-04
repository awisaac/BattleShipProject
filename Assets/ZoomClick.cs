using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This class handles all camera movement
public class ZoomClick : MonoBehaviour {

	float XCoord;
	float YCoord;
	GameObject Grid;
	GameObject OptionsPanel;
	GameObject ZoomButton;

	// Lerp Values for Camera to zoom in
	Vector3 ToTarget;
	Vector3 FromPosition;
	float CurrentLerpTime;
	float TotalLerpTime;
	float StartOrtho;
	float EndOrtho;
	float perc;

	float mouseSensitivity = 0.02f;
	Vector3 lastPosition;
	public bool menuLocked;

	// Use this for initialization
	void Start () {
		Grid = GameObject.Find ("Grid");
		OptionsPanel = GameObject.Find ("OptionsPanel");
		ZoomButton = GameObject.Find("ZoomButton");
		ToTarget = Camera.main.transform.position;
		FromPosition = Camera.main.transform.position;
		CurrentLerpTime = 0;
		TotalLerpTime = 0.5f; // 1/2 second to transition camera

		StartOrtho = 500;
		EndOrtho = 50;
		perc = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {

		// Initial Zoom In where user clicks
		if (Input.GetMouseButtonDown (0) && ZoomedOut() && !gameObject.GetComponent<Ships>().pointerOverPanel) {
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo) && hitInfo.transform == transform) {

				CenterCamera(hitInfo.point);

				ZoomButton.GetComponent<CanvasGroup>().alpha = 1;
				ZoomButton.GetComponent<CanvasGroup>().interactable = true;

			}
		}

		if (Input.GetMouseButtonDown(0) && !ZoomedOut() && !menuLocked)
		{
			lastPosition = Input.mousePosition;
		}
		
		if (Input.GetMouseButton(0) && !ZoomedOut() && !menuLocked)
		{
			Vector3 delta = Input.mousePosition - lastPosition;
			Camera.main.transform.Translate(delta.x * (1 / Time.deltaTime) * mouseSensitivity, delta.y * (1 / Time.deltaTime) * mouseSensitivity, 0);

			lastPosition = Input.mousePosition;
		}

		if (Input.GetKey(KeyCode.LeftArrow) && !ZoomedOut() && !menuLocked)
		{
			Camera.main.transform.Translate(-2, 0, 0);
		}

		if (Input.GetKey(KeyCode.RightArrow) && !ZoomedOut() && !menuLocked)
		{
			Camera.main.transform.Translate(2, 0, 0);
		}
		if (Input.GetKey(KeyCode.UpArrow) && !ZoomedOut() && !menuLocked)
		    {
			Camera.main.transform.Translate(0, 2, 0);
		}
		if (Input.GetKey(KeyCode.DownArrow) && !ZoomedOut() && !menuLocked)
		{
			Camera.main.transform.Translate(0, -2, 0);
		}

		//lerp!
		if (perc < 1.0f) {
			CurrentLerpTime += Time.deltaTime;
			perc = CurrentLerpTime / TotalLerpTime;
			Camera.main.transform.position = Vector3.Lerp (FromPosition, ToTarget, perc);
			Camera.main.orthographicSize = Mathf.Lerp (StartOrtho, EndOrtho, perc);

			if (ZoomedIn()) 
			{
				Grid.GetComponent<Gridlines> ().SubGrid (true);
			}
		}


		if (Camera.main.transform.position.x < 25) {
			Camera.main.transform.position = new Vector3 (25, Camera.main.transform.position.y, -10);
		}

		if (Camera.main.transform.position.x > 975) {
			Camera.main.transform.position = new Vector3 (975, Camera.main.transform.position.y, -10);
		}

		if (Camera.main.transform.position.y < 25) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, 25, -10);
		}

		if (Camera.main.transform.position.y > 975) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, 975, -10);
		}
	}

	public bool ZoomedOut()
	{
		return Camera.main.orthographicSize == 500;
	}

	public bool ZoomedIn()
	{
		return Camera.main.orthographicSize == 50;
	}

	public void ZoomOutClick()
	{
		ZoomButton.GetComponent<CanvasGroup>().alpha = 0;
		ZoomButton.GetComponent<CanvasGroup>().interactable = false;

		Grid.GetComponent<Gridlines>().SubGrid(false);
		GetComponent<Ships> ().pointerOverZoom = false;

		StartOrtho = 50.0f;
		EndOrtho = 500.0f;
		ToTarget = new Vector3 (500,500, -10);
		
		FromPosition = Camera.main.transform.position;		
		
		CurrentLerpTime = 0;
		perc = 0;
	}

	public void CenterCamera(Vector3 position)
	{
		if(Camera.main.orthographicSize == 500.0f) {
			StartOrtho = 500.0f;
			EndOrtho = 50.0f;
		}

		else
		{
			StartOrtho = 50.0f;
			EndOrtho = 50.0f;
		}

		ToTarget = new Vector3 (position.x, position.y, -10);		
		FromPosition = Camera.main.transform.position;				
		CurrentLerpTime = 0;
		perc = 0;
	}
}