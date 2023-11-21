using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CrushingWalls : MonoBehaviour {
    [SerializeField] GameObject left, right;
    [SerializeField] float introJumpAmt, introJumpTime, afterIntroWaitTime;
    [SerializeField] float speed, maxSpeed, speedIncAmt, repelAmt;
    [SerializeField] int hitsNeeded;
    float startSpeed;

    Vector2 rightOrigin, leftOrigin;

    public delegate void func(float t);
    func runOnComplete = null;

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Ball" && hitsNeeded > 0)
            repelWalls();
        else if(col.gameObject.tag == "Player")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Start() {
        DOTween.Init();
        startSpeed = speed;

        right.SetActive(false);
        left.SetActive(false);
    }

    public void startCrushing(func runAfterStart, func runOnBeat) {
        runOnComplete = runOnBeat;
        //  sets the position of the walls to be just beyond the screen
        //  right
        right.SetActive(true);
        var p = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
        p += right.transform.lossyScale.x / 2f;
        right.transform.position = new Vector3(p, right.transform.position.y);
        rightOrigin = right.transform.position;

        //  left
        left.SetActive(true);
        p = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        p -= left.transform.lossyScale.x / 2f;
        left.transform.position = new Vector3(p, left.transform.position.y);
        leftOrigin = left.transform.position;

        //  crusher
        StartCoroutine(crusher(runAfterStart));
    }


    IEnumerator crusher(func runAfterStart) {
        //  jump at the start to grab the player's attention
        right.transform.DOMoveX(right.transform.position.x - introJumpAmt, introJumpTime);
        left.transform.DOMoveX(left.transform.position.x + introJumpAmt, introJumpTime);
        yield return new WaitForSeconds(introJumpTime + afterIntroWaitTime);

        if(runAfterStart != null)
            runAfterStart(0f);  //  float doesn't matter

        //  crusher
        while(hitsNeeded > 0) {
            right.transform.position = Vector2.MoveTowards(right.transform.position, new Vector2(left.transform.position.x, right.transform.position.y), speed * Time.deltaTime);
            left.transform.position = Vector2.MoveTowards(left.transform.position, new Vector2(right.transform.position.x, left.transform.position.y), speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void repelWalls() {
        right.transform.position = new Vector3(right.transform.position.x + repelAmt, right.transform.position.y);
        left.transform.position = new Vector3(left.transform.position.x - repelAmt, left.transform.position.y);
        speed = Mathf.Clamp(speed + speedIncAmt, startSpeed, maxSpeed);
        hitsNeeded--;
        
        //  player did it
        if(hitsNeeded <= 0) {
            if(runOnComplete != null)
                runOnComplete(.25f);
            right.transform.DOMoveX(rightOrigin.x + 5f, .24f);
            left.transform.DOMoveX(leftOrigin.x - 5f, .24f);
            Destroy(right.gameObject, .25f);
            Destroy(left.gameObject, .25f);
        }
    }
}
