using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallInstance : MonoBehaviour {
    Rigidbody2D rb;
    Transform playerTrans;
    SpriteRenderer sr;

    Coroutine descaler = null;

    int puts = 0;   //  tell this to the player maybe
    Vector2 lastPos;

    private void Start() {
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        rb.velocity = new Vector2(0f, -.1f);
        descaler = StartCoroutine(descale());
    }

    public void hit(float hitAmt) {
        //  finds angle btw camera and player
        var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var d = mPos - playerTrans.position;
        d.Normalize();

        rb.velocity = d * hitAmt;
        if(descaler == null)
            descaler = StartCoroutine(descale());
        puts++;
    }

    public void resetBall() {
        transform.position = lastPos;
        rb.velocity = Vector2.zero;
    }

    public bool readyForHit() {
        return rb.velocity == Vector2.zero;
    }

    IEnumerator descale() {
        sr.transform.DOScale(1.25f, .15f);
        while(rb.velocity != Vector2.zero)
            yield return new WaitForEndOfFrame();
        sr.transform.DOScale(1f, .15f);
        lastPos = transform.position;
        descaler = null;
    }
}
