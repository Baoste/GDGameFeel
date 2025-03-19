
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb { get; private set; }
    private PlayerInput playerInput;
    private InputControls inputActions;
    private Player player;

    #region Move
    [Header("Move")]
    public float moveSpeed = 9f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 24f;
    [SerializeField] private float velPower = 0.87f;
    [SerializeField] private float frictionAmount = 0.22f;
    private Vector2 inputVec;
    private float moveDeadArea = 0.25f;
    private Gamepad gamepad;
    #endregion

    #region Dash
    [Header("Dash")]
    [SerializeField] private float dashMaxSpeed = 800f;
    [SerializeField] private float dashInDcc = 150f;
    [SerializeField] private float dashAcc = 28f;
    [SerializeField] private float dashDec = 24f;
    [SerializeField] private float dashPower = 0.9f;
    [SerializeField] public float dashAccTime = 0.05f;
    [SerializeField] public float dashTime = 0.15f;
    [SerializeField] public float dashDecTime = 0.3f;
    public bool isDashing
    {
        get
        {
            bool v = dashTrigger > 0;
            if (v)
                dashTrigger = 0;
            return v;
        }
        private set
        {
            dashTrigger = value ? 1 : 0;
        }
    }
    private float dashTrigger = 0;
    private Vector2 recentInputVec = Vector2.right;
    #endregion

    #region Aim
    public Vector2 aimVec { get; private set; }
    private Vector2 recentAimVec = Vector2.up;
    #endregion

    #region File
    public bool isFiring
    {
        get
        {
            bool v = fireReady > 0;
            return v;
        }
        private set
        {
            fireReady = value ? 1 : 0;
        }
    }
    private float fireReady = 0;
    public bool isFire
    {
        get
        {
            bool v = fireTrigger > 0;
            if (v)
            {
                fireReady = 0;
                fireTrigger = 0;
            }
            return v;
        }
        private set
        {
            fireTrigger = value ? 1 : 0;
        }
    }
    private float fireTrigger = 0;
    public float recoil = 3f;
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inputActions = new InputControls();

        if (playerInput.defaultActionMap == "Player1")
        {
            inputActions.Player1.Enable();
            inputActions.Player1.Move.performed += OnMove;
            inputActions.Player1.Dash.performed += OnDash;
            inputActions.Player1.Aim.performed += OnAim;
            inputActions.Player1.Fire.performed += OnFirePerformed;
            inputActions.Player1.Fire.canceled += OnFireCanceled;
        }
        else if (playerInput.defaultActionMap == "Player2")
        {
            inputActions.Player2.Enable();
            inputActions.Player2.Move.performed += OnMove;
            inputActions.Player2.Move.canceled += OnMoveCanceled;
            inputActions.Player2.Dash.performed += OnDash;
            inputActions.Player2.Aim.performed += OnAim;
            inputActions.Player2.Fire.performed += OnFirePerformed;
            inputActions.Player2.Fire.canceled += OnFireCanceled;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        gamepad = Gamepad.current;
    }

    private void Update()
    {
        // make sure stick will read zero
        if (playerInput.defaultActionMap == "Player1")
        {
            Vector2 leftStick = gamepad.leftStick.ReadValue();
            if (leftStick.magnitude < 0.1f)
                inputVec = Vector2.zero;
        }
    }

    private void OnDestroy()
    {
        if (playerInput.defaultActionMap == "Player1")
            inputActions.Player1.Disable();
        else if (playerInput.defaultActionMap == "Player2")
            inputActions.Player2.Disable();
    }

    #region MoveFunc
    public bool isMoving()
    {
        if (inputVec.magnitude > moveDeadArea)
            return true;
        return false;
    }

    public void PlayerMove()
    {
        Vector2 targetSpeed = moveSpeed * inputVec;
        Vector2 speedDif = targetSpeed - rb.velocity;
        float speedDist = speedDif.sqrMagnitude;
        float accelRate = (targetSpeed.magnitude > moveDeadArea) ? acceleration : deceleration;
        float movement = Mathf.Pow(speedDist * accelRate, velPower);
        rb.AddForce(movement * speedDif.normalized);
        //rb.velocity = targetSpeed;
    }
    #endregion

    public void PlayerIdle()
    {
        // Friction
        Vector2 v = rb.velocity;
        float amount = Mathf.Min(v.sqrMagnitude, frictionAmount);
        if (v.magnitude > 0.2f)
            rb.AddForce(-v * amount, ForceMode2D.Impulse);
        else
            rb.velocity = Vector2.zero;
    }

    #region DashFunc
    public void PlayerDashIn()
    {
        Vector2 direction = rb.velocity.normalized;
        float dashAmount = Mathf.Pow(rb.velocity.sqrMagnitude * dashInDcc, dashPower);
        rb.AddForce(dashAmount * -direction);
    }

    public void PlayerDashTime()
    {
        Vector2 direction = recentInputVec;
        if (rb.velocity.sqrMagnitude > 0.5f)
            direction = rb.velocity.normalized;
        float dashDif = dashMaxSpeed - rb.velocity.sqrMagnitude;
        float dashAmount = Mathf.Pow(Mathf.Abs(dashDif) * dashAcc, dashPower) * Mathf.Sign(dashDif);
        rb.AddForce(dashAmount * direction);
    }

    public void PlayerDashOut()
    {
        Vector2 targetSpeed = moveSpeed * inputVec;
        Vector2 dashDif = targetSpeed - rb.velocity;
        float dashDist = dashDif.sqrMagnitude;
        float dashAmount = Mathf.Pow(Mathf.Abs(dashDist) * dashDec, dashPower);
        rb.AddForce(dashAmount * dashDif.normalized);
    }
    #endregion

    #region Aim & Fire
    public void PlayerAim()
    {
        Vector2 speedDif = - rb.velocity;
        float speedDist = speedDif.sqrMagnitude;
        float movement = Mathf.Pow(speedDist * 28, 0.82f);
        rb.AddForce(movement * speedDif.normalized);

        Arrow arrow = GetComponent<Player>().arrow;
        if (arrow)
            recentAimVec = arrow.aimDirection;
    }
    public void PlayerFire()
    {
        Vector2 speedDif = -recoil * recentAimVec - rb.velocity;
        float speedDist = speedDif.sqrMagnitude;
        float movement = Mathf.Pow(speedDist * 38, 0.82f);
        rb.AddForce(movement * speedDif.normalized);
    }
    #endregion


    private void OnMove(InputAction.CallbackContext context)
    {
        inputVec = context.ReadValue<Vector2>();
        if (inputVec.magnitude > moveDeadArea)
            recentInputVec = inputVec;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        inputVec = Vector2.zero;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        dashTrigger = context.ReadValue<float>();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        Vector2 tmp = context.ReadValue<Vector2>();
        if (tmp.magnitude > 0.5f)
            aimVec = tmp;
        if (playerInput.defaultActionMap == "Player2")
        {
            Vector2 p = GetComponentInParent<Player>().transform.position;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2 Worldpos2D = new Vector2(Worldpos.x, Worldpos.y);
            aimVec = (Worldpos2D- p).normalized;
        }
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        fireTrigger = 0;
        fireReady = context.ReadValue<float>();
    }
    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        fireReady = 0;
        fireTrigger = 1;
        player.canAim = true;
    }
}
