using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteracting : MonoBehaviour {
    InputMaster controls;
    PlayerMovement pm;
    InteractUI ic;

    KdTree<Interactable> interactables = new KdTree<Interactable>();

    Coroutine helperChecker = null;

    private void Start() {
        pm = GetComponent<PlayerMovement>();
        if(FindObjectOfType<InteractUI>() == null)
            Debug.LogError("Add Interact Canvas to the scene");
        ic = FindObjectOfType<InteractUI>();
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Interact.performed += ctx => interact();
        ShrineHandler.init();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void addInteractable(Interactable i) {
        interactables.Add(i);
        if(helperChecker == null)
            helperChecker = StartCoroutine(helperWaiter());
    }

    void interact() {
        var shrineDist = 100f;
        if(ShrineHandler.shrineCount > 0) {
            shrineDist = ShrineHandler.getClosestDist(transform.position);
        }

        //  trying to interact with an interactable
        if(interactables.Count > 0) {
            var relevantInteractable = interactables.FindClosest(transform.position);
            var normDist = Vector2.Distance((Vector2)transform.position, (Vector2)relevantInteractable.transform.position);

            //  if closer than shrine's closest and in range to be interacted with
            if(normDist < shrineDist && normDist <= relevantInteractable.interactRadius) {
                relevantInteractable.toggleInteraction(pm);
                checkClosestInteractable(relevantInteractable, false);
                return;
            }
        }

        //  shrines are closer
        ShrineHandler.updateCurrentShrine(transform.position);
    }

    IEnumerator helperWaiter() {
        Interactable closest = interactables.FindClosest(transform.position);
        while(interactables.Count > 0) {
            yield return new WaitForEndOfFrame();

            checkClosestInteractable(closest, true);

            if(!ic.isShowing)
                yield return new WaitForSeconds(.1f);
        }
        helperChecker = null;
    }
    void checkClosestInteractable(Interactable closest, bool checkClosest) {
        //  finds new interactable
        if(checkClosest && closest != interactables.FindClosest(transform.position))
            closest = interactables.FindClosest(transform.position);

        //  checks if closest is available
        if(!closest.gameObject.activeInHierarchy || !closest.canInteract) {
            ic.hide();
            return;
        }

        //  checks if should be showing
        if(Vector2.Distance(transform.position, closest.transform.position) <= closest.interactRadius)
            ic.showAtPos(closest.transform);
        else if(ic.isShowing)
            ic.hide();
    }
}
