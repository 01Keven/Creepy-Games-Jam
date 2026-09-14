using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform handPoint;
    [SerializeField] private MetalDetector metalDetector;


    private List<InteractableItem> inventoryList = new List<InteractableItem>();
    private InteractableItem currentHeldItem = null;

    private int currentItemIndex = -1; // Índice do item atualmente equipado

    private InputSystem_Actions inputActions;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        if (inputActions.Player.Drop.WasPressedThisFrame() && currentHeldItem != null)
        {
            string nomeDoItem = currentHeldItem.itemName;
            DropCurrentItem();
            Debug.Log("Item dropped" + (nomeDoItem != null ? ": " + nomeDoItem : ""));
        }

        float scrollValue = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;
        if (scrollValue > 0f)
        {
            SwitchItem(1);
        }
        else if (scrollValue < 0f)
        {
            SwitchItem(-1);
        }
    }

    public void PickupItem(InteractableItem item)
    {
        inventoryList.Add(item);

        if (currentHeldItem == null)
        {
            currentItemIndex = 0;
            EquipItem(item);
        } else
        {
            item.gameObject.SetActive(false); // Desativa o item se já houver um item equipado
        }
    }


    private void EquipItem(InteractableItem item)
    {
        // if (item.requiresTwoHands && metalDetector != null)
        // {
        //     metalDetector.ForceHolster();
        // }
        // {
            
        // }

        currentHeldItem = item;
        item.gameObject.SetActive(true);
        item.OnPickup(handPoint);

        if (item.requiresTwoHands && metalDetector != null) // força o detector a ser guardado se puxar um item do invetario
        {
            metalDetector.ForceHolster();
        }

        if (playerMovement != null)
        {
            playerMovement.SetHeavyLoad(item.requiresTwoHands);
        }
    }

    private void DropCurrentItem()
    {
        inventoryList.Remove(currentHeldItem);
        currentHeldItem.OnDrop();

        if (inventoryList.Count > 0)
        {
            if (currentItemIndex >= inventoryList.Count)
            {
                currentItemIndex = inventoryList.Count - 1; // Volta para o primeiro item se o índice atual estiver fora do alcance
            }
            EquipItem(inventoryList[currentItemIndex]);
        } else
        {
            currentHeldItem = null;
            currentItemIndex = -1; // Nenhum item equipado

            // restaura a velocidade se ficar de mão vazia
            if (playerMovement != null)
            {
                playerMovement.SetHeavyLoad(false);
            }
        }

    }

    public bool isHoldingTwoHandedItem()
    {
        return currentHeldItem != null && currentHeldItem.requiresTwoHands;
    }

    private void SwitchItem(int direction)
    {
        if (inventoryList.Count <= 1) return; // Se houver apenas um item ou nenhum, não faz sentido trocar


        if (currentHeldItem != null)
        {
            currentHeldItem.gameObject.SetActive(false);
        }

        currentItemIndex += direction; // Atualiza o índice do item atual com base na direção (1 para próximo, -1 para anterior)

        if (currentItemIndex >= inventoryList.Count) // Se o índice for maior que o tamanho da lista, volta para o primeiro item
        {
            currentItemIndex = 0; // Volta para o primeiro item
        }
        else if (currentItemIndex < 0)
        {
            currentItemIndex = inventoryList.Count - 1; // Vai para o último item
        }
        EquipItem(inventoryList[currentItemIndex]); // Equipa o item selecionado
    }
}
