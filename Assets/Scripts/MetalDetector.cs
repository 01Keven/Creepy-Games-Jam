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
            isEquipped = !isEquipped;
            detectorMesh.enabled = isEquipped;

            if (!isEquipped) isOn = false;
        }

        if (isEquipped && inputActions.Player.ToggleDetector.WasPressedThisFrame())
        {
            isOn = !isOn;
            Debug.Log("Detector Ligador " + isOn);
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

                buried.UpdateVisualCue(Mathf.Clamp01(intensity));
            }
        }
    
    }



}