using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FogOfWar : MonoBehaviour {

	Texture2D texture;
	Island[] islands;

	int numberOfIslands = 100;

	// Use this for initialization
	void Start () 
	{
		texture = new Texture2D(100, 100);
		GetComponent<Renderer>().material.mainTexture = texture;

		// Create ocean
		for (int i = 0; i < 100; i++)
		{
			for (int j = 0; j < 100; j++)
			{
				texture.SetPixel(i,j, new Color32(0,128,255,255));
			}
		}

		GenerateIslands ();
		texture.Apply();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Islands are randomly chosen pixels
	void GenerateIslands()
	{
		islands = new Island[numberOfIslands];

		for (int i = 0; i < numberOfIslands; i++)
		{
			int xPos = (int)(Random.value * 96); // left bottom corner
			int yPos = (int)(Random.value * 96);  
			int size = (int)(Random.value * 5) + 1;
			Color32[,] landColors = new Color32[size,size];
			bool[,] shape = new bool[size,size];

			for (int j = 0; j < size; j++)
			{
				for (int k = 0; k < size; k++)
				{
					// decide if land or water is in this spot of the island
					int randShape = (int)(Random.value * 2);
					if (randShape == 1)
					{
						shape[j,k] = true;
					}

					if (shape[j,k])
					{
						// all colors vary by +/- 16
						byte CV1 = (byte)(44 + (int)(Random.value * 32) - 16);
						byte CV2 = (byte)(176 + (int)(Random.value * 32) - 16);
						byte CV3 = (byte)(55 + (int)(Random.value * 32) - 16);

						landColors[j,k] = new Color32(CV1, CV2, CV3, 255);

						texture.SetPixel(xPos + j, yPos + k, landColors[j,k]);
					}
				}
			}

			islands[i] = new Island(landColors, xPos, yPos, size, shape);			
		}
	}

	// Return formerly visible area to darkness
	public void ShipLeave(Vector3 position, Vector3 extent)
	{
		int startX = (int)(position.x - extent.x) / 10 - 5;
		int endX = (int)(position.x + extent.x) / 10 + 5;
		int startY = (int)(position.y - extent.y) / 10 - 5;
		int endY = (int)(position.y + extent.y) / 10 + 5;
		
		//Uncover new position
		for (int i = startX; i < endX; i++) 
		{
			for (int j = startY; j < endY; j++)
			{
				if (i < 100 && i >= 0 && j < 100 && j >= 0)
				{
					texture.SetPixel(i,j,new Color32(0,128,255,255));
				}
			}
		}
		
		texture.Apply();
	}

	// Ship can see 5+ from its extents
	public void ShipSight(Vector3 position, Vector3 extent)
	{
		int startX = (int)(position.x - extent.x) / 10 - 5;
		int endX = (int)(position.x + extent.x) / 10 + 5;
		int startY = (int)(position.y - extent.y) / 10 - 5;
		int endY = (int)(position.y + extent.y) / 10 + 5;

		//Uncover new position yet maintain islands
		for (int i = startX; i < endX; i++) 
		{
			for (int j = startY; j < endY; j++)
			{
				if (i < 100 && i >= 0 && j < 100 && j >= 0)
				{
					texture.SetPixel(i,j,new Color32(0,192,255,255));

					foreach (Island land in islands)
					{
						for (int m = land.xPos; m < land.xPos + land.landSize; m++)
						{
							for (int n = land.yPos; n < land.yPos + land.landSize; n++)
							{
								if (m == i && n == j && land.landShape[m - land.xPos, n - land.yPos]) {
									texture.SetPixel(i,j,land.landColor[m - land.xPos, n - land.yPos]);
								}
							}
						}
					}
				}
			}
		}

		texture.Apply();
	}
}

// structure that holds shape, position, and color of an island
public class Island
{
	public Color32[,] landColor;
	public int xPos;
	public int yPos;
	public int landSize;
	public bool[,] landShape;

	public Island(Color32[,] lColor, int x, int y, int size, bool[,] shape)
	{
		landColor = lColor;
		xPos = x;
		yPos = y;
		landSize = size;
		landShape = shape;
	}
}