using UnityEngine;

public class MetalDetector : MonoBehaviour
{
    public bool isOn = false; // Estado do detector de metais
    public bool isEquipped = true; // Estado de equipar ou desequipar o detector de metais

    [SerializeField] private float detectionRadius = 10f; // Alcance de detecção do detector de metais
    [SerializeField] private Transform detectorTip; // A ponta do detector

    private InputSystem_Actions inputActions;
    private MeshRenderer detectorMesh;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        detectorMesh = GetComponent<MeshRenderer>();

    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        HandleInputs();

        if (isOn && isEquipped)
        {
            ScanForBuriedItems();
        }
    }

    private void HandleInputs()
        {
            if (inputActions.Player.HolsterDetector.WasPressedThisFrame())
            {

                if (!isEquipped)
                {
                    PlayerInventory inventory = GetComponentInParent<PlayerInventory>();
                    if (inventory != null && inventory.isHoldingTwoHandedItem())
                    {
                        Debug.Log("Não pode sacar enquanto segura o detector");
                        return;
                    }
                }
                isEquipped = !isEquipped;
                detectorMesh.enabled = isEquipped;
                if (!isEquipped) isOn = false;
            }

            if (isEquipped && inputActions.Player.ToggleDetector.WasPressedThisFrame())
            {
                isOn = !isOn;
                Debug.Log("[DEBUG DETECTOR] Botão pressionado. Detector Ligado: " + isOn);

            }
        }

        private void ScanForBuriedItems()
        {
            Collider[] hitColliders = Physics.OverlapSphere(detectorTip.position, detectionRadius);

            foreach (var hitCollider in hitColliders)
            {
                BuriedItem buried = hitCollider.GetComponent<BuriedItem>();
                if (buried != null)
                {
                    float distance = Vector3.Distance(detectorTip.position, buried.transform.position);
                    float intensity = 1f - (distance / detectionRadius);

                    // LOG DE RASTREAMENTO (Vai flodar o console, bom para testar distâncias)
                    // Debug.Log($"[DEBUG DETECTOR] Rastreado: {buried.gameObject.name} | Distância: {distance:F2} | Intensidade enviada: {intensity:F2}");

                    buried.UpdateVisualCue(Mathf.Clamp01(intensity));
                }
            }
        }

        public void ForceHolster()
    {
        if (isEquipped)
        {
            isEquipped = false;
            isOn = false;
            detectorMesh.enabled = false;
            Debug.Log("Guardado a força item pesado");
        }
    }
    }