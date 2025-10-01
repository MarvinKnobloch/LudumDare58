using UnityEngine;

public interface IInteractables
{
    public GameObject interactObj { get; }
    public string interactiontext { get; }
    public void Interaction();
}
