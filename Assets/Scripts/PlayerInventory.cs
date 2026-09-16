using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private Transform handPoint;
    [SerializeField] private MetalDetector metalDetector;
    [SerializeField] private int maxSlots = 3; // Limite máximo do inventário

    [System.Serializable]
    public class InventorySlot
    {
        public InteractableItem templateItem;
        public List<InteractableItem> stackedItems = new List<InteractableItem>();

        public int Weight => templateItem.requiresTwoHands ? 2 : 1; 
        public bool IsFull => !templateItem.isStackable || stackedItems.Count >= templateItem.maxStack;
    }

    private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private InventorySlot currentEquippedSlot = null;
    private int currentSlotIndex = -1;

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
        if (inputActions.Player.Drop.WasPressedThisFrame() && currentEquippedSlot != null && currentEquippedSlot.stackedItems.Count > 0)
        {
            string nomeDoItem = currentEquippedSlot.templateItem.itemName;
            DropCurrentItem();
            Debug.Log("Item dropped: " + nomeDoItem);
        }

        float scrollValue = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;
        if (scrollValue > 0f)
        {
            SwitchSlot(1);
        }
        else if (scrollValue < 0f)
        {
            SwitchSlot(-1);
        }
    }

    private int CalculateUsedSlots()
    {
        int total = 0;
        foreach (var slot in inventorySlots)
        {
            total += slot.Weight;
        }
        return total;
    }

    public void PickupItem(InteractableItem newItem)
    {
        // 1. Tenta agrupar
        if (newItem.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.templateItem.itemName == newItem.itemName && !slot.IsFull)
                {
                    slot.stackedItems.Add(newItem);
                    newItem.gameObject.SetActive(false); 
                    newItem.transform.SetParent(transform); 
                    Debug.Log($"Agrupado! Agora você tem {slot.stackedItems.Count} {newItem.itemName}s");
                    UpdateCurseState();
                    return; 
                }
            }
        }

        // 2. Verifica espaço
        int currentUsedSlots = CalculateUsedSlots();
        int itemCost = newItem.requiresTwoHands ? 2 : 1;

        if (currentUsedSlots + itemCost > maxSlots)
        {
            Debug.LogWarning("Inventário cheio ou item pesado demais para o espaço restante!");
            return; 
        }

        // 3. Cria novo slot
        InventorySlot newSlot = new InventorySlot { templateItem = newItem };
        newSlot.stackedItems.Add(newItem);
        inventorySlots.Add(newSlot);

        UpdateCurseState();

        if (currentEquippedSlot == null)
        {
            currentSlotIndex = inventorySlots.Count - 1;
            EquipSlot(newSlot);
        }
        else
        {
            newItem.gameObject.SetActive(false);
            newItem.transform.SetParent(transform); 
        }
    }

    private void EquipSlot(InventorySlot slot)
    {
        currentEquippedSlot = slot;
        
        // Pega o item físico do topo da pilha
        InteractableItem topItem = slot.stackedItems[slot.stackedItems.Count - 1];
        
        topItem.gameObject.SetActive(true);
        topItem.OnPickup(handPoint);

        // if (topItem.requiresTwoHands && metalDetector != null)
        // {
        //     metalDetector.ForceHolster();
        // }

        if (playerMovement != null)
        {
            playerMovement.SetHeavyLoad(topItem.requiresTwoHands);
        }
    }

    private void DropCurrentItem()
    {
        if (currentEquippedSlot == null || currentEquippedSlot.stackedItems.Count == 0) return;

        // Pega o item do topo e remove da pilha
        InteractableItem itemToDrop = currentEquippedSlot.stackedItems[currentEquippedSlot.stackedItems.Count - 1];
        currentEquippedSlot.stackedItems.Remove(itemToDrop);
        itemToDrop.OnDrop();

        // Se o slot ficou vazio após dropar
        if (currentEquippedSlot.stackedItems.Count == 0)
        {
            inventorySlots.Remove(currentEquippedSlot);
            
            if (inventorySlots.Count > 0)
            {
                currentSlotIndex = Mathf.Clamp(currentSlotIndex - 1, 0, inventorySlots.Count - 1);
                EquipSlot(inventorySlots[currentSlotIndex]);
            }
            else
            {
                currentEquippedSlot = null;
                currentSlotIndex = -1;
                if (playerMovement != null) playerMovement.SetHeavyLoad(false);
            }
        }
        else
        {
            // Re-equipa o próximo item da pilha (ex: se soltou a 3ª moeda, equipa a 2ª)
            EquipSlot(currentEquippedSlot);
        }

        UpdateCurseState();
    }

    private void SwitchSlot(int direction)
    {
        if (inventorySlots.Count <= 1) return; 

        if (currentEquippedSlot != null && currentEquippedSlot.stackedItems.Count > 0)
        {
            // Desativa o item atual antes de trocar
            currentEquippedSlot.stackedItems[currentEquippedSlot.stackedItems.Count - 1].gameObject.SetActive(false);
        }

        currentSlotIndex += direction; 

        if (currentSlotIndex >= inventorySlots.Count) 
        {
            currentSlotIndex = 0; 
        }
        else if (currentSlotIndex < 0)
        {
            currentSlotIndex = inventorySlots.Count - 1; 
        }
        
        EquipSlot(inventorySlots[currentSlotIndex]); 
    }

    public bool isHoldingTwoHandedItem()
    {
        return currentEquippedSlot != null && currentEquippedSlot.templateItem.requiresTwoHands;
    }

    public InteractableItem GetCurrentHeldItem()
    {
        if (currentEquippedSlot != null && currentEquippedSlot.stackedItems.Count > 0)
        {
            return currentEquippedSlot.stackedItems[currentEquippedSlot.stackedItems.Count - 1];
        }
        return null;
    }

    private void UpdateCurseState()
    {
        if (playerMovement == null) return;
        
        bool hasCurse = false;
        bool hasPull = false;
        float maxPullForce = 0f;

        // Vasculha todos os slots e os itens dentro deles
        foreach (var slot in inventorySlots)
        {
            foreach (var item in slot.stackedItems)
            {
                if (item.isCursedItem) hasCurse = true;

                if (item.isPullingItem)
                {
                    hasPull = true;
                    if (item.pullForce > maxPullForce)
                    {
                        maxPullForce = item.pullForce;
                    }
                }
            }
        }

        playerMovement.SetCursedState(hasCurse);
        playerMovement.SetPullState(hasPull, maxPullForce);
    }
}