using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BossHealth : MortalInstance {
    [SerializeField] int ballDmg;
    [SerializeField] Rigidbody2D rb;


    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Ball")
            getHit(col.gameObject.transform, transform, ballDmg, 100f);
    }


    public override void getHit(Transform hitter, Transform hittie, int dmg, float throwAmt) {
        if(invincible)
            return;
        health -= dmg;

        var dir = (hittie.position - hitter.position).normalized * throwAmt;
        rb.velocity = dir;

        startInvinc();
    }
}
