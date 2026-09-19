using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance; // Padrão Singleton para fácil acesso

    [Header("Referências da UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    
    [Header("Sistema de Escolhas")]
    public GameObject choicesContainer; // O painel que vai agrupar os botões
    public GameObject choiceButtonPrefab; // O prefab do botão de resposta

    [Header("Configurações")]
    public float typingSpeed = 0.05f;

    private Camera activeNPCCamera;
    private Camera storedPlayerCamera;
    private GameObject currentPlayer;
    private Coroutine typingCoroutine;
    private bool isTyping = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(DialogueNode node, Camera npcCam, GameObject player, Camera playerCam)
    {
        activeNPCCamera = npcCam;
        currentPlayer = player;
        storedPlayerCamera = playerCam; // Salva a câmera aqui!
        
        dialoguePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;

        DisplayNode(node);
    }

    public void DisplayNode(DialogueNode node)
    {
        // Limpa botões antigos
        foreach (Transform child in choicesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        nameText.text = node.npcName;
        
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(node));
    }

    private IEnumerator TypeSentence(DialogueNode node)
    {
        isTyping = true;
        dialogueText.text = "";
        
        // Efeito Máquina de Escrever
        foreach (char letter in node.dialogueText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        ShowChoices(node);
    }

    private void ShowChoices(DialogueNode node)
    {
        // Se não tiver escolhas, cria um botão padrão de "Avançar/Sair"
        if (node.choices == null || node.choices.Length == 0)
        {
            CreateButton("Next", null, DialogueAction.Nothing);
            return;
        }

        // Cria os botões baseados nas opções do ScriptableObject
        foreach (DialogueChoice choice in node.choices)
        {
            CreateButton(choice.choiceText, choice.nextNode, choice.action);
        }
    }

    private void CreateButton(string text, DialogueNode nextNode, DialogueAction action)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer.transform);
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = text;
        
        Button btn = buttonObj.GetComponent<Button>();
        btn.onClick.AddListener(() => OnChoiceClicked(nextNode, action));
    }

    private void OnChoiceClicked(DialogueNode nextNode, DialogueAction action)
    {
        if (isTyping) return; // Impede clicar antes do texto terminar

        if (action == DialogueAction.OpenShopp)
        {
            EndDialogue();

            PlayerInventory inventory = currentPlayer.GetComponent<PlayerInventory>();
            ShopManager.Instance.OpenShop(inventory);

            return;
        }


        if (nextNode != null)
        {
            DisplayNode(nextNode); // Continua a conversa
        }
        else
        {
            EndDialogue(); // Encerra se não houver próximo nó
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        
        activeNPCCamera.gameObject.SetActive(false);
        
        // NOVO: Religa a câmera salva em vez de tentar buscar o Camera.main
        if (storedPlayerCamera != null) 
        {
            storedPlayerCamera.gameObject.SetActive(true);
        }

        if (currentPlayer != null)
        {
            currentPlayer.GetComponent<PlayerMovement>().FreezePlayer(false);
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}