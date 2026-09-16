using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField] private float walkSpeed = 5f;
    [Header("Efeito de Puxão")]
    public Transform pullTarget; // ponto onde puxa o player
    private bool isBeingPulled = false;
    private float currentPullForce = 0f;
    
    private CharacterController characterController;
    private InputSystem_Actions inputActions;

    private float verticalVelocity;
    private float originalSpeed;

    private bool isCursed = false;
    private Camera playerCamera;
    private float originalFov;

    public GameObject uiNote;

    public bool canMove { get; private set; } = true;

    public void FreezePlayer(bool freeze)
    {
        canMove = !freeze;
    }

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
        Vector2 inputVector = Vector2.zero;

        if (canMove)
        {
            inputVector = inputActions.Player.Move.ReadValue<Vector2>();
            if (isCursed)
            {
                inputVector *= -1f;
            }
        }

        Vector3 movement = transform.right * inputVector.x + transform.forward * inputVector.y;
        movement *= walkSpeed;

        if (isBeingPulled && pullTarget != null) // indenpendente do jogador estar andando ou nao
        {
            Vector3 pullDirection = (pullTarget.position - transform.position).normalized; // descobre a direção
            pullDirection.y = 0; // para o jogador nao sair voando ou atravessar o chão
            movement += pullDirection * currentPullForce; // força do puxao contra ou favor do jogador
        }

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

    public void SetPullState(bool state, float force)
    {
        isBeingPulled = state;
        currentPullForce = force;
    }
}
