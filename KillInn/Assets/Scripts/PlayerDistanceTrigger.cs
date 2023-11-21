using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDistanceTrigger : MonoBehaviour {
    [SerializeField] float triggerDist;
    [SerializeField] bool onlyCheckAlongX;
    [SerializeField] UnityEvent events = new UnityEvent();
    Transform playerTrans;

    Coroutine checker = null;

    private void Start() {
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        checker = StartCoroutine(check());
    }

    IEnumerator check() {
        bool run = true;
        float dist;
        while(run) {
            dist = onlyCheckAlongX ? Mathf.Abs(playerTrans.position.x - transform.position.x) : Vector2.Distance(playerTrans.position, transform.position);
            if(dist <= triggerDist) {
                events.Invoke();
                run = false;
            }
            yield return new WaitForSeconds(.01f);
        }
        checker = StartCoroutine(resetCheck());
    }

    IEnumerator resetCheck() {
        bool run = true;
        float dist;
        while(run) {
            dist = onlyCheckAlongX ? Mathf.Abs(playerTrans.position.x - transform.position.x) : Vector2.Distance(playerTrans.position, transform.position);
            if(dist > triggerDist)
                run = false;
            yield return new WaitForSeconds(.01f);
        }
        checker = StartCoroutine(check());
    }
}
