using UnityEngine;

public class BuriedItem : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject realItemPrefab; // Prefab do item a ser instanciado
    [SerializeField] private MeshRenderer visualCueRenderer; // Renderer para o sinal visual

    private void Awake()
    {
        if (visualCueRenderer != null)
        {
            visualCueRenderer.material.color = new Color(1, 1, 1, 0); // Torna o sinal visual transparente inicialmente
        }
    }

    public void UpdateVisualCue(float intensity)
    {
        if (visualCueRenderer != null)
        {
            Color currentColor = visualCueRenderer.material.color;
            currentColor.a = intensity;
            visualCueRenderer.material.color = currentColor;

        }
    }

    public void Interact(GameObject interactor)
    {
        MetalDetector detector = interactor.GetComponentInChildren<MetalDetector>();
        // Instancia o item real no local do item enterrado
        if (detector != null && detector.isOn)
        {
            DigUpItem();
        }
    }

    private void DigUpItem()
    {
        // Instancia o item real no local do item enterrado
        Instantiate(realItemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); // Destroi o item enterrado após desenterrar
    }
}