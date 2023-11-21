using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCol : MonoBehaviour {

    PlayerMovement pm;

    private void OnTriggerEnter2D(Collider2D col) {
        pm.touchedGround(col);
    }
    private void OnTriggerExit2D(Collider2D col) {
        pm.leftGround(col);
    }


    private void Awake() {
        pm = GetComponentInParent<PlayerMovement>();
    }
}
