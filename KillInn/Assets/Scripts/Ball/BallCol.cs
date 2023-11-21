using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallCol : MonoBehaviour {
    [SerializeField] orders ord;
    [SerializeField] Throwable bb;


    public enum orders {
        None, Top, Right, Bottom, Left
    }

    private void OnCollisionEnter2D(Collision2D col) {
        //  rebounds off walls
        if(col.gameObject.tag == "Ground" || col.gameObject.tag == "Bounds" || col.gameObject.tag == "BallHittable" || col.gameObject.tag == "HurtsPlayer") {
            Vector2 mod = Vector2.zero;
            var bbbbm = bb.getBorderBouncyMod();
            switch(ord) {
                case orders.Top:
                    mod = new Vector2(1f, -1f);
                    break;
                case orders.Right:
                    mod = new Vector2(-1f, 1f);
                    break;
                case orders.Bottom:
                    mod = new Vector2(1f, -1f);
                    break;
                case orders.Left:
                    mod = new Vector2(-1f, 1f);
                    break;
            }
            mod *= bbbbm;   //  applies how bouncy the ball should be
            bb.getRb().velocity = bb.getSavedThrownVel() * mod;
            //tm.startBulletTime(.35f, .1f);
        }

        //  returns to player
        else if(col.gameObject.tag == "Player") {
            bb.setMainColState(true);
        }

        bb.getCustSubEvents(col.gameObject.tag).Invoke();
    }
}
