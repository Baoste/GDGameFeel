using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class Arrow : MonoBehaviour
{

    #region stateMachine
    public ArrowStateMachine stateMachine;
    public ArrowAimState aimState;
    public ArrowFireState fireState;
    public ArrowStopState stopState;
    #endregion

    public Rigidbody2D rb;
    public Collider2D col;
    public TrailRenderer trailRenderer { get; private set; }
    private Light2D arrowLight;

    #region state
    public Player player;
    public Player getArrowPlayer;
    private Player hit;
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
                lightParticle.Play();
                impulseSource.GenerateImpulse();
                GenerateLightning();
            }
            else
                gameObject.layer = LayerMask.NameToLayer("Arrow");
            _isFast = value;
        }
    }
    private bool _isFast;
    private ParticleSystem lightParticle;
    #endregion

    [Header("Combine")]
    public GameObject hitRing;
    public WaveGenerator waveGenerator;
    public GameObject lightning;
    public Volume lightningVol;
    public AudioManager audioManager { get; private set; }
    public CinemachineImpulseSource impulseSource { get; private set; }

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
        lenToPlayer = 1.5f;
        fireForce = 30f;
        lerpAmount = 0f;
        flyTime = 0.5f;
        isFast = false;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        arrowLight = GetComponentInChildren<Light2D>();
        lightParticle = GetComponentInChildren<ParticleSystem>();
        audioManager = FindObjectOfType<AudioManager>();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hit = collision.gameObject.GetComponent<Player>();
        Arrow arrow = collision.gameObject.GetComponent<Arrow>();
        // player dead
        if (hit != null && hit.stateMachine.currentState != hit.deadState)
        {
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            // 设置需要使用的 Renderer 索引
            cameraData.SetRenderer(1);
            Vector3 pos = collision.contacts[0].point;
            StartCoroutine(TimeFreeze(1, pos));
        }
        
        // Ifragile
        IFragile fragile = collision.gameObject.GetComponent<IFragile>();
        if (fragile != null)
        {
            audioManager.PlaySfx(audioManager.hitWall);
            impulseSource.GenerateImpulse();
            Vector3 hitPos = collision.GetContact(0).point;

            float offsetX = 0.25f;
            float offsetY = 0.25f;
            Vector3 pos1 = new Vector3(hitPos.x + offsetX, hitPos.y, 0f);
            fragile.Destroy(pos1);
            Vector3 pos2 = new Vector3(hitPos.x - offsetX, hitPos.y, 0f);
            fragile.Destroy(pos2);
            Vector3 pos3 = new Vector3(hitPos.x, hitPos.y + offsetY, 0f);
            fragile.Destroy(pos3);
            Vector3 pos4 = new Vector3(hitPos.x, hitPos.y - offsetY, 0f);
            fragile.Destroy(pos4);
            Vector3 pos5 = new Vector3(hitPos.x + offsetX, hitPos.y + offsetY, 0f);
            fragile.Destroy(pos5);
            Vector3 pos6 = new Vector3(hitPos.x + offsetX, hitPos.y - offsetY, 0f);
            fragile.Destroy(pos6);
            Vector3 pos7 = new Vector3(hitPos.x - offsetX, hitPos.y + offsetY, 0f);
            fragile.Destroy(pos7);
            Vector3 pos8 = new Vector3(hitPos.x - offsetX, hitPos.y - offsetY, 0f);
            fragile.Destroy(pos8);
        }
    }

    private IEnumerator TimeFreeze(float t, Vector3 pos)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(t);

        Time.timeScale = 1;
        waveGenerator.transform.position = pos;
        waveGenerator.CallShockWave();

        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(0);
        hit.stateMachine.ChangeState(hit.deadState);
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

        lenToPlayer = 1.5f - lerpAmount;
        fireForce = 40f + lerpAmount * 60f;
        arrowLight.intensity = lerpAmount * 0.5f;
        flyTime = 0.5f + lerpAmount * 0.3f;
        player.controller.recoil = 3f + lerpAmount * 18f;
    }

    public void InitArrow()
    {
        DOTween.To(() => lenToPlayer, x => lenToPlayer = x, 1.5f, .5f);
        DOTween.To(() => arrowLight.intensity, x => arrowLight.intensity = x, 0f, .5f);
        fireForce = 30f;
        flyTime = 0.5f;
        isFast = false;
        // player.controller.recoil recovery is in PlayerFireState.cs
    }

    private void GenerateLightning()
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

}
