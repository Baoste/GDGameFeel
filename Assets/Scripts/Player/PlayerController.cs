using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb {  get; private set; }

    #region Move
    [Header("Move")]
    public float moveSpeed = 9f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float deceleration = 24f;
    [SerializeField] private float velPower = 0.87f;
    [SerializeField] private float frictionAmount = 0.22f;
    private Vector2 inputVec;
    #endregion

    #region Dash
    [Header("Dash")]
    [SerializeField] private float dashMaxSpeed = 800f;
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
            dashTrigger = 0;
            return v;
        }
        private set
        {
            dashTrigger = value ? 1 : 0;
        }
    }
    private float dashTrigger = 0;
    #endregion

    private Vector2 recentInputVec = Vector2.right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool isMoving()
    {
        if (inputVec.magnitude > 0.01f)
            return true;
        return false;
    }

    public void PlayerMove()
    {
        // Move
        Vector2 targetSpeed = moveSpeed * inputVec;
        Vector2 speedDif = targetSpeed - rb.velocity;
        float speedDist = speedDif.sqrMagnitude;
        float accelRate = (targetSpeed.magnitude > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(speedDist * accelRate, velPower);
        rb.AddForce(movement * speedDif.normalized);
        //rb.velocity = targetSpeed;
    }

    public void PlayerIdle()
    {
        // Friction
        Vector2 v = rb.velocity;
        float amount = Mathf.Min(v.sqrMagnitude, frictionAmount);
        if (v.magnitude > 0.2f)
            rb.AddForce(-v * amount, ForceMode2D.Impulse);
        else
            rb.velocity = Vector2.zero;
    }

    public void PlayerDashIn()
    {
        Vector2 direction = rb.velocity.normalized;
        float dashAmount = Mathf.Pow(rb.velocity.sqrMagnitude * dashAcc, velPower);
        rb.AddForce(dashAmount * -direction);
    }

    public void PlayerDashTime()
    {
        Vector2 direction = recentInputVec;
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

    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
        if (inputVec.magnitude > 0.1f)
            recentInputVec = inputVec;
    }

    private void OnDash(InputValue value)
    {
        dashTrigger = value.Get<float>();
    }
}
