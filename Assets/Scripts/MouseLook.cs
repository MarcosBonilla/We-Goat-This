using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [Header("Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private float pitch;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool isLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isLocked;
        }

        if (Mouse.current == null || cameraTransform == null || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up, delta.x, Space.World);

        pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }
}
