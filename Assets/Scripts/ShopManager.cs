using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Referências da UI")]
    public GameObject shopPanel;
    public Transform itemsContainer; // Onde os botões de venda vão ser agrupados
    public GameObject shopSlotPrefab; // O prefab que tem o script ShopSlotUI
    public TextMeshProUGUI playerMoneyText;

    private PlayerInventory currentPlayerInventory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenShop(PlayerInventory inventory)
    {
        currentPlayerInventory = inventory;
        shopPanel.SetActive(true);
        
        // Congela o player e libera o mouse para clicar na loja
        inventory.GetComponent<PlayerMovement>().FreezePlayer(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshShopUI();
    }

    public void RefreshShopUI()
    {
        // Limpa a lista antiga da tela
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        // Atualiza o saldo do jogador
        playerMoneyText.text = "Carteira: $" + currentPlayerInventory.currentMoney.ToString();

        // Vasculha o inventário agrupado
        foreach (var slot in currentPlayerInventory.inventorySlots)
        {
            if (slot.stackedItems.Count > 0)
            {
                InteractableItem topItem = slot.stackedItems[slot.stackedItems.Count - 1];
                
                // Só exibe o item se ele puder ser vendido
                if (topItem.isSellable)
                {
                    CreateShopSlot(topItem, slot.stackedItems.Count);
                }
            }
        }
    }

    private void CreateShopSlot(InteractableItem item, int amount)
    {
        GameObject slotObj = Instantiate(shopSlotPrefab, itemsContainer);
        ShopSlotUI slotUI = slotObj.GetComponent<ShopSlotUI>();
        
        slotUI.itemIcon.sprite = item.itemIcon;
        
        // Se houver itens agrupados, mostra a quantidade no nome (ex: "Moeda Antiga (x3)")
        string amountText = amount > 1 ? $" (x{amount})" : "";
        slotUI.itemNameText.text = item.itemName + amountText;
        
        slotUI.itemPriceText.text = "+ $" + item.itemValor.ToString();

        // Configura o botão para vender o item e imediatamente recarregar a UI
        slotUI.sellButton.onClick.AddListener(() => 
        {
            currentPlayerInventory.SellItem(item);
            RefreshShopUI(); 
        });
    }

    // Chamado por um botão "Fechar/Sair" na interface da loja
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        
        if (currentPlayerInventory != null)
        {
            currentPlayerInventory.GetComponent<PlayerMovement>().FreezePlayer(false);
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}