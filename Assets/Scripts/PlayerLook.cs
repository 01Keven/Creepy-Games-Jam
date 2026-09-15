using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform playerBody;

    private float xRotation = 0f;
    private InputSystem_Actions inputActions;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        playerMovement = GetComponentInParent<PlayerMovement>();

    }

    private void OnEnable()
    {
        inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    void Update()
    {
        if (playerMovement != null && !playerMovement.canMove) return;

        Vector2 mouseInput = UnityEngine.InputSystem.Mouse.current.delta.ReadValue();

        float mouseX = mouseInput.x * mouseSensitivity;
        float mouseY = mouseInput.y * mouseSensitivity;
        
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70f, 70f);

        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, 0f);
        playerBody.Rotate(Vector3.up * mouseX, Space.Self);
    }
}