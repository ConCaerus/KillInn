using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour {
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float sightDist, minDistFromPlayer;

    protected bool canMove = true;

    Coroutine stunner = null;

    private void LateUpdate() {
        if(canMove)
            move();
    }

    protected abstract void move();
    protected abstract void runOnStun();

    public void setCanMove(bool b) {
        canMove = b;
    }
    public void stunForTime(float t) {
        if(!gameObject.activeInHierarchy)
            return;
        runOnStun();
        if(stunner != null)
            StopCoroutine(stunner);
        stunner = StartCoroutine(stunWaiter(t));
    }
    IEnumerator stunWaiter(float t) {
        canMove = false;
        yield return new WaitForSeconds(t);
        canMove = true;
        stunner = null;
    }
}
