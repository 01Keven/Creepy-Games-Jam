using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform handPoint;

    private List<InteractableItem> inventoryList = new List<InteractableItem>();

    private InteractableItem currentHeldItem = null;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
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
    }

    public void PickupItem(InteractableItem item)
    {
        inventoryList.Add(item);

        if (currentHeldItem == null)
        {
            EquipItem(item);
        } else
        {
            item.gameObject.SetActive(false); // Desativa o item se já houver um item equipado
        }
    }


    private void EquipItem(InteractableItem item)
    {
        currentHeldItem = item;
        item.gameObject.SetActive(true);
        item.OnPickup(handPoint);
    }

    private void DropCurrentItem()
    {
        inventoryList.Remove(currentHeldItem);
        currentHeldItem.OnDrop();
        currentHeldItem = null;
    }
}
