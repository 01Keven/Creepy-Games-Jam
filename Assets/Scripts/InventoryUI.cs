using UnityEngine;
using UnityEngine.UI;
using TMPro; // Usado para o texto do TextMeshPro
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public class UISlot
    {
        public GameObject slotRoot; // O objeto principal do Slot (Slot_0, Slot_1...)
        public Image iconImage; // A imagem do ícone do item
        public TextMeshProUGUI countText; // O texto de quantidade
        public RectTransform rectTransform; // Usado para aumentar/diminuir o tamanho
    }

    [Header("Configurações da UI")]
    public UISlot[] uiSlots; // Array onde você vai arrastar os 3 slots

    // Método que será chamado pelo PlayerInventory toda vez que algo mudar
    public void RefreshUI(List<PlayerInventory.InventorySlot> inventorySlots, int currentIndex)
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            // Se houver um item físico ocupando esse número de slot na lista
            if (i < inventorySlots.Count)
            {
                uiSlots[i].slotRoot.SetActive(true);
                
                // Pega a imagem que você configurou no Inspector do item
                uiSlots[i].iconImage.sprite = inventorySlots[i].templateItem.itemIcon;

                // Atualiza a quantidade (só mostra o texto se tiver mais de 1)
                int count = inventorySlots[i].stackedItems.Count;
                bool isStackable = inventorySlots[i].templateItem.isStackable;
                uiSlots[i].countText.text = isStackable ? count.ToString() : "";

                // A MÁGICA VISUAL: Aumenta o slot atual e diminui os que estão no bolso
                if (i == currentIndex)
                {
                    uiSlots[i].rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f); // 20% maior
                    uiSlots[i].iconImage.color = Color.white; // Cor normal
                }
                else
                {
                    uiSlots[i].rectTransform.localScale = new Vector3(0.8f, 0.8f, 1f); // 20% menor
                    uiSlots[i].iconImage.color = new Color(0.7f, 0.7f, 0.7f, 1f); // Levemente escurecido
                }
            }
            else
            {
                // Se não tem item nesse espaço da lista, desliga o visual do slot
                uiSlots[i].slotRoot.SetActive(false);
            }
        }
    }
}