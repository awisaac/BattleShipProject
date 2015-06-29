using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZoomClick : MonoBehaviour {

	float XCoord;
	float YCoord;
	GameObject Origin;
	GameObject DockButton;
	public GameObject ZoomButton;	
	GameObject OptionsCanvas;

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
	public bool ZoomedOut;

	// Use this for initialization
	void Start () {
		ZoomedOut = true;
		Origin = GameObject.Find ("Origin");
		ZoomButton = GameObject.Find ("ZoomButton");
		DockButton = GameObject.Find ("DockButton");
		OptionsCanvas = GameObject.Find ("OptionsCanvas");

		ZoomButton.SetActive (false);

		ToTarget = Camera.main.transform.position;
		FromPosition = Camera.main.transform.position;
		CurrentLerpTime = 0;
		TotalLerpTime = 0.5f; // 1/2 second to transition camera

		StartOrtho = 500;
		EndOrtho = 500;
		perc = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {

		// Initial Zoom In where user clicks
		if (Input.GetMouseButtonDown (0) && ZoomedOut && !gameObject.GetComponent<Ships>().pointerOverPanel) {
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo) && hitInfo.transform == transform) {

				StartLerpIn(hitInfo);

				ZoomedOut = false;
				ZoomButton.SetActive(true);
				OptionsCanvas.GetComponent<CanvasGroup>().alpha = 0;
				OptionsCanvas.GetComponent<CanvasGroup>().interactable = false;
			}
		}

		if (Input.GetMouseButtonDown(0) && !ZoomedOut && OptionsCanvas.GetComponent<CanvasGroup>().alpha == 0)
		{
			lastPosition = Input.mousePosition;
		}
		
		if (Input.GetMouseButton(0) && !ZoomedOut && OptionsCanvas.GetComponent<CanvasGroup>().alpha == 0)
		{
			Vector3 delta = Input.mousePosition - lastPosition;
			Camera.main.transform.Translate(delta.x * (1 / Time.deltaTime) * mouseSensitivity, delta.y * (1 / Time.deltaTime) * mouseSensitivity, 0);

			lastPosition = Input.mousePosition;
		}

		if (Input.GetKey(KeyCode.LeftArrow) && !ZoomedOut && OptionsCanvas.GetComponent<CanvasGroup>().alpha == 0)
		{
			Camera.main.transform.Translate(-2, 0, 0);
		}

		if (Input.GetKey(KeyCode.RightArrow) && !ZoomedOut && OptionsCanvas.GetComponent<CanvasGroup>().alpha == 0)
		{
			Camera.main.transform.Translate(2, 0, 0);
		}
		if (Input.GetKey(KeyCode.UpArrow) && !ZoomedOut && OptionsCanvas.GetComponent<CanvasGroup>().alpha == 0)
		    {
			Camera.main.transform.Translate(0, 2, 0);
		}
		if (Input.GetKey(KeyCode.DownArrow) && !ZoomedOut && OptionsCanvas.GetComponent<CanvasGroup>().alpha == 0)
		{
			Camera.main.transform.Translate(0, -2, 0);
		}

		//increment timer once per frame
		CurrentLerpTime += Time.deltaTime;
		if (CurrentLerpTime > TotalLerpTime) {
			CurrentLerpTime = TotalLerpTime;
		}

		//lerp!
		if (perc < 1.0f) {
			perc = CurrentLerpTime / TotalLerpTime;
			Camera.main.transform.position = Vector3.Lerp (FromPosition, ToTarget, perc);
			Camera.main.orthographicSize = Mathf.Lerp (StartOrtho, EndOrtho, perc);
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
		
		if (perc == 1.0f && !ZoomedOut) 
		{
			Origin.GetComponent<Gridlines> ().SubGrid (true);
		}
	}
	
	public void ZoomOutClick()
	{
		DockButton.GetComponent<CanvasGroup> ().alpha = 0;
		DockButton.GetComponent<Button> ().interactable = false;

		Origin.GetComponent<Gridlines>().SubGrid(false);
		ZoomButton.SetActive(false);
		StartLerpOut ();
		ZoomedOut = true;
	}

	public void StartLerpIn(RaycastHit hit)
	{
		StartOrtho = 500.0f;
		EndOrtho = 50.0f;
		ToTarget = new Vector3 (hit.point.x, hit.point.y, -10);

		FromPosition = Camera.main.transform.position;		

		CurrentLerpTime = 0;
		perc = 0;
	}

	void StartLerpOut()
	{
		StartOrtho = 50.0f;
		EndOrtho = 500.0f;
		ToTarget = new Vector3 (500,500, -10);

		FromPosition = Camera.main.transform.position;		

		CurrentLerpTime = 0;
		perc = 0;
	}

}