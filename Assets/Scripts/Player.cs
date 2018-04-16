using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    public Tilemap groundTiles;
    public Tilemap obstacleTiles;
    public Tilemap bridgeTiles;
    public int woodCount = 0;

    private float moveTime = 0.1f;
    private bool isMoving = false;
    private bool canAct = true;
    private Sprite characterSprite; 

    public Sprite bridgeSprite;
    [SerializeField]
    private Bridge currentBridge;
    private List<Bridge> bridges = new List<Bridge>();

    // Use this for initialization
    void Start () {

        characterSprite = GetComponent<Sprite>();
	}
	
	// Update is called once per frame
	void Update () {

        //We do nothing if the player is still moving.
        if (isMoving || !canAct ) return;

        //To store move directions.
        int horizontal = 0;
        int vertical = 0;
        //To get move directions
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        //We can't go in both directions at the same time
        if ( horizontal != 0 )
            vertical = 0;
      
        //If there's a direction, we are trying to move.
        if (horizontal != 0 || vertical != 0)
        {
            StartCoroutine(actionCooldown());
            Move(horizontal, vertical);
        }

	}

    private void Move(int xDir, int yDir)
    {
        Vector2 startCell = transform.position;
        Vector2 targetCell = startCell + new Vector2(xDir, yDir);

        bool isOnGround = getCell(groundTiles, startCell) != null; //If the player is on the ground
        bool isOnBridge = getCell(bridgeTiles, startCell) != null; //If the player is on a bridge
        bool hasGroundTile = getCell(groundTiles, targetCell) != null; //If target Tile has a ground
        bool hasObstacleTile = getCell(obstacleTiles, targetCell) != null; //if target Tile has an obstacle
        bool hasBridgeTile = getCell(bridgeTiles, targetCell) != null; //if target Tile has a bridge (plank)

        //If the player starts their movement from a ground tile.
        if (isOnGround)
        {

            //If the front tile is a walkable ground tile, the player moves here.
            if (hasGroundTile && !hasObstacleTile)
                StartCoroutine(SmoothMovement(targetCell));

            //If the front tile is a bridge plank
            else if (hasBridgeTile)
            {
                currentBridge = Bridge.belongsTo(bridges, targetCell);

                //We verify we are entering the bridge from one of its extremity 
                if (currentBridge.isExtremity(targetCell))
                {
                    Debug.Log("Entering a bridge!");
                    // If we are entering the bridge from the start, we reverse end/beginning to handle its treatement more easily.
                    if (currentBridge.firstPlank().Equals(targetCell))
                        currentBridge.reverseBridge();
                    currentBridge.setHasEnd(false);

                    StartCoroutine(SmoothMovement(targetCell));
                }
                else //otherwise we ignore it.
                    currentBridge = null;
            }
            //If the player has wood to build a new bridge over the water, he build a bridge and moves there.
            else if (!hasGroundTile && !hasBridgeTile && woodCount > 0)
            {
                Debug.Log("Building a new bridge !");
                woodCount--;
                currentBridge = new Bridge(bridgeTiles, bridgeSprite); //We create a new bridge
                bridges.Add(currentBridge); //We add it to the bridge list
                currentBridge.addPlank(targetCell); //We add a plank to the bridge
                StartCoroutine(SmoothMovement(targetCell)); //We move to this new plank.
            }
        }
        //If the player starts their movement from a bridge tile
        else if (isOnBridge)
        {
            //If they are leaving the bridge for a walkable ground tile.
            if (hasGroundTile && !hasObstacleTile)
            {
                Debug.Log("Leaving a bridge !");
                StartCoroutine(SmoothMovement(targetCell));

                //If we are leaving the last plank of the bridge, we delete it.
                if ( currentBridge.firstPlank().Equals(startCell))
                {
                    Debug.Log("Deleting the bridge !");
                    currentBridge.removeLastPlank();
                    bridges.Remove(currentBridge);
                    woodCount++;
                }
                else
                    currentBridge.setHasEnd(true); //The bridge get an end.

                currentBridge = null;
            }
            //If they keep walking toward the water, they have wood, and it's the end of the bridge.
            else if ( !hasGroundTile && !hasBridgeTile && woodCount > 0 && currentBridge.isOnLastPlank(startCell) )
            {
                Debug.Log("adding a plank!");
                woodCount--;
                currentBridge.addPlank(targetCell);
                StartCoroutine(SmoothMovement(targetCell));
            }
            //If they are backtracking the bridge
            else if ( currentBridge.playerIsBackTracking(startCell, targetCell) )
            {
                Debug.Log("Backtracking!");
                StartCoroutine(SmoothMovement(targetCell));
                currentBridge.removeLastPlank();
                woodCount++;
            }

        }

        if (!isMoving)
            StartCoroutine(BlockedMovement(targetCell));

        
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }

    //Blocked animation
    private IEnumerator BlockedMovement(Vector3 end)
    {
        Debug.Log("Blocked");
        isMoving = true;

        Vector3 originalPos = transform.position;

        end = transform.position + ((end - transform.position) / 3);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = (1 / moveTime)/2;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, originalPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }

    private IEnumerator actionCooldown()
    {
        canAct = false;

        float cooldown = 0.2f;
        while ( cooldown > 0f )
        {
            cooldown -= Time.deltaTime;
            yield return null;
        }
        canAct = true;
    }

    IEnumerator Teleport(Vector2 targetPos, float aTime)
    {
        float alpha = GetComponent<Renderer>().material.color.a;

        //The character disappear
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0f, t));
            GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }

        //Now me teleport it
        transform.position = targetPos;

        //Now it fades back to reality 
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 1f, t));
            GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("Something touched!");
        //If we collided with the exit, we load the next level in two seconds.
        if ( coll.tag == "Exit")
        {
            Debug.Log("Sortie touché!");
            canAct = false; //Prevent the player from moving.
            Invoke("NextLevel", 0.9f);
            //enabled = false;
        }
        else if ( coll.tag == "Wood")
        {
            woodCount++;
            Debug.Log("You picked up wood ! You have " + woodCount + "piece of woods.");
            coll.gameObject.SetActive(false);
        }
        else if ( coll.tag == "Passage" )
        {
            Debug.Log("Teleport!");
            PassageWay passage = coll.gameObject.GetComponent<PassageWay>();
            StartCoroutine( Teleport(passage.exitPos(), 0.2f));
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    private TileBase getCell(Tilemap tilemap, Vector2 cellWorldPos)
    {
        return tilemap.GetTile(tilemap.WorldToCell(cellWorldPos));
    }
    private bool hasTile(Tilemap tilemap, Vector2 cellWorldPos)
    {
        return tilemap.HasTile(tilemap.WorldToCell(cellWorldPos));
    }

}
