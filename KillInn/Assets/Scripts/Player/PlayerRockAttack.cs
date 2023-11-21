using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRockAttack : PlayerAttack {
    PlayerMovement pm;
    InputMaster controls;
    [SerializeField] float throwSpeed;
    GameObject curBall = null;

    List<Interactable> rockInstances = new List<Interactable>();

    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Attack.performed += ctx => attack();

        foreach(var i in FindObjectsOfType<RockInstance>())
            rockInstances.Add(i.GetComponent<Interactable>());
    }

    private void OnDisable() {
        controls.Disable();
    }

    protected override void custAttack() {
        curBall.SetActive(true);
        curBall.transform.position = transform.position;

        var dir = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        curBall.GetComponent<Rigidbody2D>().velocity = dir.normalized * 10f * throwSpeed;
        foreach(var i in rockInstances) {
            if(i.gameObject.GetInstanceID() != curBall.gameObject.GetInstanceID())
                i.canInteract = true;
        }
        curBall = null;
    }

    protected override bool custCanAttack() {
        return curBall != null && !pm.isBeingThrown();
    }

    public void pickupRock(GameObject rock) {
        if(curBall != null)
            return;

        foreach(var i in rockInstances) {
            i.canInteract = false;
        }
        curBall = rock;
        rock.SetActive(false);
    }
    public bool hasRock() {
        return curBall != null;
    }
}
