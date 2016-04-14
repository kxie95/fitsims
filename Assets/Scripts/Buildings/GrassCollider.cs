using UnityEngine;
using System.Collections;

public class GrassCollider : MonoBehaviour {
		
	public tk2dSprite[] singleTiles;
	public tk2dTiledSprite[] tiledTiles;
	public GameObject[] selectedGrassType;	
	public GameObject selectedGrass;
	public int collisionCounter = 0;
	public bool isMoving = true;
	
	public bool inCollision = false;
	public bool initializeOwnGrid = true;
	public Color myGreen;
	
	
	// Use this for initialization
	void Start () 
	{
		InitializeOwnGrid ();
	}
	
	// Update is called once per frame
	void Update () {
				
	
	}

	private void InitializeOwnGrid()
	{
		if(initializeOwnGrid)
		{
			selectedGrassType = GameObject.FindGameObjectsWithTag("Grass");	
			
			foreach (GameObject grass in selectedGrassType) 
			{
				if(((GrassSelector)grass.GetComponent("GrassSelector")).isSelected)				
				{
					selectedGrass = grass;	
					break;
				}
			}
			
			singleTiles = selectedGrass.GetComponentsInChildren<tk2dSprite>();	
			tiledTiles = selectedGrass.GetComponentsInChildren<tk2dTiledSprite>();	
			myGreen = ((tk2dSprite)singleTiles[0]).color;  //new Color(0.59f, 0.785f, 0);	
			initializeOwnGrid = false;
		}		
	}

	void OnCollisionEnter(Collision col)
	{
		if(isMoving)
		{	
			collisionCounter++;			
			
			foreach (tk2dSprite tile in singleTiles) 
			{				
				tile.color = Color.red;			
			}
			
			foreach (tk2dTiledSprite tile in tiledTiles) 
			{
				tile.color = Color.red;	
			}
			
			//if(col.gameObject.tag == "OutofMap")
			//{
				inCollision = true;
			//}			
			
		}
	}
	
	void OnCollisionExit(Collision col)
	{
		if(isMoving)
		{
			collisionCounter--;	
						
			if(collisionCounter == 0)
			{				
				foreach (tk2dSprite tile in singleTiles) 
				{				
					tile.color = myGreen;		
				}
				
				foreach (tk2dTiledSprite tile in tiledTiles) 
				{
					tile.color = myGreen;
				}
				
				inCollision = false;
			}
		}
	}
	
}
