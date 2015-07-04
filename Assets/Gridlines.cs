using UnityEngine;
using System.Collections;

// This class creates the grid and draws the subgrid when camera is zoomed in
public class Gridlines : MonoBehaviour {

	GameObject[] grids;
	GameObject Grid;
	LineRenderer[] vertLines;
	LineRenderer[] horiLines;
	public Material black;

	// Use this for initialization
	void Start () {

		grids = new GameObject[198]; 
		Grid = GameObject.Find ("Grid");
		vertLines = new LineRenderer[99];
		horiLines = new LineRenderer[99];

		for (int i = 0; i < 99; i++) {
			grids[i] = new GameObject();
			grids[i].transform.SetParent(Grid.transform);
			vertLines [i] = grids[i].AddComponent<LineRenderer> ();
			vertLines [i].SetVertexCount(2);
			vertLines [i].SetPosition (0, new Vector3 (0, i * 10 + 10, 0));
			vertLines [i].SetPosition (1, new Vector3 (1000, i * 10 + 10, 0));
			vertLines[i].material = black;

			if ((i + 1) % 10 == 0){
				vertLines[i].SetWidth(1f, 1f);
			}
			else
			{
				vertLines[i].SetWidth(0.1f, 0.1f);
			}

			grids[i + 99] = new GameObject();
			grids[i + 99].transform.SetParent(Grid.transform);
			horiLines [i] = grids[i + 99].AddComponent<LineRenderer> ();
			horiLines [i].SetVertexCount(2);
			horiLines [i].SetPosition (0, new Vector3 (i * 10 + 10, 0, 0));
			horiLines [i].SetPosition (1, new Vector3 (i * 10 + 10, 1000, 0));
			horiLines[i].material = black;
			horiLines[i].SetWidth(0.1f, 0.1f);

			if ((i + 1) % 10 == 0){
				horiLines[i].SetWidth(1f, 1f);
			}
			else
			{
				horiLines[i].SetWidth(0.1f, 0.1f);
			}
		}

		SubGrid (false);
	}	

	public void SubGrid(bool showSubGrid)
	{
		for (int i = 0; i < 99; i++) {
				
			if ((i + 1) % 10 != 0){		
				vertLines[i].enabled = showSubGrid;
			}

			if ((i + 1) % 10 != 0){
				horiLines[i].enabled = showSubGrid;
			}
		}
	}
}
