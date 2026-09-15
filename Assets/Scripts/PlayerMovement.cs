using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float walkSpeed = 5f;
    
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

    private float verticalVelocity;
    private float originalSpeed;

    private bool isCursed = false;
    private Camera playerCamera;
    private float originalFov;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        originalSpeed = walkSpeed;

        playerCamera = Camera.main;
        if (playerCamera != null)
        {
            originalFov = playerCamera.fieldOfView;
        }

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

        if (isCursed)
        {
            inputVector *= -1f;
        }

        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
        movement *= walkSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);

        HandleDizziness();
    }

    public void SetHeavyLoad(bool isHeavy)
    {
        walkSpeed = isHeavy ? originalSpeed * 0.5f : originalSpeed;
    }

    public void SetCursedState(bool state)
    {
        isCursed =  state;
    }

    private void HandleDizziness()
    {
        if (playerCamera == null) return;
        
        if (isCursed)
        {
            // usa o tempo do jogo para criar uma onda que sobe e desce
            playerCamera.fieldOfView = originalFov + Mathf.Sin(Time.time * 2f) * 5f;
        }
        else if (playerCamera.fieldOfView != originalFov)
        {
            // quando passsa, o FOV volta suavemente
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, originalFov, Time.deltaTime * 5f);
        }
    }
}
