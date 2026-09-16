using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Sistema de Dialogo/Novo Texto")]
public class DialogueNode : ScriptableObject
{
    [Header("Informações da Fala")]
    public string npcName = "Vendedor";
    
    [TextArea(3, 5)]
    public string dialogueText; // O que o NPC vai falar

    [Header("Opções de Resposta (Deixe vazio para apenas 'Avançar')")]
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText; // O texto do botão (Ex: "Vender moedas")
    public DialogueNode nextNode; // Para qual diálogo essa escolha leva (pode ser nulo para fechar a conversa)
}