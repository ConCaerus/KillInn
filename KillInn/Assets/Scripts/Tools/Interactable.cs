using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {
    [SerializeField] UnityEvent interactEvents;
    [SerializeField] UnityEvent uninteractEvents;
    [SerializeField] bool holdsAttention = true;
    public float interactRadius;

    bool interacting = false;

    [HideInInspector] public bool canInteract = true;

    public void toggleInteraction(PlayerMovement pm) {
        //  if interactable has an interacting state
        if(holdsAttention) {
            if(!interacting) {
                pm.setCanMove(false);
                interactEvents.Invoke();
            }
            else {
                pm.setCanMove(true);
                uninteractEvents.Invoke();
            }
            interacting = !interacting;
        }

        //  if interactable is a one-time thing
        else {
            interactEvents.Invoke();
        }
    }

    private void Awake() {
        FindObjectOfType<PlayerInteracting>().addInteractable(this);
    }
}
