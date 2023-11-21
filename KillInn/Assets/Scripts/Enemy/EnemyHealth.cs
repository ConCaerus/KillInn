using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MortalInstance {
    EnemyMovement em;


    private void Start() {
        em = GetComponent<EnemyMovement>();
    }

    public override void getHit(Transform hitter, Transform hittie, int dmg, float throwAmt) {
        if(invincible)
            return;
        health -= dmg;
        Debug.Log("her");
        startInvinc(.5f);
        em.stunForTime(2f);
        if(health < 0f)
            gameObject.SetActive(false);
    }
}
