using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//  Decide if the player gets thrown after they've been hit
public class PlayerHealth : MortalInstance {
    [SerializeField] SpriteRenderer sr;
    PlayerMovement pm;

    const int dmgLimit = 2; //  if dmg < that -> half tick : if dmg > that -> full tick

    public static Action<bool> extraHitLogic;


    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
    }

    //  if dmg is greater than 2, deals a full tick of damage
    public override void getHit(Transform hitter, Transform hittie, int dmg, float throwAmt) {
        if(invincible)
            return;

        //  effects
        UnityEvent e = new UnityEvent();
        e.AddListener(delegate { invincible = false; });
        invincible = true;
        SpriteFX.hurtFX(sr, Unity.VisualScripting.CoroutineRunner.instance, e);

        //  logic
        health -= dmg < dmgLimit ? 1 : 2;
        pm.beThrown((hittie.position - hitter.position).normalized * throwAmt);

        //  run extra
        extraHitLogic(dmg >= dmgLimit);

        if(health <= 0)
            Destroy(gameObject);
    }
}
