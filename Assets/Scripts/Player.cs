using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cawotte.Toolbox.Audio;

/// <summary>
/// Scripts that handle the Player character
/// </summary>

public class Player : MonoBehaviour {


    [Header("References")]

    [SerializeField]
    private MapGlobalReference mapGlobalReference = null;

    private Map map = null;

    private AudioSourcePlayer audioPlayer = null;

    [Header("ScriptableObject Variables")]

    [SerializeField]
    private int woodCountStartingValue = 0;
    [SerializeField]
    private IntVariable woodCount = null;

    [SerializeField]
    private IntVariable keyCount = null;


    [Header("Player Info")]

    public bool isMoving = false;

    public bool onCooldown = false;
    public bool onExit = false;
    private float moveTime = 0.1f;

    [Header("Audio")]
    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Sound grassStep = null;

    [SerializeField]
    private Sound bridgeStep = null;

    [SerializeField]
    private Sound blockedStep = null;

    public int WoodCount
    {
        get
        {
            return woodCount.Value;
        }
        set
        {
            woodCount.Value = value;
        }
    }

    public int KeyCount
    {
        get
        {
            return keyCount.Value;
        }
        set
        {
            keyCount.Value = value;
        }
    }

    //Screen.SetResolution(1280, 600, false);

    private void Awake()
    {
        audioPlayer = AudioSourcePlayer.AddAsComponent(gameObject, audioManager);
    }

    private void Start()
    {
        map = mapGlobalReference.Map;
        woodCount.Value = woodCountStartingValue;
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
      
        //If there's a direction, we try to move.
        if (horizontal != 0 || vertical != 0)
        {
            StartCoroutine(actionCooldown(0.2f));
            Move(horizontal, vertical);
        }

	}

    private void Move(int xDir, int yDir)
    {


        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + new Vector2(xDir, yDir).normalized;

        
        bool isOnGround = map.IsGround(startPos); //If the player is on the ground
        bool isOnBridge = map.IsBridge(startPos); //If the player is on a bridge

        bool hasGroundTile = map.IsGround(endPos); //If target Tile has a ground
        bool hasObstacleTile = map.IsObstacle(endPos); //if target Tile has an obstacle


        bool canWalk = map.CanMoveFromTo(this, transform.position, endPos);

        bool hasBridgeTile = map.IsBridge(endPos); //if target Tile has a bridge (plank)

        if (canWalk)
        {
            if (hasBridgeTile)
            {
                audioPlayer.PlaySound(bridgeStep);
            } else
            {
                audioPlayer.PlaySound(grassStep);
            }

            StartCoroutine(SmoothMovement(endPos));
        }
        else
        {

            StartCoroutine(BlockedMovement(endPos));
        }

    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        //while (isMoving) yield return null;

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

        isMoving = true;

        //Don't play the blocked sound if there's a lever, no negative feedback.
        if (!map.HasLever(end))
            audioPlayer.PlaySound(blockedStep);

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

        /*
        //The lever disable the sound so its doesn't overlap with this one, so it blocked has been muted, we restore it.
        if ( AudioManager.getInstance() != null && AudioManager.getInstance().Find("blocked").source.mute )
        {
            AudioManager.getInstance().Find("blocked").source.Stop();
            AudioManager.getInstance().Find("blocked").source.mute = false;
        } */

        isMoving = false;
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



}
