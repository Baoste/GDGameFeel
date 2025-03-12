using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Arrow : MonoBehaviour
{

    #region State
    public ArrowStateMachine stateMachine;
    public ArrowAimState aimState;
    public ArrowFireState fireState;
    public ArrowStopState stopState;
    #endregion

    public Rigidbody2D rb;
    public Collider2D col;

    public Player player;
    public Player getArrowPlayer;
    public Vector2 aimDirection;

    private void Awake()
    {
        player = GetComponentInParent<Player>();

        stateMachine = new ArrowStateMachine();
        aimState = new ArrowAimState(stateMachine, this, "isAim");
        fireState = new ArrowFireState(stateMachine, this, "isFire");
        stopState = new ArrowStopState(stateMachine, this, "isStop");

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        stateMachine.Initialize(aimState);
    }

    void Update()
    {
        stateMachine.currentState.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            getArrowPlayer = collision.gameObject.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
                getArrowPlayer = null;
        }
    }

}
