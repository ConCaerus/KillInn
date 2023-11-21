using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBouncyBallAttack : PlayerAttack {
    PlayerMovement pm;
    InputMaster controls;
    [SerializeField] float throwSpeed;
    [SerializeField] GameObject ballPreset;
    GameObject curBall;

    bool thrown = false;

    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Attack.performed += ctx => attack();
        controls.Player.CallBall.performed += ctx => curBall.GetComponent<BouncyBall>().toggleBabyMode();

        curBall = Instantiate(ballPreset.gameObject, transform.position, Quaternion.identity, null);
        curBall.SetActive(false);
    }

    private void OnDisable() {
        controls.Disable();
    }

    protected override void custAttack() {
        thrown = true;
        curBall.SetActive(true);
        curBall.transform.position = transform.position;
        curBall.GetComponent<BouncyBall>().startBeingThrown();

        var dir = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
        curBall.GetComponent<Rigidbody2D>().velocity = dir.normalized * 10f * throwSpeed;
    }

    protected override bool custCanAttack() {
        return !thrown && !pm.isBeingThrown();
    }

    public void returnBall() {
        if(!thrown)
            return;
        thrown = false;

        curBall.SetActive(false);
    }
}
