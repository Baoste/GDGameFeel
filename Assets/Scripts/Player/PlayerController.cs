
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb { get; private set; }
    private PlayerInput playerInput;
    //private InputControls inputActions;
    private Player player;

    private Vector3Int selectedCell;
    private Vector3Int? lastHighlightedCell = null;
    private Color highlightColor = Color.yellow;
    private Color originalColor = Color.white;

    private Tilemap tilemap;
    private new Camera camera;
    

    #region Move
    [Header("Move")]
    public float moveSpeed = 9f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 24f;
    [SerializeField] private float velPower = 0.87f;
    [SerializeField] private float frictionAmount = 0.22f;
    private Vector2 inputVec;
    private float moveDeadArea = 0.7f;
    public Gamepad gamepad { get; private set; }
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
    public float recoil = 18f;
    #endregion

    #region Respawn
    private float submitTrigger = 0.0f;
    private Vector2 spawnVector = Vector2.zero;
    #endregion

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        var players = FindObjectsOfType<Player>();
        player = players.FirstOrDefault(p => p.playerIndex == playerInput.playerIndex);
        player.controller = this;
        rb = player.GetComponent<Rigidbody2D>();

        //inputActions = new InputControls();
        //if (playerInput.defaultActionMap == "Player1")
        //{
        //    inputActions.Player1.Enable();
        //    inputActions.Player1.Move.performed += OnMove;
        //    inputActions.Player1.Dash.performed += OnDash;
        //    inputActions.Player1.Aim.performed += OnAim;
        //    inputActions.Player1.Fire.performed += OnFirePerformed;
        //    inputActions.Player1.Fire.canceled += OnFireCanceled;
        //}
        //else if (playerInput.defaultActionMap == "Player2")
        //{
        //    inputActions.Player2.Enable();
        //    inputActions.Player2.Move.performed += OnMove;
        //    inputActions.Player2.Move.canceled += OnMoveCanceled;
        //    inputActions.Player2.Dash.performed += OnDash;
        //    inputActions.Player2.Aim.performed += OnAim;
        //    inputActions.Player2.Fire.performed += OnFirePerformed;
        //    inputActions.Player2.Fire.canceled += OnFireCanceled;
        //}
        tilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
        camera = Camera.main;
        
        selectedCell = tilemap.WorldToCell(camera.transform.position);
        HighlightCell(selectedCell);
    }

    void Start()
    {
        gamepad = Gamepad.current;
    }

    private void Update()
    {
    }

    private void OnDestroy()
    {
        //if (playerInput.defaultActionMap == "Player1")
        //    inputActions.Player1.Disable();
        //else if (playerInput.defaultActionMap == "Player2")
        //    inputActions.Player2.Disable();
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
        if (v.magnitude > 0.5f)
            rb.AddForce(-v * amount, ForceMode2D.Impulse);
        else
            rb.velocity = Vector2.zero;
    }

    #region DashFunc
    public void PlayerDashIn()
    {
        Vector2 direction = Vector2.zero - rb.velocity.normalized;
        float vel = Mathf.Clamp(rb.velocity.sqrMagnitude, 0, 20f);
        float dashAmount = Mathf.Pow(vel * dashInDcc, dashPower);
        rb.AddForce(dashAmount * direction);
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

        Arrow arrow = player.arrow;
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


    private void OnMove(InputValue value)
    {
        Vector2 moveArea = value.Get<Vector2>();
        if (moveArea.magnitude > moveDeadArea)
        {
            inputVec = moveArea;
            recentInputVec = inputVec;
        }
        else
        {
            inputVec = Vector2.zero;
        }
    }

    //private void OnMoveCanceled(InputAction.CallbackContext context)
    //{
    //    inputVec = Vector2.zero;
    //}

    private void OnDash(InputValue value)
    {
        dashTrigger = value.Get<float>();
    }

    private void OnAim(InputValue value)
    {
        Vector2 tmp = value.Get<Vector2>();
        if (tmp.magnitude > 0.5f)
            aimVec = tmp;
        if (tmp.magnitude > 1.1f)
        {
            Vector2 p = player.transform.position;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2 Worldpos2D = new Vector2(Worldpos.x, Worldpos.y);
            aimVec = (Worldpos2D - p).normalized;
        }
    }

    private void OnCharge(InputValue value)
    {
        fireTrigger = 0;
        fireReady = value.Get<float>();
    }
    private void OnFire(InputValue value)
    {
        fireReady = 0;
        fireTrigger = 1;
        player.canAim = true;

        SetGamepadMotor(0);
    }

    public void SetGamepadMotor(float f)
    {
        if (gamepad != null)
            gamepad.SetMotorSpeeds(f, f);
    }

    private void OnRespawn(InputValue value)
    {

        Debug.Log(value);
        spawnVector = value.Get<Vector2>();
    }

    public void PlayerRespawn()
    {
        Debug.Log(player.isChoosing);
        if (!player.isChoosing) return;

        Vector3Int tmp = new Vector3Int(Mathf.RoundToInt(spawnVector[0]), Mathf.RoundToInt(spawnVector[1]), 0);

        if (tmp != Vector3Int.zero)
        {
            Vector3Int nextCell = selectedCell + tmp;
            if (IsValidCell(nextCell))
            {
                selectedCell = nextCell;
                HighlightCell(selectedCell);
            }
        }
    }
    public void SpawnPlayer(Vector3 position)
    {
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(0);
        Debug.Log("玩家已复活在: " + position);
        player.transform.position = position;
        player.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        player.StartCoroutine(InvincibilityRoutine(player.invincibleDuration));
        player.stateMachine.ChangeState(player.idleState);
    }
    private void OnSubmit(InputValue value)
    {
        submitTrigger = value.Get<float>();
    }

    public void PlayerSubmit()
    {
        if (submitTrigger > 0.0f)
        {
            if (IsValidCell(selectedCell))
            {
                Vector3 respawnWorldPos = tilemap.GetCellCenterWorld(selectedCell);
                SpawnPlayer(respawnWorldPos);
                player.isChoosing = false;

                if (lastHighlightedCell.HasValue)
                {
                    tilemap.SetTileFlags(lastHighlightedCell.Value, TileFlags.None);
                    tilemap.SetColor(lastHighlightedCell.Value, originalColor);
                    lastHighlightedCell = null;
                }
            }
            submitTrigger = 0.0f;
        }

    }

    private bool IsValidCell(Vector3Int cellPos)
    {
        TileBase tile = tilemap.GetTile(cellPos);
        return tile != null;
    }
    private void HighlightCell(Vector3Int cell)
    {
        if (player.isChoosing == true)
        {
            if (lastHighlightedCell.HasValue)
            {
                tilemap.SetTileFlags(lastHighlightedCell.Value, TileFlags.None);
                tilemap.SetColor(lastHighlightedCell.Value, originalColor);
            }

            tilemap.SetTileFlags(cell, TileFlags.None);
            tilemap.SetColor(cell, highlightColor);
            lastHighlightedCell = cell;
        }
    }
    private IEnumerator InvincibilityRoutine(float duration)
    {
        player.isInvincible = true;

        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        float timer = 0f;

        while (timer < duration)
        {
            // 简单闪烁效果
            sr.color = new Color(1f, 1f, 1f, 0.5f); // 半透明
            yield return new WaitForSeconds(0.2f);
            sr.color = new Color(1f, 1f, 1f, 1f); // 不透明
            yield return new WaitForSeconds(0.2f);

            timer += 0.4f;
        }

        sr.color = new Color(1f, 1f, 1f, 1f); // 最终恢复正常
        player.isInvincible = false;
    }
    public Vector3Int GetSelectedCell()
    {
        return selectedCell;
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }
}