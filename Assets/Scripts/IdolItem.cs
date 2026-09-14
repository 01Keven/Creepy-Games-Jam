using UnityEngine;


public class IdolItem : InteractableItem
{
    public float sanityDrainRate = 5f;

    public override void OnPickup(Transform handTransform)
    {
        base.OnPickup(handTransform);

        Debug.Log("Idolo na sua mão");
    }

    public override void OnDrop()
    {
        base.OnDrop();

        Debug.Log("Voce soltou o Idolo");
    }
}