using Cinemachine;
using System.Collections;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Player : MonoBehaviour
{
    #region StateMachine
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
    public PlayerFallState fallState;
    #endregion

    public Animator animator {  get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }
    public PlayerController controller { get; private set; }
    public AudioManager audioManager { get; private set; }
    private float fixedDeltaTime;
    private TwistGenerator twistGenerator;

    #region Arrow
    public Arrow arrow;
    #endregion

    #region State
    public bool canAim;
    #endregion

    #region Combine
    [Header("Need Combine")]
    public GameObject endMenu;
    public Transform spriteTrans;
    public GameObject shadowRight;
    public GameObject shadowLeft;
    public GameObject blood;
    public ParticleSystem dustEffect;
    public ParticleSystemForceField forceField;
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
        fallState = new PlayerFallState(stateMachine, this, "isFall");
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioManager = FindObjectOfType<AudioManager>();
        twistGenerator = FindObjectOfType<TwistGenerator>();

        canAim = true;
        forceField.enabled = false;

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
        GameObject shadow = null;
        if (controller.rb.velocity.x * spriteTrans.localScale.x < 0 )
            shadow = Instantiate(shadowLeft, transform.position, Quaternion.identity);
        else
            shadow = Instantiate(shadowRight, transform.position, Quaternion.identity);
        shadow.transform.localScale = spriteTrans.localScale;
        SpriteRenderer shadowSp = shadow.GetComponent<SpriteRenderer>();
        shadowSp.sprite = spriteTrans.GetComponent<SpriteRenderer>().sprite;
        float alpha = shadowSp.color.a;
        Color color = spriteTrans.GetComponent<SpriteRenderer>().color;
        color.a = alpha;
        shadowSp.color = color;
    }

    public void GenerateBlood()
    {       
        Instantiate(blood, transform.position, Quaternion.identity);
    }

    public void DashFreeze()
    {
        StartCoroutine(TimeFreeze(0.05f));
    }
    
    private IEnumerator TimeFreeze(float t)
    {
        fixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;

        yield return new WaitForSecondsRealtime(t);

        twistGenerator.transform.position = transform.position;
        twistGenerator.CallShockTwist();

        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime;
    }
}
