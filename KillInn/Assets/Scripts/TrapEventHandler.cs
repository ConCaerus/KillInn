using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrapEventHandler : MonoBehaviour {
    [SerializeField] SpriteRenderer pileSr;
    [SerializeField] Transform mainCamPoint, beatCamPoint, startCamPoint;
    [SerializeField] Transform playerStartPoint;
    [SerializeField] float timeB4Crushing;
    [SerializeField] int maxWiggles;
    [SerializeField] float wiggleMoveAmt;

    Transform playerTrans;
    PlayerMovement pm;
    PlayerBouncyBallAttack pa;
    CameraMovement cm;
    CrushingWalls cw;

    InputMaster controls;

    bool crusherReady = false;
    bool canWiggle = false;
    bool wiggleState = false;   //  true - wiggles right next, false - wiggles left next

    private void Awake() {
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        cm = FindObjectOfType<CameraMovement>();
        cw = FindObjectOfType<CrushingWalls>();
        pm = FindObjectOfType<PlayerMovement>();
        pa = FindObjectOfType<PlayerBouncyBallAttack>();

        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => wiggle();

        StartCoroutine(handler());
    }

    private void OnDisable() {
        controls.Disable();
    }

    IEnumerator handler() {
        cm.setCanMove(false);
        cm.snapToPos(startCamPoint.position);
        pa.setCanAttack(false);
        pm.setCanMove(false);
        //  starts the player at a high y point, so they fall into the scene
        playerTrans.position = playerStartPoint.position;
        playerTrans.GetComponent<Collider2D>().enabled = false;

        //  wait for the player to come into frame of the camera for the camera to start following the falling player
        while(playerTrans.position.y > startCamPoint.position.y)
            yield return new WaitForEndOfFrame();
        cm.setHardFollowPlayer(true);
        cm.setCanMove(true);
        playerTrans.GetComponent<Collider2D>().enabled = true;

        //  wait for the player to fall into the pile
        var camTrans = Camera.main.transform;
        while(camTrans.position.y > mainCamPoint.position.y)
            yield return new WaitForEndOfFrame();
        cm.setHardFollowPlayer(false);
        cm.setCanMove(false);

        yield return new WaitForSeconds(timeB4Crushing);
        canWiggle = true;

        //  wait for the crusher to start
        cw.startCrushing(crusherIsReady, changeCamPoint);
        while(!crusherReady)
            yield return new WaitForEndOfFrame();

        //  allow the player to move (wiggle) and start the crushing of the walls
        while(maxWiggles > 0f)
            yield return new WaitForEndOfFrame();

        //  plays some anim of the player escaping the pile
        canWiggle = false;

        //  player has wiggled free, let them do stuff
        pa.setCanAttack(true);
        pm.setCanMove(true);
        pileSr.sortingOrder -= 2;
        pileSr.DOColor(new Color(pileSr.color.r, pileSr.color.g, pileSr.color.b, .25f), .25f);
        //  pile still remains, but it's rendered behind the player and it doesn't have a collider
    }

    void wiggle() {
        if(canWiggle) {
            pileSr.transform.DOComplete();
            pileSr.transform.DOPunchPosition((wiggleState ? Vector2.right : Vector2.left) * wiggleMoveAmt, .15f);
            wiggleState = !wiggleState;
            maxWiggles--;
        }
    }

    void crusherIsReady(float nothing) {
        crusherReady = true;
    }
    void changeCamPoint(float prepTime) {
        StartCoroutine(changeCamPointWaiter(prepTime));
    }

    IEnumerator changeCamPointWaiter(float prepTime) {
        yield return new WaitForSeconds(prepTime);
        yield return new WaitForSeconds(1f);
        mainCamPoint.transform.position = beatCamPoint.position;
        cm.setCanMove(true);
    }
}
