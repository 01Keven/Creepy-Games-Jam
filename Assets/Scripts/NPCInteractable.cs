using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [Header("Configurações de Diálogo")]
    public DialogueNode startingNode; // A primeira frase do NPC
    public Camera npcCamera; // A câmera focada no rosto dele

    public void Interact(GameObject interactor)
    {
        PlayerMovement pm = interactor.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            pm.FreezePlayer(true);
        }

        // 1. CAPTURA a câmera enquanto ela ainda está ligada
        Camera playerCam = Camera.main; 

        // 2. Desliga a câmera principal e liga a do NPC
        if (playerCam != null) playerCam.gameObject.SetActive(false);
        npcCamera.gameObject.SetActive(true);

        // 3. Envia a câmera capturada para o Manager
        DialogueManager.Instance.StartDialogue(startingNode, npcCamera, interactor, playerCam);
    }
}