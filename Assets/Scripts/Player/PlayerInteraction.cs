using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction
{
    public Player player;
    private float checkTimer;
    private float checkInterval = 0.2f;

    public void InteractInput(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            InteractPerformed();
        }
    }
    private void InteractPerformed()
    {
        if (IngameController.Instance.menuController.gameIsPaused)
        {
            return;
        }
        if (player.currentInteractable == null) return;

        switch (player.state)
        {
            case Player.States.Ground:
                player.currentInteractable.Interaction();
                break;
            case Player.States.GroundIntoAir:
                player.currentInteractable.Interaction();
                break;
            case Player.States.Air:
                player.currentInteractable.Interaction();
                break;
        }
    }
    public void AddInteraction(IInteractables interactable)
    {
        player.interactables.Add(interactable);
        GetClosestInteraction();
        IngameController.Instance.playerUI.HandleInteractionBox(true);
    }
    public void RemoveInteraction(IInteractables interactable)
    {
        if (player.interactables.Contains(interactable)) player.interactables.Remove(interactable);
        else return;

        InteractionUpdate();

        if (player.interactables.Count == 0)
        {
            player.currentInteractable = null;
            IngameController.Instance.playerUI.HandleInteractionBox(false);
        }
    }
    public void InteractionUpdate()
    {
        if (player.interactables.Count != 0)
        {
            checkTimer += Time.deltaTime;
            if (checkInterval > checkTimer)
            {
                checkTimer = 0;
                GetClosestInteraction();
            }
        }
    }
    public void GetClosestInteraction()
    {
        float closestDistance = 10f;
        foreach (IInteractables interaction in Player.Instance.interactables)
        {
            float currentDistance;
            currentDistance = Vector3.Distance(Player.Instance.transform.position, interaction.interactObj.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                player.closestInteraction = interaction;
            }
        }
        player.currentInteractable = player.closestInteraction;
        IngameController.Instance.playerUI.InteractionTextUpdate(player.currentInteractable.interactiontext);
    }
}
