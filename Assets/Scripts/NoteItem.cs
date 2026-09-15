using UnityEngine;

public class NoteItem : InteractableItem
{


    private bool isReading = false;
    private PlayerMovement playerMovementRef;

    public void ToggleReading()
    {
        isReading = !isReading;


        if (playerMovementRef == null)
        {
            playerMovementRef = GetComponentInParent<PlayerMovement>();
        
        }

        if (playerMovementRef != null)
        {
            if (playerMovementRef.uiNote != null)
            {
                playerMovementRef.uiNote.SetActive(isReading);
            }
            
            playerMovementRef.FreezePlayer(isReading);
        }


    }

    public override void OnDrop()
    {
        if (isReading)
        {
            ToggleReading();
        }

        base.OnDrop();
    }
}