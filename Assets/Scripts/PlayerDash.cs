using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Stats (upgradable at runtime)")]
    public float dashDistance = 6f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    public float dashDamage = 10f;
    public float dashKnockbackForce = 12f;

    [Header("Hit Detection")]
    public float hitCheckRadius = 0.6f;

    private CharacterController controller;

    private float cooldownTimer;
    private bool isDashing;
    private float dashTimer;
    private float dashSpeed;
    private Vector3 dashDirection;
    private readonly HashSet<Collider> hitThisDash = new HashSet<Collider>();

    public bool IsDashing => isDashing;
    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isDashing && cooldownTimer <= 0f)
        {
            bool dashPressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            if (dashPressed)
            {
                StartDash();
            }
        }

        if (isDashing)
        {
            TickDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = 0f;
        dashSpeed = dashDistance / Mathf.Max(dashDuration, 0.0001f);
        dashDirection = transform.forward;
        hitThisDash.Clear();
        cooldownTimer = dashCooldown;
    }

    private void TickDash()
    {
        dashTimer += Time.deltaTime;

        Vector3 prevPos = transform.position;
        controller.Move(dashDirection * dashSpeed * Time.deltaTime);
        Vector3 newPos = transform.position;

        CheckHitsAlongSegment(prevPos, newPos);

        if (dashTimer >= dashDuration)
        {
            isDashing = false;
        }
    }

    private void CheckHitsAlongSegment(Vector3 prevPos, Vector3 newPos)
    {
        Vector3 a = prevPos + controller.center;
        Vector3 b = newPos + controller.center;

        Collider[] hits = Physics.OverlapCapsule(a, b, hitCheckRadius);
        foreach (var col in hits)
        {
            if (hitThisDash.Contains(col)) continue;
            if (!col.CompareTag("Enemy")) continue;

            hitThisDash.Add(col);
            var enemy = col.GetComponent<EnemyPlaceholder>();
            if (enemy != null)
            {
                enemy.ApplyKnockback(dashDirection * dashKnockbackForce, dashDamage);
            }
        }
    }

    public void IncreaseDashDistance(float amount) => dashDistance += amount;
    public void IncreaseDashDamage(float amount) => dashDamage += amount;
    public void IncreaseKnockbackForce(float amount) => dashKnockbackForce += amount;
    public void ReduceCooldown(float amount) => dashCooldown = Mathf.Max(0.1f, dashCooldown - amount);
}
