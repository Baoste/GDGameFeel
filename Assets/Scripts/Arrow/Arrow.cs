using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;

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
    private Player hit;
    public Player getArrowPlayer;
    public Vector2 aimDirection;

    [Header("Combine")]
    public GameObject hitRing;

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
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // arrow coliider
        Arrow arrow = collision.gameObject.GetComponent<Arrow>();
        if (arrow != null)
        {
            impulseSource.GenerateImpulse();
            ContactPoint2D contact = collision.contacts[0];
            Vector3 pos = contact.point;
            //Instantiate(hitRing, pos, Quaternion.identity);
            Instantiate(hitRing, pos, Quaternion.Euler(new Vector3(0, 0, 10f)));
            StartCoroutine(WaitToStop(.2f));
        }
        // player dead
        hit = collision.gameObject.GetComponent<Player>();
        if (hit != null)
        {
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            // 设置需要使用的 Renderer 索引
            cameraData.SetRenderer(1);
            StartCoroutine(TimeFreeze(1));
        }
    }
    private IEnumerator WaitToStop(float t)
    {
        yield return new WaitForSeconds(t);
        stateMachine.ChangeState(stopState);
    }

    private IEnumerator TimeFreeze(float t)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(t);
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(0);
        hit.stateMachine.ChangeState(hit.deadState);
        Time.timeScale = 1;
    }
}
