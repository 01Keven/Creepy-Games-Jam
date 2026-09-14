using System;
using UnityEngine;

public class BuriedItem : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject realItemPrefab; // Prefab do item a ser instanciado
    [SerializeField] private MeshRenderer visualCueRenderer; // Renderer para o sinal visual


    private float currentAlpha = 0f;
    private void Awake()
    {
        if (visualCueRenderer != null)
        {
            visualCueRenderer.material.color = new Color(1, 1, 1, 0); // Torna o sinal visual transparente inicialmente
        }
    }

    private void Update()
    {
        if (currentAlpha > 0f)
        {
            currentAlpha -= Time.deltaTime * 0.5f;
            ApplyAlpha(currentAlpha);
        }
    }



    public void UpdateVisualCue(float intensity)
    {
       
        currentAlpha = intensity;
        ApplyAlpha(currentAlpha);

        
    }

    private void ApplyAlpha(float alpha)
    {
        if (visualCueRenderer != null)
        {
            Color currentColor = visualCueRenderer.material.color;
            currentColor.a = Mathf.Clamp01(alpha);
            visualCueRenderer.material.color = currentColor;
        }
    }

    public void Interact(GameObject interactor)
        {
            Debug.Log($"[DEBUG BURIAL] Interação recebida de: {interactor.name}");
            
            MetalDetector detector = interactor.GetComponentInChildren<MetalDetector>();
            
            if (detector != null)
            {
                Debug.Log($"[DEBUG BURIAL] Detector encontrado! Está ligado? {detector.isOn}");
                
                if (detector.isOn)
                {
                    Debug.Log("[DEBUG BURIAL] Sucesso! Cavando e revelando o item real.");
                    DigUpItem();
                }
                else
                {
                    Debug.LogWarning("[DEBUG BURIAL] Falha: O detector está equipado, mas está DESLIGADO.");
                }
            }
            else
            {
                Debug.LogWarning("[DEBUG BURIAL] Falha: Nenhum detector encontrado na mão do player.");
            }
        }

    private void DigUpItem()
    {
        // Instancia o item real no local do item enterrado
        Instantiate(realItemPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject); // Destroi o item enterrado após desenterrar
    }
}