
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb { get; private set; }
    private PlayerInput playerInput;
    //private InputControls inputActions;
    private Player player;

    public Vector3Int selectedCell { get; private set; }
    private Vector3Int? lastHighlightedCell = null;
    private Color highlightColor;
    private Color originalColor = Color.white;

    public Tilemap tilemap { get; private set; }
    public List<Tilemap> walls { get; private set; }
    

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
    public bool isSubmit
    {
        get
        {
            bool v = submitTrigger > 0;
            if (v)
                submitTrigger = 0;
            return v;
        }
        private set
        {
            submitTrigger = value ? 1 : 0;
        }
    }
    private float submitTrigger = 0;
    private Vector2 spawnVector = Vector2.zero;
    #endregion

    public GameObject pauseCanvas { get; private set; }
    private bool isPause;
    private float pauseTrigger = 0;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        var players = FindObjectsOfType<Player>();
        player = players.FirstOrDefault(p => p.playerIndex == playerInput.playerIndex);
        player.controller = this;
        rb = player.GetComponent<Rigidbody2D>();
        highlightColor = player.playerColor;

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
        walls = new List<Tilemap>();
        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Tilemap tilemap = wall.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                walls.Add(tilemap);
            }
        }

        if (player.playerIndex == 0)
            selectedCell = new Vector3Int(-1, -1, 0);
        else
            selectedCell = new Vector3Int(-0, -1, 0);

        pauseCanvas = GameObject.Find("Canvas").transform.Find("Pause").gameObject;
        isPause = false;
    }

    void Start()
    {
        gamepad = Gamepad.current;
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

    #region Respawn

    private void OnRespawn(InputValue value)
    {
        spawnVector = value.Get<Vector2>();
    }

    public void PlayerRespawn()
    {
        if (!player.isChoosing) return;

        Vector3Int tmp = new Vector3Int(Mathf.RoundToInt(spawnVector[0]), Mathf.RoundToInt(spawnVector[1]), 0);
        HighlightCell(selectedCell);
        if (tmp != Vector3Int.zero)
        {
            for (int i = 1; i <= 6; i++)
            {
                Vector3Int nextCell = selectedCell + i * tmp;
                if (!IsValidCell(nextCell))
                    continue;
                else
                {
                    selectedCell = nextCell;
                    HighlightCell(selectedCell);
                    break;
                }
            }

        }
    }

    public void SpawnPlayer(Vector3 position)
    {
        if (player.playerIndex == 0)
            selectedCell = new Vector3Int(-1, -1, 0);
        else
            selectedCell = new Vector3Int(-0, -1, 0);
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(0);
        player.transform.position = position;
        player.ShowPlayer();
        player.stateMachine.ChangeState(player.idleState);
    }
    private void OnSubmit(InputValue value)
    {
        submitTrigger = value.Get<float>();
    }

    public void PlayerSubmit()
    {
        if (isSubmit)
        {
            if (IsValidCell(selectedCell))
            {
                Vector3 respawnWorldPos = tilemap.GetCellCenterWorld(selectedCell);
                SpawnPlayer(respawnWorldPos);
                InitCell();
            }
        }
    }

    public void InitCell()
    {
        player.isChoosing = false;
        if (lastHighlightedCell.HasValue)
        {
            tilemap.SetTileFlags(lastHighlightedCell.Value, TileFlags.None);
            tilemap.SetColor(lastHighlightedCell.Value, originalColor);
            lastHighlightedCell = null;
        }
    }

    private bool IsValidCell(Vector3Int cellPos)
    {
        TileBase tile = tilemap.GetTile(cellPos);
        bool flag = true;
        foreach (var wall in walls)
        {
            TileBase walltile = wall.GetTile(cellPos);
            if (walltile != null)
            {
                flag = false;
                break;
            }
        }
        
        return tile != null && flag;
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
            player.transform.position = tilemap.GetCellCenterWorld(selectedCell);
        }
    }

    #endregion

    #region Pause
    private void OnPause(InputValue value)
    {
        pauseTrigger = value.Get<float>();
        if (pauseTrigger > 0)
        {
            isPause = !isPause;
            if (isPause)
            {
                Time.timeScale = 0f;
                pauseCanvas.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f;
                pauseCanvas.SetActive(false);
            }
        }
    }

    #endregion

}