using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class Arrow : MonoBehaviour, IElement
{

    #region stateMachine
    public ArrowStateMachine stateMachine;
    public ArrowFallState fallState;
    public ArrowAimState aimState;
    public ArrowFireState fireState;
    public ArrowStopState stopState;
    public ArrowBrokenState brokenState;
    #endregion

    public Rigidbody2D rb;
    public Collider2D col;
    public TrailRenderer trailRenderer { get; private set; }
    public Light2D arrowLight;
    private float fixedDeltaTime;
    private Coroutine timeFreezeCoroutine;
    public Vector3 generatePos;

    #region state
    public Player player;
    public Player getArrowPlayer;
    private Player hit;
    private Player hitPlayer;
    public Vector2 aimDirection;
    public float lenToPlayer { get; private set; }
    public float fireForce { get; private set; }
    public float lerpAmount { get; private set; }
    public float flyTime { get; private set; }
    public bool isFast
    {
        get
        {
            return _isFast;
        }
        set
        {
            if (value)
            {
                gameObject.layer = LayerMask.NameToLayer("ArrowFast");
                hitWallCol.layer = LayerMask.NameToLayer("ArrowFast");
                lightParticle.Play();
                impulseSource.GenerateImpulse();
                GenerateLightning();
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Arrow");
                hitWallCol.layer = LayerMask.NameToLayer("ArrowWall");
            }
            _isFast = value;
        }
    }
    private bool _isFast;
    public ParticleSystem lightParticle { get; private set; }
    public ArrowGenerator arrowGenerator { get; private set; }
    #endregion

    [Header("Combine")]
    #region GenerateFall
    public Sprite onfloor;
    public Sprite normal;
    public GameObject shadow;
    #endregion
    public GameObject hitWallCol;
    public GameObject hitRing;
    public GameObject lightning;
    public Volume lightningVol { get; private set; }
    public WaveGenerator waveGenerator { get; private set; }
    public AudioManager audioManager { get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }
    public bool IsElementalEffectTriggered { get; set; } = false;

    public ParticleSystem fireParticle;

    protected void Awake()
    {
        player = GetComponentInParent<Player>();

        stateMachine = new ArrowStateMachine();
        fallState = new ArrowFallState(stateMachine, this, "isFall");
        aimState = new ArrowAimState(stateMachine, this, "isAim");
        fireState = new ArrowFireState(stateMachine, this, "isFire");
        stopState = new ArrowStopState(stateMachine, this, "isStop");
        brokenState = new ArrowBrokenState(stateMachine, this, "isBroken");
    }

    protected void Start()
    {
        lenToPlayer = 1.5f;
        fireForce = 60f;
        lerpAmount = 0f;
        flyTime = 0.5f;
        isFast = false;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        arrowLight = GetComponentInChildren<Light2D>();
        lightParticle = GetComponentInChildren<ParticleSystem>();
        arrowGenerator = FindObjectOfType<ArrowGenerator>();

        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioManager = FindObjectOfType<AudioManager>();
        waveGenerator = FindObjectOfType<WaveGenerator>();
        lightningVol = GameObject.FindGameObjectWithTag("GlobalVol").GetComponent<Volume>();

        generatePos = transform.position + Vector3.down * 20f;

        stateMachine.Initialize(fallState);
    }

    protected void Update()
    {
        stateMachine.currentState.Update();
    }

    protected void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    protected virtual void HandleCollision(Collision2D collision)
    {
        hit = collision.gameObject.GetComponent<Player>();
        Arrow arrow = collision.gameObject.GetComponent<Arrow>();

        // player dead
        if (hit != null && !hit.isInvincible)
        {
            hitPlayer = hit;
            hitPlayer.stateMachine.ChangeState(hitPlayer.killState);
            if (lerpAmount > 0.5f)
                SplitParts(hitPlayer);
            hit = null;

            audioManager.PlaySfx(audioManager.hitPlayer);
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.SetRenderer(1);

            // hit player move back
            Vector3 pos = collision.contacts[0].point;
            Vector2 dir = (hitPlayer.transform.position - pos).normalized;
            float force = 50f + lerpAmount * 70f;
            hitPlayer.controller.rb.AddForce(dir * force, ForceMode2D.Impulse);

            if (timeFreezeCoroutine == null)
                timeFreezeCoroutine = StartCoroutine(TimeFreeze(2f, pos));
        }

        // Ifragile
        IFragile fragile = collision.gameObject.GetComponent<IFragile>();
        if (fragile != null)
        {
            audioManager.PlaySfx(audioManager.wallBroken);
            impulseSource.GenerateImpulse();
            Vector3 hitPos = collision.GetContact(0).point;

            float offsetX = 0.25f;
            float offsetY = 0.25f;

            Vector3[] poses = new Vector3[8];
            poses[0] = new Vector3(hitPos.x + offsetX, hitPos.y, 0f);
            poses[1] = new Vector3(hitPos.x - offsetX, hitPos.y, 0f);
            poses[2] = new Vector3(hitPos.x, hitPos.y + offsetY, 0f);
            poses[3] = new Vector3(hitPos.x, hitPos.y - offsetY, 0f);
            poses[4] = new Vector3(hitPos.x + offsetX, hitPos.y + offsetY, 0f);
            poses[5] = new Vector3(hitPos.x + offsetX, hitPos.y - offsetY, 0f);
            poses[6] = new Vector3(hitPos.x - offsetX, hitPos.y + offsetY, 0f);
            poses[7] = new Vector3(hitPos.x - offsetX, hitPos.y - offsetY, 0f);

            foreach (Vector3 pos in poses)
            {
                if (fragile.Destroy(pos))
                    break;
            }
        }
    }

    protected IEnumerator TimeFreeze(float t, Vector3 pos)
    {
        waveGenerator.transform.position = pos;
        waveGenerator.CallShockWave();

        fixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;

        yield return new WaitForSecondsRealtime(t);

        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime;

        //audioManager.SfxAudio.enabled = false;
        impulseSource.m_DefaultVelocity = Vector3.one * 2f;
        impulseSource.GenerateImpulse();
        timeFreezeCoroutine = null;

        //var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        //cameraData.SetRenderer(0);
    }

    public void ChargeUp(float t)
    {
        if (t > 1f)
        {
            t = 1f;
            if (!isFast)
                isFast = true;
        }
        lerpAmount = Mathf.Pow(t, 2);

        lenToPlayer = 1.5f - lerpAmount / 2;
        fireForce = 60f + lerpAmount * 40f;
        arrowLight.intensity = lerpAmount * 0.5f;
        flyTime = 0.5f + lerpAmount * 0.3f;
        player.controller.recoil = 3f + lerpAmount * 18f;

        player.controller.SetGamepadMotor(lerpAmount / 2f);
    }

    public void InitArrow()
    {
        DOTween.To(() => lenToPlayer, x => lenToPlayer = x, 1.5f, .5f);
        DOTween.To(() => arrowLight.intensity, x => arrowLight.intensity = x, 0f, .5f);
        fireForce = 60f;
        flyTime = 0.5f;
        isFast = false;
        // player.controller.recoil recovery is in PlayerFireState.cs
    }

    public void GenerateLightning()
    {
        audioManager.PlaySfx(audioManager.lightning);

        GameObject lightningObj = Instantiate(lightning, transform.position, Quaternion.identity);
        lightningObj.GetComponent<SpriteRenderer>().DOFade(0, .3f);
        Destroy(lightningObj, 0.5f);

        Bloom bloom;
        lightningVol.profile.TryGet<Bloom>(out bloom);

        float scale = 0.9f;
        DOTween.To(
            () => scale,
            (float newScale) =>
            {
                scale = newScale;
                bloom.threshold.SetValue(new FloatParameter(scale));
            },
            1.2f,
            .5f  // duration
        );
    }

    public void GenerateShadow(Vector3 pos)
    {
        GameObject shadowObj = Instantiate(shadow, pos, Quaternion.identity);
        shadowObj.transform.localScale = Vector3.zero;
        shadowObj.transform.DOScaleX(1f, 0.9f).SetEase(Ease.InQuad);
        shadowObj.transform.DOScaleY(0.6f, 0.9f).SetEase(Ease.InQuad);
        Destroy(shadowObj, 1f);
    }

    private void SplitParts(Player player)
    {
        Color color = Color.white;
        color.a = 0;
        player.spriteTrans.GetComponent<SpriteRenderer>().color = color;
        player.playerParts.DeadTrigger();
        player.shadow.enabled = false;
    }

    public virtual bool ElementalEffectTriggered()
    {
        bool tmp = IsElementalEffectTriggered;
        if (!IsElementalEffectTriggered)
            IsElementalEffectTriggered = true;
        return tmp;
    }
}
