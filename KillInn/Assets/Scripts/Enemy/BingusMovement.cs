using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BingusMovement : EnemyMovement {
    Transform playerTrans;

    private void Start() {
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
    }

    protected override void move() {
        var d = Mathf.Abs(transform.position.x - playerTrans.position.x);
        Debug.Log("here");
        //  returns if doesn't see player or if likes distance from player
        GetComponent<SpriteRenderer>().color = Color.red;
        if(d > sightDist || d < minDistFromPlayer)
            return;
        GetComponent<SpriteRenderer>().color = Color.green;

        Vector2 target;
        //  checks if too far away
        if(d > minDistFromPlayer)
            target = Vector2.MoveTowards(transform.position, new Vector2(playerTrans.position.x, transform.position.y), moveSpeed * Time.deltaTime);
        else
            target = Vector2.MoveTowards(transform.position, new Vector2(playerTrans.position.x, transform.position.y), -moveSpeed * Time.deltaTime);

        //  funny
        transform.position = target;
    }

    protected override void runOnStun() {
        GetComponent<SpriteRenderer>().color = Color.red;
    }
}
