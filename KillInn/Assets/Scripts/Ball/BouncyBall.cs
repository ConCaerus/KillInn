using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : Throwable {
    [SerializeField] float babyTime, babyMaxAmt, unclampedBabyInc, noise;
    float weight;
    float recallDist = 2f;
    [SerializeField] float returnSpeed;

    Transform playerTrans;
    PlayerBouncyBallAttack pbba;

    Coroutine returnModeWaiter = null, distChecker = null;

    List<Vector2> bouncePoints = new List<Vector2>();
    int curBouncePointsIndex = -1;


    private void OnEnable() {
        getRb().velocity = Vector2.zero;
        weight = 0f;
        if(returnModeWaiter != null)
            StopCoroutine(returnModeWaiter);
    }
    private void FixedUpdate() {
        setSavedThrownVel(getRb().velocity);
        if(weight != 0f) {
            var n = new Vector2(Random.Range(-noise, noise), Random.Range(-noise, noise));
            var dir = ((Vector2)transform.position - (Vector2)playerTrans.position + n).normalized;
            setSavedThrownVel(getSavedThrownVel() - dir * weight);
            applySavedThrownVel();
        }

        /*  boring
        if(curBouncePointsIndex < bouncePoints.Count && curBouncePointsIndex > -1) {
            //  checks dist
            if(Vector2.Distance(transform.position, bouncePoints[curBouncePointsIndex]) < .5f) {
                bouncePoints.RemoveAt(curBouncePointsIndex);
                curBouncePointsIndex--;
                if(curBouncePointsIndex < 0)
                    pbba.returnBall();
            }
            else {
                transform.position = Vector2.MoveTowards(transform.position, bouncePoints[curBouncePointsIndex], returnSpeed * 10f * Time.fixedDeltaTime);
            }
        }
        */
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
        return 1f;
    }

    public void addBouncePoints() {
        if(curBouncePointsIndex == -1) {
            bouncePoints.Add(transform.position);
        }
    }

    public void startBeingThrown() {
        if(distChecker == null) {
            distChecker = StartCoroutine(distCheckWaiter());
            curBouncePointsIndex = -1;
            bouncePoints.Clear();
            addBouncePoints();
            weight = 0;
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

    //  clamped baby mode
    public void toggleReturnMode() {
        if(!gameObject.activeInHierarchy)
            return;
        weight = 0;
        curBouncePointsIndex = bouncePoints.Count - 1;
        getRb().velocity = Vector2.zero;
        getRb().gravityScale = 0f;
        if(returnModeWaiter != null)
            StopCoroutine(returnModeWaiter);
        returnModeWaiter = StartCoroutine(weightInc());
    }
    IEnumerator weightInc() {
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
        returnModeWaiter = null;
    }

    public bool isBeingThrown() {
        return gameObject.activeInHierarchy;
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
