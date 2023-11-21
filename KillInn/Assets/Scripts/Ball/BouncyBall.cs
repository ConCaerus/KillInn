using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : Throwable {
    [SerializeField] float babyTime, babyMaxAmt, unclampedBabyInc, noise;
    float weight;
    float recallDist = 2f;

    Transform playerTrans;
    PlayerBouncyBallAttack pbba;

    Coroutine babyModeWaiter = null, distChecker = null;


    private void OnEnable() {
        getRb().velocity = Vector2.zero;
        weight = 0f;
        if(babyModeWaiter != null)
            StopCoroutine(babyModeWaiter);
    }
    private void FixedUpdate() {
        setSavedThrownVel(getRb().velocity);
        if(weight != 0f) {
            var n = new Vector2(Random.Range(-noise, noise), Random.Range(-noise, noise));
            var dir = ((Vector2)transform.position - (Vector2)playerTrans.position + n).normalized;
            setSavedThrownVel(getSavedThrownVel() - dir * weight);
            applySavedThrownVel();
        }
    }

    protected override void custTriggerEnter(Collider2D col) {
        if(col.gameObject.tag == "Enemy")
            col.gameObject.GetComponent<MortalInstance>().getHit(transform, col.gameObject.transform, enemyDmg, 0f);
    }
    protected override void custTriggerExit(Collider2D col) {
    }
    protected override void init() {
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        pbba = FindObjectOfType<PlayerBouncyBallAttack>();
    }
    public override float getBorderBouncyMod() {
        return 1;
    }

    public void startBeingThrown() {
        if(distChecker == null)
            distChecker = StartCoroutine(distCheckWaiter());
    }

    //  clamped baby mode
    public void toggleBabyMode() {
        if(!gameObject.activeInHierarchy)
            return;
        weight = 0;
        getRb().velocity = Vector2.zero;
        if(babyModeWaiter != null)
            StopCoroutine(babyModeWaiter);
        babyModeWaiter = StartCoroutine(babyMode());
    }
    IEnumerator babyMode() {
        //  checks if time = 0f
        if(babyTime == 0f)
            weight = babyMaxAmt;
        else {
            weight = 0f;
            while(weight < babyMaxAmt) {
                weight += babyMaxAmt / babyTime * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }
    IEnumerator distCheckWaiter() {
        //  wait for ball to leave dist
        while(Vector2.Distance(playerTrans.position, transform.position) <= recallDist)
            yield return new WaitForEndOfFrame();
        //  wait for ball to enter dist
        while(Vector2.Distance(playerTrans.position, transform.position) > recallDist) 
            yield return new WaitForEndOfFrame();
        pbba.returnBall();
        distChecker = null;
    }

    /*
    //  baby mode that grows as long as right click is being pressed
    public void startUnclampedBabyMode() {
        if(babyModeWaiter != null)
            return;
        babyModeWaiter = StartCoroutine(unclampedBabyMode());
    }
    public void stopBabyMode() {
        if(babyModeWaiter != null)
            StopCoroutine(babyModeWaiter);
        babyModeWaiter = null;
    }
    IEnumerator unclampedBabyMode() {
        weight = 0f;
        while(true) {
            weight += unclampedBabyInc * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }*/
}
