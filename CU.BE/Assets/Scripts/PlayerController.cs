using System.Collections;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    [Tooltip("The GameObject that represents the position of the left lane.")]
    public Transform LeftLane;
    [Tooltip("The GameObject that represents the position of the middle lane.")]
    public Transform MiddleLane;
    [Tooltip("The GameObject that represents the position of the left lane.")]
    public Transform RightLane;

    [Tooltip("How long it takes to move between lanes.")]
    public float LerpTime = 0.12f;

    [Tooltip("How long to slow down the player before continuing.")]
    public float HitTime = 1.0f;

    [Tooltip("Scale to apply to the game time when the player is hit (a scale of 0.5 reduces it to 1/2 speed).")]
    public float HitTimeScale = 0.5f;

    public float speed = 25.0f;
    [Tooltip("Speed (units/sec) when changing lanes. Increase to make swaps faster.")]
    public float switchSpeed = 25.0f;
    bool laneChanging = false;
    private Coroutine laneChangeCoroutine = null;
    float step;
    Vector3 targetPos;
    public bool rolling = false;
    bool true_rolling = false;
    CapsuleCollider normalPlayerCol;
    BoxCollider rollingPlayerCol;
    Animator playerAnimator;
    private bool runCoroutineActive = false;
    Rigidbody rb;
    int instancesRunning = 0;
    public bool movingForward = false;
    float percSpeed = 0.0f;
    public GameObject NButton;
    public GameObject MButton;


    enum PlayerState
    {
        Left,
        Middle,
        Right,
        Moving
    }

    private PlayerState state = PlayerState.Middle;
    private PlayerState pendingFinalState;
    private PlayerState previousState;

    private void Start()
    {
        normalPlayerCol = gameObject.GetComponent<CapsuleCollider >();
        rollingPlayerCol = gameObject.GetComponent<BoxCollider>();
        playerAnimator = gameObject.GetComponentInChildren<Animator>();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    //switch lanes
    IEnumerator MoveToTarget(Transform target, PlayerState finalState)
    {
        GameState.LaneChanges += 1;
        state = PlayerState.Moving;

        // Ensure target is set (StartLaneChange sets these too) and mark lane change active
        targetPos = target.transform.position;
        laneChanging = true;

        while (Mathf.Abs(transform.position.x - target.position.x) > 0.02f)
        {
            yield return null;
        }

        transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
        laneChanging = false;
        state = finalState;
        laneChangeCoroutine = null;
    }

    private void StartLaneChange(Transform target, PlayerState finalState)
    {
        // Stop any active lane-change coroutine so we can change direction instantly
        if (laneChangeCoroutine != null)
        {
            StopCoroutine(laneChangeCoroutine);
            laneChangeCoroutine = null;
        }

        // set immediate target and start new coroutine
        // record previous/start state only if we're not already in a moving state
        if (state != PlayerState.Moving)
        {
            previousState = state;
        }
        pendingFinalState = finalState;
        targetPos = target.transform.position;
        laneChanging = true;
        state = PlayerState.Moving;
        laneChangeCoroutine = StartCoroutine(MoveToTarget(target, finalState));
    }

    private Transform LaneTransformForState(PlayerState s)
    {
        switch (s)
        {
            case PlayerState.Left:
                return LeftLane;
            case PlayerState.Middle:
                return MiddleLane;
            case PlayerState.Right:
                return RightLane;
            default:
                return MiddleLane;
        }
    }

    //handles movement
    private void FixedUpdate()
    {
        // FixedUpdate handles physics and movement while laneChanging
        //movement hwen changing lanes
        if (laneChanging)
        {
            step = switchSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPos.x, transform.position.y, transform.position.z), step);
        }
    }
    IEnumerator MovingTime()
    {
        if (runCoroutineActive)
            yield break;

        runCoroutineActive = true;
        instancesRunning += 1;

        // Cycle per-segment speed multipliers: 100%, 125%, 150% repeating every 3 segments
        int cycleIndex = GameState.Segment % 3;
        float multiplier;
        switch (cycleIndex)
        {
            case 0:
                multiplier = 1.0f;
                break;
            case 1:
                multiplier = 1.25f;
                break;
            default:
                multiplier = 1.5f;
                break;
        }
        float effectiveSpeed = speed * multiplier;

        // ramp up while W is held (or until max)
        if (percSpeed < 1)
        {
            movingForward = true;
            playerAnimator.SetBool("moveF", true);
            yield return new WaitForSeconds(0.15f);
            while (percSpeed < 1 && Input.GetKey(KeyCode.W))
            {
                rb.velocity = new Vector3(0, 0, percSpeed * effectiveSpeed);
                playerAnimator.SetFloat("movingSpeed", Mathf.Max(0.7f, percSpeed));
                percSpeed += 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
            rb.velocity = new Vector3(0, 0, percSpeed * effectiveSpeed);
            playerAnimator.SetFloat("movingSpeed", Mathf.Max(0.7f, percSpeed));
        }

        // maintain forward movement while W is held
        while (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector3(0, 0, percSpeed * effectiveSpeed);
            yield return null;
        }

        // ramp down when W released (gradual deceleration to zero)
        percSpeed = Mathf.Min(percSpeed, 1f);
        while (instancesRunning == 1 && percSpeed > 0f)
        {
            if (percSpeed < 0.75f && movingForward)
            {
                movingForward = false;
                StartCoroutine(StillTutorial());
            }
            playerAnimator.SetBool("moveF", false);
            // smaller steps for a smoother deceleration
            percSpeed -= 0.05f;
            percSpeed = Mathf.Max(0f, percSpeed);
            playerAnimator.SetFloat("movingSpeed", percSpeed);
            rb.velocity = new Vector3(0, 0, percSpeed * effectiveSpeed);
            yield return new WaitForSeconds(0.05f);
        }
        if (instancesRunning == 1)
        {
            rb.velocity = Vector3.zero;
        }
        instancesRunning -= 1;
        runCoroutineActive = false;
    }
    //show n and m button icons if player stands still
    IEnumerator StillTutorial()
    {
        NButton.SetActive(true);
        MButton.SetActive(true);
        while (!movingForward)
        {
            yield return null;
        }
        NButton.SetActive(false);
        MButton.SetActive(false);
    }
    void Update()
    {
        // Lane change input handling (count only once per key press; supports mid-swap reversal)
        if (movingForward)
        {
            bool leftPressed = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
            bool rightPressed = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

            if (leftPressed)
            {
                if (laneChanging)
                {
                    // if we were heading right, reverse back to previous lane
                    if (pendingFinalState == PlayerState.Right)
                    {
                        StartLaneChange(LaneTransformForState(previousState), previousState);
                    }
                }
                else
                {
                    if (state == PlayerState.Middle)
                        StartLaneChange(LeftLane, PlayerState.Left);
                    else if (state == PlayerState.Right)
                        StartLaneChange(MiddleLane, PlayerState.Middle);
                }
            }
            else if (rightPressed)
            {
                if (laneChanging)
                {
                    // if we were heading left, reverse back to previous lane
                    if (pendingFinalState == PlayerState.Left)
                    {
                        StartLaneChange(LaneTransformForState(previousState), previousState);
                    }
                }
                else
                {
                    if (state == PlayerState.Middle)
                        StartLaneChange(RightLane, PlayerState.Right);
                    else if (state == PlayerState.Left)
                        StartLaneChange(MiddleLane, PlayerState.Middle);
                }
            }
        }
        if ((Input.GetKeyDown(KeyCode.Space)) && !rolling && movingForward)
        {
            StartCoroutine(rollForward());
        }

        // Start running when W is pressed (hold W to keep running)
        if (Input.GetKeyDown(KeyCode.W) && !runCoroutineActive)
        {
            StartCoroutine(MovingTime());
        }

        //log button presses
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameState.LiveInput("A");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GameState.LiveInput("D");
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            GameState.NMInput("N");
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            GameState.NMInput("M");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            GameState.LiveInput("Space");
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameState.LiveInput("Left");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameState.LiveInput("Right");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Obstacle"))
        {
            StartCoroutine(Hit());
        }
    }

    /// <summary>
    /// Player hit an obstacle. Slow down for a bit...
    /// </summary>
    IEnumerator Hit()
    {
        Time.timeScale = HitTimeScale;

        yield return new WaitForSeconds(HitTime);

        // Restore the previous time scale.
        Time.timeScale = 1;
    }
    //roll forward
    public IEnumerator rollForward()
    {
        rolling = true;
        playerAnimator.SetBool("rollF", true);
        // grant immediate roll immunity and swap colliders
        true_rolling = true;
        normalPlayerCol.enabled = false;
        rollingPlayerCol.enabled = true;

        // keep the animator timing intact
        yield return new WaitForSeconds(0.3f);
        yield return null;
        playerAnimator.SetBool("rollF", false);
        //wait untill roll animation ends
        yield return new WaitForSeconds(0.6f);
        //reset colliders to default
        normalPlayerCol.enabled = true;
        rollingPlayerCol.enabled = false;
        true_rolling = false;
        yield return new WaitForSeconds(0.5f);
        rolling = false;
    }
}
