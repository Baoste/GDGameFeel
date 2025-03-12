using Cinemachine;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Player : MonoBehaviour
{
    #region State
    public PlayerStateMachine stateMachine;
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerDashState dashState;
    public PlayerDashInState dashInState;
    public PlayerDashingState dashingState;
    public PlayerDashOutState dashOutState;
    public PlayerAimState aimState;
    public PlayerFireState fireState;
    #endregion

    public Animator animator {  get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }
    public PlayerController controller { get; private set; }

    #region Arrow
    public Arrow arrow;
    #endregion

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(stateMachine, this, "isIdle");
        moveState = new PlayerMoveState(stateMachine, this, "isMove");
        dashState = new PlayerDashState(stateMachine, this, "isDash");
        dashInState = new PlayerDashInState(stateMachine, this, "isDash");
        dashingState = new PlayerDashingState(stateMachine, this, "isDash");
        dashOutState = new PlayerDashOutState(stateMachine, this, "isDash");
        aimState = new PlayerAimState(stateMachine, this, "isAim");
        fireState = new PlayerFireState(stateMachine, this, "isFire");
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        stateMachine.currentState.Update();
        arrow = GetComponentInChildren<Arrow>();
    }

    private void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

}
