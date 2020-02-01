﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour {


    [Header("References")]

    [SerializeField]
    private Map map;

    public TextMeshProUGUI woodText;
    public TextMeshProUGUI keyText;
    public int woodCount = 0;
    public int keyCount = 0; 
    public bool isMoving = false;

    public bool onCooldown = false;
    public bool onExit = false;
    private float moveTime = 0.1f;

    private AudioSource walkingSound;

    //public RuleTile tileBridge;

    // Use this for initialization
    void Start () {

        //We don't show the item counts until we get at least one of them ( then they remain enabled ).
        woodText.enabled = false;
        keyText.enabled = false;
        updateKeyText();
        updateWoodText();
        bridges = new List<Bridge>();

        //Default walking sound is the grass one
        grassSound();
    }
	
	// Update is called once per frame
	void Update () {


        //We do nothing if the player is still moving.
        if (isMoving || onCooldown || onExit ) return;

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
            StartCoroutine(actionCooldown(0.2f));
            Move(horizontal, vertical);
        }

	}

    private void Move(int xDir, int yDir)
    {

        Vector2 startCell = transform.position;
        Vector2 targetCell = startCell + new Vector2(xDir, yDir).normalized;

        bool isOnGround = map.IsGround(startCell); //If the player is on the ground
        bool isOnBridge = map.IsBridge(startCell); //If the player is on a bridge

        bool hasGroundTile = map.IsGround(targetCell); //If target Tile has a ground
        bool hasObstacleTile = map.IsObstacle(targetCell); //if target Tile has an obstacle
        bool hasBridgeTile = map.IsBridge(targetCell); //if target Tile has a bridge (plank)

        //If the player starts their movement from a ground tile.
        if (isOnGround)
        {

            //If the front tile is a walkable ground tile, the player moves here.
            if (hasGroundTile && !hasObstacleTile)
            {
                if ( doorCheck(targetCell) )
                {
                    grassSound();
                    StartCoroutine(SmoothMovement(targetCell));
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));
               
            }

            //If the front tile is a bridge plank
            else if (hasBridgeTile)
            {
                if ( doorCheck(targetCell ))
                {
                    currentBridge = Bridge.belongsTo(bridges, targetCell);
                    Debug.Log(currentBridge);
                    //We verify we are entering the bridge from one of its extremity 
                    if (currentBridge.isExtremity(targetCell))
                    {
                        //Debug.Log("Entering a bridge!");
                        // If we are entering the bridge from the start, we reverse end/beginning to handle its treatement more easily.
                        if (currentBridge.firstPlank().Equals(targetCell))
                            currentBridge.reverseBridge();
                        currentBridge.setHasEnd(false);

                        bridgeSound();
                        StartCoroutine(SmoothMovement(targetCell));
                    }
                    else //otherwise we ignore it.
                        currentBridge = null;
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));
            }
            //If the player has wood to build a new bridge over the water, he build a bridge and moves there.
            else if (!hasGroundTile && !hasBridgeTile && woodCount > 0 && !hasObstacleTile )
            {
                if (doorCheck(targetCell))
                {
                    //Debug.Log("Building a new bridge !");
                    woodCount--; updateWoodText();
                    currentBridge = new Bridge(bridgesTilemap, bridgeTiles); //We create a new bridge
                                                                             //currentBridge = ScriptableObject.CreateInstance<Bridge>().InitB(bridgeTiles);
                                                                             //currentBridge.initB(bridgeTiles);
                    bridges.Add(currentBridge); //We add it to the bridge list
                    currentBridge.addPlank(targetCell); //We add a plank to the bridge

                    bridgeSound();
                    StartCoroutine(SmoothMovement(targetCell)); //We move to this new plank.
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));
            }
        }
        //If the player starts their movement from a bridge tile
        else if (isOnBridge)
        {
            //If there's an obstacle
            if ( hasObstacleTile )
                StartCoroutine(BlockedMovement(targetCell));
            //If they are leaving the bridge for a walkable ground tile.
            else if (hasGroundTile)
            {
                Debug.Log("Leaving a bridge !");
                //We check if there's a door where we ends up, and handle it.
                if ( doorCheck(targetCell) )
                {
                    grassSound();
                    StartCoroutine(SmoothMovement(targetCell));

                    //If we are leaving the last plank of the bridge, we delete it.
                    if (currentBridge.firstPlank().Equals(startCell))
                    {
                        Debug.Log("Deleting the bridge !");
                        currentBridge.removeLastPlank();
                        bridges.Remove(currentBridge);
                        woodCount++; updateWoodText();
                    }
                    else
                        currentBridge.setHasEnd(true); //The bridge get an end.

                    currentBridge = null;
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));

            }
            //If they keep walking toward the water, they have wood, and it's the end of the bridge.
            else if (  !hasBridgeTile && woodCount > 0 && currentBridge.isOnLastPlank(startCell) )
            {
                if ( doorCheck(targetCell) )
                {
                    Debug.Log("adding a plank!");
                    woodCount--; updateWoodText();
                    currentBridge.addPlank(targetCell);

                    bridgeSound();
                    StartCoroutine(SmoothMovement(targetCell));
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));
            }
            //If they are backtracking the bridge
            else if ( currentBridge.playerIsBackTracking(startCell, targetCell) )
            {
                if (doorCheck(targetCell))
                {
                    Debug.Log("Backtracking!");

                    bridgeSound();
                    StartCoroutine(SmoothMovement(targetCell));
                    currentBridge.removeLastPlank();
                    woodCount++; updateWoodText();
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));
            }

        }

        if (!isMoving)
            StartCoroutine(BlockedMovement(targetCell));

        
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        //while (isMoving) yield return null;

        isMoving = true;

        //Play movement sound
        if ( walkingSound != null )
        {
            walkingSound.loop = true;
            walkingSound.Play();
        }

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        if (walkingSound != null)
            walkingSound.loop = false;

        isMoving = false;
    }

    //Blocked animation
    private IEnumerator BlockedMovement(Vector3 end)
    {
        //while (isMoving) yield return null;

        isMoving = true;


        if (AudioManager.getInstance() != null)
            AudioManager.getInstance().Find("blocked").source.Play();

        Vector3 originalPos = transform.position;

        end = transform.position + ((end - transform.position) / 3);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = (1 / (moveTime*2) );

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

        //The lever disable the sound so its doesn't overlap with this one, so it blocked has been muted, we restore it.
        if ( AudioManager.getInstance() != null && AudioManager.getInstance().Find("blocked").source.mute )
        {
            AudioManager.getInstance().Find("blocked").source.Stop();
            AudioManager.getInstance().Find("blocked").source.mute = false;
        }
        isMoving = false;
    }

    public IEnumerator Teleport(PassageWay Teleporter, float aTime)
    {

        //If the teleporter is in use, abort
        if (Teleporter.isTeleporting) yield break;

        //We wait for any other movement coroutines to finish before starting this one.
        while (isMoving) yield return null;

        isMoving = true;

        Debug.Log("Teleporting from " + name);

        //We set both teleporters as "In Use"
        Teleporter.setTeleportersAvailability(true);
        //we prevent the player from moving while teleporting

        float alpha = GetComponent<Renderer>().material.color.a;

        //The character disappear
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0f, t));
            GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }

        //Now me teleport the player
        transform.position = Teleporter.exitPos();

        //The character fades back to reality 
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(0f, alpha, t));
            GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }

        //We allow the player to move again
        isMoving = false;
        //We set both teleporter as "Available"
        Teleporter.setTeleportersAvailability(false);
    }

    private IEnumerator actionCooldown(float cooldown)
    {
        onCooldown = true;

        //float cooldown = 0.2f;
        while ( cooldown > 0f )
        {
            cooldown -= Time.deltaTime;
            yield return null;
        }

        onCooldown = false;
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

    //A method that handle doors : Return true if you can move on the tile, false otherwise. 
    //If the door can be opened, opens it.
    //TO ADD : Levered Doors.
    private bool doorCheck(Vector2 targetCell)
    {
        Collider2D coll = whatsThere(targetCell);

        //No obstacle, we can walk there
        if (coll == null)
            return true;

        //If there's a golden door in front of the character
        if (coll.tag == "Door")
        {
            Door door = coll.gameObject.GetComponent<Door>();
            //If the door is closed and we can open it
            if (door.IsClosed() && keyCount > 0)
            {
                keyCount--; updateKeyText();
                door.open();
                return true;
            }
            else if (!door.IsClosed()) //If it's already opened
                return true;
            else //If it's closed.
                return false;
        }
        //if there's a levered door in front of the character.
        else if (coll.tag == "LeveredDoor")
        {
            Debug.Log("LeveredDoor detected!");
            LeveredDoor door = coll.gameObject.GetComponent<LeveredDoor>();
            //If the door is open
            if (door.isOpen)
                return true;
            //If the door is close.
            else
                return false;
        }
        else if (coll.tag == "Lever" )
        {

            //Click sound !
            if (AudioManager.getInstance() != null)
            {
                AudioManager.getInstance().Find("leverClick").source.Play();
                AudioManager.getInstance().Find("blocked").source.mute = true;
            }
            Lever lever = coll.gameObject.GetComponent<Lever>();
            lever.operate();
            //We operate the lever, but can't move there, so we return false;
            return false;
        }
        else
            return true;
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        //Debug.Log("Something touched!");
        //If we collided with the exit, we load the next level in two seconds.
        if ( coll.tag == "Exit")
        {
            Debug.Log("Sortie touché!");
            if (AudioManager.getInstance() != null)
                AudioManager.getInstance().Find("victory").source.Play();
            onExit = true; //Prevent the player from moving.
            Invoke("NextLevel", 1f);
            //enabled = false;
        }
        else if ( coll.tag == "Wood")
        {
            woodCount++; updateWoodText();
            //Debug.Log("You picked up wood ! You have " + woodCount + "piece of woods.");
            coll.gameObject.SetActive(false);

            if (AudioManager.getInstance() != null)
                AudioManager.getInstance().Find("woodpickup").source.Play();
        }
        else if ( coll.tag == "Passage" )
        {
            //Debug.Log("Teleport!");
            PassageWay passage = coll.gameObject.GetComponent<PassageWay>();
            //StartCoroutine(Teleport(passage, 0.2f));
            StartCoroutine(passage.Teleport(this, 0.2f));
            //StartCoroutine(actionCooldown(0.4f));
        }
        else if ( coll.tag == "Key" )
        {
            //Debug.Log("Key picked!");
            if (AudioManager.getInstance() != null)
                AudioManager.getInstance().Find("keypickup").source.Play();
            keyCount++; updateKeyText();
            coll.gameObject.SetActive(false);
        }
    }

    public void grassSound()
    {
        if (AudioManager.getInstance() != null)
            walkingSound = AudioManager.getInstance().Find("grass").source;
    }
    public void bridgeSound()
    {
        if ( AudioManager.getInstance() != null )
            walkingSound = AudioManager.getInstance().Find("bridge").source;
    }

    public Collider2D whatsThere(Vector2 targetPos)
    {
        RaycastHit2D hit;
        hit = Physics2D.Linecast(targetPos, targetPos);
        return hit.collider;
    }

    public void updateWoodText()
    {
        if (!woodText.enabled && woodCount != 0)
            woodText.enabled = true;
        woodText.text = "wood:" + woodCount;
    }
    public void updateKeyText()
    {
        if (!keyText.enabled && keyCount != 0)
            keyText.enabled = true;
        keyText.text = "keys:" + keyCount;
    }


    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

}
