using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour {
    [SerializeField] bool fullTick;
    [SerializeField] float throwAmt;

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            var offset = col.gameObject.transform.position - transform.position;
            col.gameObject.GetComponent<PlayerHealth>().getHit(transform, col.transform, fullTick ? 3 : 1, throwAmt);
        }
    }
}
