using Cinemachine;
using UnityEngine;


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
    public PlayerDeadState deadState;
    #endregion

    public Animator animator {  get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }
    public PlayerController controller { get; private set; }

    #region Arrow
    public Arrow arrow;
    #endregion

    #region Combine
    [Header("Need Combine")]
    public Transform spriteTrans;
    public GameObject shadowRight;
    public GameObject shadowLeft;
    public GameObject blood;
    public ParticleSystem dustEffect;
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
        deadState = new PlayerDeadState(stateMachine, this, "isDead");
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
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

    public void GenerateShadow()
    {
        if (controller.rb.velocity.x * spriteTrans.localScale.x < 0 )
            Instantiate(shadowLeft, transform.position, Quaternion.identity).transform.localScale = spriteTrans.localScale;
        else if (controller.rb.velocity.x * spriteTrans.localScale.x > 0)
            Instantiate(shadowRight, transform.position, Quaternion.identity).transform.localScale = spriteTrans.localScale;
    }

    public void GenerateBlood()
    {
        GameObject ps = Instantiate(blood, transform.position, Quaternion.identity);

        Vector3 impulseDir = Vector3.one;
        impulseSource.m_DefaultVelocity = impulseDir * 2f;
        impulseSource.GenerateImpulse();

        Destroy(ps, 5f);
    }
}
