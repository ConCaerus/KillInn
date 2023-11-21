using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockInstance : Throwable {
    Interactable inter;
    PlayerRockAttack pra;

    protected override void init() {
        Physics2D.IgnoreLayerCollision(gameObject.layer, FindObjectOfType<PlayerMovement>().gameObject.layer);
        inter = GetComponent<Interactable>();
        pra = FindObjectOfType<PlayerRockAttack>();
    }
    protected override void custTriggerEnter(Collider2D col) {
        if(col.gameObject.tag == "Ground") {
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("CameraBounds"), true);
            inter.canInteract = !pra.hasRock();
        }
    }
    protected override void custTriggerExit(Collider2D col) {
        if(col.gameObject.tag == "Ground") 
            Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("CameraBounds"), false);
    }
    public override float getBorderBouncyMod() {
        return .25f;
    }
}
