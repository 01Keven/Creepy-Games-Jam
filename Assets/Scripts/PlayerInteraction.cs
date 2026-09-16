using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private PlayerInventory inventory;


    private InputSystem_Actions inputActions;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Update()
    {
        if (inputActions.Player.OpenPaper.WasPressedThisFrame())
        {
            // verifica se o item atual é bilhete
            NoteItem noteInHand = inventory.GetCurrentHeldItem() as NoteItem;
                // se for bilhete, abre a tela e ignora o resto
            if (noteInHand != null)
            {
                noteInHand.ToggleReading();
                return;
            }
        }

        if (playerMovement != null && !playerMovement.canMove) return;
        
        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            TryInteract();
        }
                
        
    }

    private void TryInteract()
    {
        // Cria um raio a partir da posição da câmera na direção que ela está olhando
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Verifica se o raio colidiu com algum objeto dentro do alcance de interação
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            IInteractable hitItem = hit.collider.GetComponent<IInteractable>(); // Tenta obter o componente IInteractable do objeto atingido pelo raio
            if (hitItem != null)
            {
                hitItem.Interact(transform.root.gameObject); // Chama o método Interact do item para realizar a interação
            }
        }
    }
    

}