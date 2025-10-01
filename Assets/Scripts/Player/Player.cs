using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [NonSerialized] public Controls controls;
    private InputAction moveInput;

    [NonSerialized] public Rigidbody2D rb;
    [NonSerialized] public BoxCollider2D playerCollider;
    [NonSerialized] private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    public float movementSpeed = 8;
    public int maxFallSpeed = -10;
    public float groundIntoAirTransitionTime = 0.1f;
    [NonSerialized] public float groundIntoAirTimer;
    [NonSerialized] public Vector2 moveDirection;
    [NonSerialized] public Vector2 playerVelocity;
    [NonSerialized] public bool faceRight;
    [NonSerialized] public float baseGravityScale;
    public LayerMask groundCheckLayer;
    public int playerGroundDrag = -2;

    [Header("Jump")]
    public float jumpStrength = 14;
    public int maxJumpCount = 2;
    [NonSerialized] public int currentJumpCount;
    public float maxJumpTime = 0.3f;
    public float jumpCancelVelocityMultiplier = 20;
    [NonSerialized] public float jumpTimer;
    [NonSerialized] public bool jumpPerformed;

    [Header("Dash")]
    public float dashTime = 0.2f;
    public float dashStrength = 20;
    public int maxDashCount;
    [NonSerialized] public int currentDashCount;

    [Header("IFrames")]
    public float iFramesDuration = 1;
    [NonSerialized] public bool iframesActive;

    //Animations
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string deathState = "Death";

    ////Interaction
    [NonSerialized] public List<IInteractables> interactables = new List<IInteractables>();
    [NonSerialized] public IInteractables currentInteractable;
    public IInteractables closestInteraction;

    [NonSerialized] public PlayerMovement playerMovement = new PlayerMovement();
    [NonSerialized] public PlayerCollision playerCollision = new PlayerCollision();
    [NonSerialized] public PlayerInteraction playerInteraction = new PlayerInteraction();

    [Space]
    public States state;

    public enum States
    {
        Ground,
        GroundIntoAir,
        Air,
        Dash,
        Death,
        Attack,
        Emtpy,
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        controls = Keybindinputmanager.Controls;
        moveInput = controls.Player.Move;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        baseGravityScale = rb.gravityScale;

        playerMovement.player = this;
        playerCollision.player = this;
        //playerInteraction.player = this;

    }
    private void Start()
    {
        state = States.Air;
        //if (health != null) health.dieEvent.AddListener(OnDeath);
    }
    private void OnEnable()
    {
        controls.Enable();
        EnableInputs(true);
    }
    private void OnDisable()
    {
        controls.Disable();
        EnableInputs(false);
    }
    public void EnableInputs(bool enabled)
    {
        if (enabled && this.enabled)
        {
            controls.Player.Jump.performed += playerMovement.JumpInput;
            controls.Player.Dash.performed += playerMovement.DashInput;
            controls.Player.Interact.performed += playerInteraction.InteractInput;
        }
        else
        {
            controls.Player.Jump.performed -= playerMovement.JumpInput;
            controls.Player.Dash.performed -= playerMovement.DashInput;
            controls.Player.Interact.performed -= playerInteraction.InteractInput;
        }
    }
    private void FixedUpdate()
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;

        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerMovement.GroundMovement();
                break;
            case States.GroundIntoAir:
                playerMovement.AirMovement();
                break;
            case States.Air:
                playerMovement.AirMovement();
                break;
            case States.Dash:
                playerMovement.DashMovement();
                break;
        }
    }
    private void Update()
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;

        ReadMovementInput();
        playerInteraction.InteractionUpdate();

        switch (state)
        {
            case States.Emtpy:
                break;
            case States.Ground:
                playerCollision.GroundCheck();
                playerMovement.RotatePlayer();
                break;
            case States.GroundIntoAir:
                playerMovement.JumpIsPressed();
                playerMovement.GroundIntoAirTransition();
                playerCollision.AirCheck();
                playerMovement.RotatePlayer();
                break;
            case States.Air:
                playerMovement.JumpIsPressed();
                playerCollision.AirCheck();
                playerMovement.RotatePlayer();
                break;
            case States.Dash:
                playerMovement.DashTime();
                break;
        }
    }
    private void ReadMovementInput()
    {
        moveDirection.x = moveInput.ReadValue<Vector2>().x;
    }
    public void SwitchToGround(bool onlyResetValues)
    {
        rb.gravityScale = baseGravityScale;
        currentDashCount = 0;
        currentJumpCount = 0;

        jumpPerformed = false;

        if (onlyResetValues == false)
        {
            state = States.Ground;
        }
    }
    public void SwitchGroundIntoAir()
    {
        groundIntoAirTimer = 0;
        state = States.GroundIntoAir;
    }
    public void SwitchToAir()
    {
        if (currentJumpCount == 0) currentJumpCount++;
        rb.gravityScale = baseGravityScale;

        state = States.Air;
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    public void IFramesStart()
    {
        if (iframesActive) return;

        StartCoroutine(IFrames());
    }
    IEnumerator IFrames()
    {
        iframesActive = true;
        yield return new WaitForSeconds(iFramesDuration);
        iframesActive = false;
    }
    private void OnDeath()
    {
        //animation
        //playerAttack.state = PlayerAttack.States.Empty;
        rb.linearVelocity = Vector2.zero;
        ChangeAnimationState(deathState);
        state = States.Death;

        //AudioManager.Instance.PlayAudioFileOneShot(AudioManager.Instance.utilityFiles[(int)AudioManager.UtilitySounds.PlayerDeath]);
    }
    public void RestartGame()
    {
        IngameController.Instance.menuController.ResetPlayer(false);
    }
    public void StopPlayerControls()
    {
        Player.Instance.rb.linearVelocity = Vector2.zero;
        Player.Instance.SwitchToGround(true);
        Player.Instance.ChangeAnimationState("Idle");
        Player.Instance.state = Player.States.Ground;

        IngameController.Instance.menuController.gameIsPaused = true;
    }
    public void StartPlayerControls()
    {
        IngameController.Instance.menuController.gameIsPaused = false;
    }
}
