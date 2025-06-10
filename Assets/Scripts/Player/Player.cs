using Cinemachine;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;


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
    public PlayerKillState killState;
    public PlayerFallState fallState;
    public PlayerRespawnState respawnState;
    #endregion

    public Animator animator {  get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }
    public PlayerController controller;
    public AudioManager audioManager { get; private set; }
    private float fixedDeltaTime;
    private TwistGenerator twistGenerator;
    public PlayerParts playerParts { get; private set; }
    public PlayerFoot playerFoot { get; private set; }

    public bool isInvincible = false;//无敌时间
    public float invincibleDuration = 3f;

    public bool isChoosing = false;

    public ReactiveProperty<int> deathCount = new(0);
    //public int deathCount = 0;

    #region Arrow
    public Arrow arrow;
    public int arrowCount;
    #endregion

    #region State
    public bool canAim;
    [SerializeField] public int playerIndex;
    public float dashCoolTime;
    #endregion

    #region Combine
    [Header("Need Combine")]
    public Color playerColor;
    public Color enemyColor;
    public GameObject endMenu;
    public GameObject winnerCanvas;
    public Transform spriteTrans;
    public GameObject shadowRight;
    public GameObject shadowLeft;
    public GameObject blood;
    public ParticleSystem dustEffect;
    public ParticleSystemForceField forceField;
    public SpriteRenderer shadow;
    public UIScore enemyScore;
    public ScorePartGenrator partGenrator;
    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(stateMachine, this, "isIdle");
        moveState = new PlayerMoveState(stateMachine, this, "isMove");
        dashState = new PlayerDashState(stateMachine, this, "isDash");
        dashInState = new PlayerDashInState(stateMachine, this, "isDash");
        dashingState = new PlayerDashingState(stateMachine, this, "isDash");
        dashOutState = new PlayerDashOutState(stateMachine, this, "isDash");
        aimState = new PlayerAimState(stateMachine, this, "isAim");
        fireState = new PlayerFireState(stateMachine, this, "isFire");
        killState = new PlayerKillState(stateMachine, this, "isDead");
        fallState = new PlayerFallState(stateMachine, this, "isFall");
        respawnState = new PlayerRespawnState(stateMachine, this, "isRespawn");
    }
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioManager = FindObjectOfType<AudioManager>();
        twistGenerator = FindObjectOfType<TwistGenerator>();
        playerParts = GetComponentInChildren<PlayerParts>();
        playerFoot = GetComponentInChildren<PlayerFoot>();

        canAim = true;
        forceField.enabled = false;
        dashCoolTime = 0f;
        arrowCount = 0;

        stateMachine.Initialize(idleState);

        deathCount.Subscribe((v) =>
        {
            enemyScore.scoreImage.sprite = enemyScore.scoreSprites[v];
        });
    }

    void Update()
    {
        stateMachine.currentState.Update();
        arrow = GetComponentInChildren<Arrow>();
        if (arrow != null)
        {
            arrow.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        arrowCount = GetComponentsInChildren<Arrow>().Length;

        dashCoolTime -= Time.deltaTime;
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
        audioManager.PlaySfx(audioManager.Explosion);
        Instantiate(blood, transform.position, Quaternion.identity);
    }

    public void DashFreeze()
    {
        StartCoroutine(TimeFreeze(0.03f));
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

    public void HidePlayer()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = false;
        // generate small parts
        partGenrator.GenerateScoreParts(transform.position);
    }

    public void ShowPlayer()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = true;
        playerParts.Init();
    }

    public void InvincibilityRoutine(float duration)
    {
        // player.isInvincible = true;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f); // 强制初始不透明
        sr.DOFade(0f, 0.2f)  // 透明
          .SetLoops(15, LoopType.Yoyo) // 无限次闪烁
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
              sr.DOKill();
              sr.DOFade(1f, 0.1f);
              isInvincible = false;
          });

        //while (timer < duration)
        //{
        //    // 简单闪烁效果
        //    sr.color = new Color(1f, 1f, 1f, 0.5f); // 半透明
        //    yield return new WaitForSeconds(0.2f);
        //    sr.color = new Color(1f, 1f, 1f, 1f); // 不透明
        //    yield return new WaitForSeconds(0.2f);

        //    timer += 0.4f;
        //}

        //sr.color = new Color(1f, 1f, 1f, 1f); // 最终恢复正常
        //isInvincible = false;
    }
}
