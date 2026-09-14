using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float walkSpeed = 5f;
    
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

    private float verticalVelocity;
    private float originalSpeed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        originalSpeed = walkSpeed;

    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


    void Update()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();

        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
        movement *= walkSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }

    public void SetHeavyLoad(bool isHeavy)
    {
        walkSpeed = isHeavy ? originalSpeed * 0.5f : originalSpeed;
    }
}
