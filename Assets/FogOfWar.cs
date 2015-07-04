using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Creates both ocean and islands initially and updates fog of war for each move
public class FogOfWar : MonoBehaviour {

	Texture2D terrainTexture;
	bool[,] IslandBitmap;
	GameObject IslandGroup;
	Color32[,] IslandColors;
	public GameObject islandPrefab;

	int numberOfIslands = 100;
	int sizeOfIslands = 6;

	// Use this for initialization
	void Start () 
	{
		terrainTexture = new Texture2D(100, 100); // each land square is 10x10 in world space
		GetComponent<Renderer>().material.mainTexture = terrainTexture;

		IslandGroup = GameObject.Find ("Islands");
		IslandBitmap = new bool[100,100];
		IslandColors = new Color32[100,100];

		// Create ocean and initial fog
		for (int i = 0; i < 100; i++)
		{
			for (int j = 0; j < 100; j++)
			{
				terrainTexture.SetPixel(i,j, new Color32(0,128,255,255));
			}
		}

		GenerateIslands ();
		terrainTexture.Apply();
	}

	// Islands are randomly chosen pixels
	void GenerateIslands()
	{
		for (int i = 0; i < numberOfIslands; i++)
		{
			int xPos = (int)(Random.value * (101 - sizeOfIslands)); // left bottom corner
			int yPos = (int)(Random.value * (101 - sizeOfIslands));  
			int size = (int)(Random.value * sizeOfIslands) + 1;

			for (int j = 0; j < size; j++)
			{
				for (int k = 0; k < size; k++)
				{
					// decide if land or water is in this spot of the island
					int randShape = (int)(Random.value * 2);
					if (randShape == 1)
					{
						// all colors vary by +/- 32
						byte CV1 = (byte)(44 + (int)(Random.value * 64) - 32);
						byte CV2 = (byte)(176 + (int)(Random.value * 64) - 32);
						byte CV3 = (byte)(55 + (int)(Random.value * 64) - 32);

						terrainTexture.SetPixel(xPos + j, yPos + k, new Color32(CV1, CV2, CV3, 255));
						IslandBitmap[xPos + j, yPos + k] = true;
						IslandColors[xPos + j, yPos + k] = new Color32(CV1, CV2, CV3, 255);
						GameObject tempIsland = Instantiate(islandPrefab, new Vector3((xPos + j) * 10.0f + 5, (yPos + k) * 10.0f + 5, 0), Quaternion.identity) as GameObject;
						tempIsland.transform.SetParent(IslandGroup.transform);
					}
				}
			}
		}
	}

	public void ShipSight(GameObject ship)
	{
		Vector3 position = ship.transform.position;
		Vector3 extent = ship.GetComponent<MeshRenderer>().bounds.extents;

		int startX = Mathf.Max(0, (int)(position.x - extent.x) / 10 - 5);
		int endX = Mathf.Min (99, (int)(position.x + extent.x) / 10 + 5);
		int startY = Mathf.Max (0, (int)(position.y - extent.y) / 10 - 5);
		int endY = Mathf.Min (99, (int)(position.y + extent.y) / 10 + 5);
		
		//Uncover new position yet maintain islands
		for (int i = startX; i <= endX; i++) 
		{
			for (int j = startY; j <= endY; j++)
			{
				if (IslandBitmap[i,j]) 
				{
					terrainTexture.SetPixel(i,j,IslandColors[i,j]);
				}

				else
				{
					terrainTexture.SetPixel(i,j,new Color32(0,192,255,255));
				}
			}
		}
		
		terrainTexture.Apply();
	}

	public void ShipLeave(GameObject ship)
	{
		Vector3 position = ship.transform.position;
		Vector3 extent = ship.GetComponent<MeshRenderer>().bounds.extents;

		int startX = Mathf.Max(0, (int)(position.x - extent.x) / 10 - 5);
		int endX = Mathf.Min (99, (int)(position.x + extent.x) / 10 + 5);
		int startY = Mathf.Max (0, (int)(position.y - extent.y) / 10 - 5);
		int endY = Mathf.Min (99, (int)(position.y + extent.y) / 10 + 5);

		//Cover old position yet maintain islands
		for (int i = startX; i <= endX; i++) 
		{
			for (int j = startY; j <= endY; j++)
			{
				if (IslandBitmap[i,j]) 
				{
					terrainTexture.SetPixel(i,j,IslandColors[i,j]);
				}

				else
				{
					terrainTexture.SetPixel(i,j,new Color32(0,128,255,255));
				}

			}
		}

		terrainTexture.Apply();

		// redraw the sight of all other ships in case their sight was overwritten	
		foreach (MeshRenderer m in GameObject.Find ("Ships").GetComponentsInChildren<MeshRenderer>())
		{
			if (m.gameObject != ship)
			{
				ShipSight(m.gameObject);
			}
		}
	}
}