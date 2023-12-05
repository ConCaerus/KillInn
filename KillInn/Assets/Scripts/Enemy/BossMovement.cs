using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : EnemyMovement {
    Transform playerTrans;

    Rigidbody2D rb;
    MortalInstance bh;

    bossState[] sequence = new bossState[] {
        bossState.Patrolling, bossState.Attacking, bossState.Attacking, bossState.Attacking, bossState.Stunned
    };

    //  stunned - standing still, patrolling - walking either around the arena or towards the player, attacking - doing one of their attacks
    //  boss should have some set sequence of states instead of just choosing random states
    //  sequence: patrol, attack, attack, attack, stun
    //  each attack should be a different attack than the previous
    enum bossState {
        Stunned, Patrolling, Attacking
    }

    private void Start() {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Player"));
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        bh = GetComponentInChildren<MortalInstance>();
        canMove = false;
        StartCoroutine(changeState());
    }

    protected override void move() {
    }

    protected override void runOnStun() {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    IEnumerator changeState() {
        while(!canMove)
            yield return new WaitForEndOfFrame();
        while(bh.health > 0) {
            //  loops through the entire sequence
            for(int i = 0; i < sequence.Length; i++) {
                sequence[i] = bossState.Patrolling;
                //Debug.Log(sequence[i].ToString());
                //  decides what's going to happen
                switch(sequence[i]) {
                    //  moves towards the player
                    //  beast boss, so have it be really fast on the ground making the player have to use the platforms to run away
                    //  when jumping to a different elevation level, have the boss pause for a short time giving the player some wiggle room
                    case bossState.Patrolling:
                        //  checks if the player is on a different elevation level
                        //  if they are, move along the x untill the boss is in position to jump to the new elevation
                        //  if they aren't, move towards the player
                        float elapsedTime = 0f;
                        float startTime = Time.time;
                        var facingRight = playerTrans.position.x > transform.position.x;
                        while(elapsedTime < 10f) {
                            var target = Vector2.MoveTowards(transform.position, new Vector2(playerTrans.position.x, transform.position.y), moveSpeed * 10f * Time.deltaTime);
                            //  checks if target changed directions
                            if((facingRight && target.x < transform.position.x) || (!facingRight && target.x > transform.position.x)) {
                                yield return new WaitForSeconds(.5f);
                                Debug.Log("here");
                                target = Vector2.MoveTowards(transform.position, new Vector2(playerTrans.position.x, transform.position.y), moveSpeed * 10f * Time.deltaTime);
                                facingRight = target.x > transform.position.x;
                            }
                            transform.position = target;
                            yield return new WaitForEndOfFrame();
                            elapsedTime += Time.time - startTime;
                            startTime = Time.time;
                        }
                        //Debug.Log("here");
                        break;


                    //  boss can't perform the same attack twice in a row
                    //  the final attack should be something like the patrol state but faster
                    //  after the final attack is finished, the boss keeps running in their current direction untill they hit a wall and get stunned.
                    case bossState.Attacking:
                        //  check if it's the final attack
                        //  if it isn't, pick a new attack that is different from the previous attack
                        yield return new WaitForSeconds(15f);
                        break;


                    //  gives the player some free attack time.
                    case bossState.Stunned:
                        //  start stunned anim
                        yield return new WaitForSeconds(5f);
                        break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
