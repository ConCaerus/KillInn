using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrownCol : MonoBehaviour {
    [SerializeField] orders ord;
    PlayerMovement pm;


    public enum orders {
        None, Top, Right, Bottom, Left
    }

    private void Awake() {
        pm = FindObjectOfType<PlayerMovement>();
    }


    private void OnCollisionEnter2D(Collision2D col) {
        //  rebounds off walls
        if(true || col.gameObject.tag == "Ground" || col.gameObject.tag == "Bounds" || col.gameObject.tag == "BallHittable" || col.gameObject.tag == "HurtsPlayer") {
            Vector2 mod = Vector2.zero;
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
            pm.hitWallDuringThrow(mod);
        }
    }
}
