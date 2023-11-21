using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossHealth : MortalInstance {
    float curThreash;

    private void Start() {
        DOTween.Init();
        curThreash = maxHealth * (2f / 3f);
    }

    public override void getHit(Transform hitter, Transform hittie, int dmg, float throwAmt) {
        health -= dmg;

        if(health <= curThreash) {
            curThreash -= maxHealth / 3f;
            hittie.DOMoveY(-10f, 1f);
            hittie.GetComponent<Collider2D>().enabled = false;
        }
    }
}
