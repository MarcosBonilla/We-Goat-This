using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("Posture Placeholders (assign child visuals)")]
    [Tooltip("Upright placeholder shown while idle/walking.")]
    public GameObject twoLegsVisual;
    [Tooltip("Low, elongated placeholder shown while running.")]
    public GameObject fourLegsVisual;

    private CharacterController controller;
    private PlayerDash dash;
    private Vector3 verticalVelocity;
    private bool isRunning;

    public bool IsRunning => isRunning;
    public float CurrentSpeed => isRunning ? runSpeed : walkSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        dash = GetComponent<PlayerDash>();
        SetPosture(false);
    }

    private void Update()
    {
        UpdateRunState();

        if (dash != null && dash.IsDashing)
        {
            return;
        }

        UpdateMovementAndJump();
    }

    private void UpdateRunState()
    {
        bool runHeld = Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
        if (runHeld != isRunning)
        {
            isRunning = runHeld;
            SetPosture(isRunning);
        }
    }

    private void SetPosture(bool fourLegs)
    {
        if (twoLegsVisual != null) twoLegsVisual.SetActive(!fourLegs);
        if (fourLegsVisual != null) fourLegsVisual.SetActive(fourLegs);
    }

    private void UpdateMovementAndJump()
    {
        var kb = Keyboard.current;
        float h = 0f;
        float v = 0f;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
        }

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDir = right * h + forward * v;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        if (controller.isGrounded)
        {
            if (verticalVelocity.y < 0f) verticalVelocity.y = -2f;

            if (kb != null && kb.spaceKey.wasPressedThisFrame)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 motion = moveDir * CurrentSpeed + verticalVelocity;
        controller.Move(motion * Time.deltaTime);
    }
}
