using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class UnseenArmInstance : MonoBehaviour {
    [SerializeField] Transform hand;
    Transform playerTrans;
    Coroutine mover = null;

    public UnseenAttack ua;

    private void OnTriggerEnter2D(Collider2D col) {
        //  grabs player
        if(col.gameObject.tag == "Player") {
            if(mover != null)
                StopCoroutine(mover);
            transform.parent = col.gameObject.transform;
            col.gameObject.GetComponent<PlayerMovement>().modRealSpeed(.5f);
            ua.incGrabbing();
        }
    }

    public void armStart(float moveSpeed, UnseenAttack unsAtt) {
        if(mover != null)
            return;
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        rotateTowardsPos(playerTrans.position, true);
        ua = unsAtt;
        mover = StartCoroutine(armMover(moveSpeed, playerTrans.position));
    }
    void rotateTowardsPos(Vector2 pos, bool checkRot, float speed = -1f) {
        var off = (Vector2)transform.position - pos;
        if(off.magnitude > .01f) {
            Quaternion rotation = Quaternion.LookRotation(off, transform.TransformDirection(Vector3.up));
            if(checkRot && Vector2.Distance(hand.transform.position, playerTrans.position) > Vector2.Distance(transform.position, playerTrans.position))
                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z + 180f);
            var target = new Quaternion(0, 0, rotation.z, rotation.w);
            transform.rotation = speed <= 0f ? target : Quaternion.RotateTowards(transform.rotation, target, speed * Time.deltaTime);
        }
    }
    void disappear() {
        float disTime = .25f;
        foreach(var i in GetComponentsInChildren<SpriteRenderer>())
            i.DOColor(Color.clear, disTime);
        GetComponent<SpriteRenderer>().DOColor(Color.clear, disTime);
        Destroy(gameObject, disTime + .01f);
    }
    IEnumerator armMover(float moveSpeed, Vector2 target) {
        float scaleFactor = transform.lossyScale.x / 2f;
        float distToLunge = 5f;
        int state = 0;  //  0 - start moving towards the player, 1 - close enough to lunge for the player
        while(true) {
            //  slowly moves towards the set target pos
            if(state == 0) {
                transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
                //  check for state change
                if(Vector2.Distance(hand.position, target) < distToLunge)
                    state = 1;
                //  adjust target to move slightly with the player
                else {
                    target = Vector2.MoveTowards(target, playerTrans.position, moveSpeed * Time.deltaTime);
                    rotateTowardsPos(target, true);
                }
            }

            //  lunges towards the target
            else if(state == 1) {
                //  wait before lunging
                float lungeWaitTime = 2f, elapsedTime;
                while(lungeWaitTime >= 0f) {
                    elapsedTime = Time.time;
                    yield return new WaitForEndOfFrame();
                    lungeWaitTime -= Time.time - elapsedTime;

                    target = Vector2.MoveTowards(target, playerTrans.position, moveSpeed * 2f * Time.deltaTime); //  pos
                    rotateTowardsPos(target, true);   //  rot
                }
                //  lunge
                while(Vector2.Distance(transform.position, target) > scaleFactor) {
                    transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * 5f * Time.deltaTime);
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(.5f);
                mover = null;
                disappear();
                break;
            }
        }
    }
}
