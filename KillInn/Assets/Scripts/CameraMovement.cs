using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class CameraMovement : MonoBehaviour {
    Transform playerTrans;
    [SerializeField] float moveSpeed, minDistFromEdge;

    [SerializeField] List<Transform> camPoints = new List<Transform>();
    KdTree<Transform> kdCamPoints = new KdTree<Transform>();

    bool canMove = true, canChangePoint = true;

    Transform curCamPoint = null;
    PlayerMovement pm;
    PlayerBouncyBallAttack pbba;

    bool hardFollowPlayer = false;

    private void Awake() {
        DOTween.Init();
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        pm = playerTrans.GetComponent<PlayerMovement>();
        pbba = FindObjectOfType<PlayerBouncyBallAttack>();
        toggleBoundsForLayer(false, playerTrans.gameObject.layer);

        //  sets up cam points and finds it's starting pos
        if(camPoints != null && camPoints.Count > 0) {
            foreach(var i in camPoints)
                kdCamPoints.Add(i);
            curCamPoint = kdCamPoints.FindClosest(playerTrans.position);
            if(curCamPoint != null)
                transform.position = new Vector3(curCamPoint.position.x, curCamPoint.position.y, transform.position.z);
        }

        //  sets up the collider bounds 
        foreach(var i in FindObjectsOfType<CameraBounds>())
            i.reposition();
    }

    private void LateUpdate() {
        //  returns if shouldn't be moving
        if(!canMove || pm.isBeingThrown()) return;

        //  hard follows player
        if(hardFollowPlayer)
            followPlayer();

        //  move to defined, special points around the scene
        else {
            if(kdCamPoints.Count > 0) {
                if(needsNewTarget(.01f) || (canChangePoint && needsNewTarget(minDistFromEdge) && kdCamPoints.Count > 0)) {
                    findNewTarget();
                }
            }
            if(curCamPoint != null) {
                moveWithWeightedPlayer();
            }
        }
    }

    void followPlayer() {
        var target = new Vector3(playerTrans.position.x, playerTrans.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void findNewTarget() {
        if(curCamPoint != null) {
            var temp = curCamPoint;
            kdCamPoints.RemoveAll(x => x == curCamPoint);
            curCamPoint = kdCamPoints.FindClosest(playerTrans.position);
            kdCamPoints.Add(temp);
        }
        else {
            curCamPoint = kdCamPoints.FindClosest(playerTrans.position);
        }
    }
    void moveWithWeightedPlayer() {
        var target = new Vector3(curCamPoint.position.x, curCamPoint.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public bool needsNewTarget(float minDist) {
        if(pbba != null && pbba.isBeingThrown())
            return false;
        var highs = Camera.main.ViewportToWorldPoint(Vector3.one);
        var lows = Camera.main.ViewportToWorldPoint(Vector3.zero);

        float heightRatio = (float)Screen.height / Screen.width;

        //  checks along x
        if(Mathf.Abs(highs.x - playerTrans.position.x) <= minDist || Mathf.Abs(lows.x - playerTrans.position.x) <= minDist)
            return isPlayerFacingEdge(highs.x, lows.x); //  checks if the player is facing the correct way


        return false;   //  doesn't check along y because fuck that
        //  checks along y
        return Mathf.Abs(highs.y - playerTrans.position.y) <= minDist ||
                Mathf.Abs(lows.y - playerTrans.position.y) <= minDist;
    }

    public void setCanMove(bool b) {
        if(b)
            curCamPoint = kdCamPoints.FindClosest(playerTrans.position);
        canMove = b;
    }
    public void setHardFollowPlayer(bool b) {
        hardFollowPlayer = b;
    }
    public void snapToPlayer() {
        snapToPos(playerTrans.position);
    }
    public void snapToPos(Vector2 pos) {
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    public static void toggleBoundsForLayer(bool actState, int l) {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("CameraBounds"), l, !actState);
    }

    public void setCanChangePoint(bool b) {
        canChangePoint = b;
    }

    bool isPlayerFacingEdge(float rightEdge, float leftEdge) {
        var px = playerTrans.position.x;
        return Mathf.Abs(px - rightEdge) < Mathf.Abs(px - leftEdge) ? pm.isFacingRight() : !pm.isFacingRight();
    }
}
