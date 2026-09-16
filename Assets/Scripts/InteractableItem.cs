using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class InteractableItem : MonoBehaviour, IInteractable
{

    public string itemName = "Interactable Item";
    public int itemValor = 0; 
    public bool isSellable = true; // pode ser vendido?
    public bool requiresTwoHands = false;
    public bool isCursedItem = false;

    public bool isPullingItem = false;
    public float pullForce = 3f;

    private Rigidbody rb;
    private Collider itemCollider;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }

    public virtual void OnPickup(Transform handTransform)
    {
        rb.isKinematic = true;
        itemCollider.enabled = false;

        transform.SetParent(handTransform);
        transform.localPosition = Vector3.zero; // centraliza o item na mão
        transform.localRotation = Quaternion.identity; // reseta a rotação do item
    }

    public virtual void OnDrop()
    {
        transform.SetParent(null); // remove o item da mão do jogador
        rb.isKinematic = false;
        itemCollider.enabled = true;

        rb.AddForce(Camera.main.transform.forward * 3f, ForceMode.Impulse); // aplica uma força para frente ao soltar o item
    }

    public virtual void Interact(GameObject interactor)
    {
        MetalDetector detector = interactor.GetComponentInChildren<MetalDetector>();

        if (requiresTwoHands && detector != null && detector.isEquipped)
        {
            Debug.Log("Item pesado de mais, guarde o detector");
            return;
        }

        Debug.Log($"{interactor.name} interacted with {gameObject.name}, valor: {itemValor}");
        PlayerInventory playerInventory = interactor.GetComponent<PlayerInventory>();

        if (playerInventory != null)
        {
            playerInventory.PickupItem(this);
        }
    }
}