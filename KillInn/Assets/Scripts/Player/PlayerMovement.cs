using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float speed, speedAccSpeed;
    [SerializeField] float jumpHeight, jumpAccSpeed;
    [SerializeField] float dashDist;
    [SerializeField] float hitWallDuringThrowDecMod;

    float realSpeed;

    bool facingRight = true;

    InputMaster controls;
    Rigidbody2D rb;
    Collider2D col;
    CameraMovement cm;
    [SerializeField] Transform thrownCols;
    [SerializeField] Animator anim;
    [SerializeField] PhysicsMaterial2D slippery, frictionry;

    Coroutine queuedJump = null;
    Coroutine coyoteTime = null;
    Coroutine jumpCanceler = null;
    Coroutine dasher = null;

    float savedInput;   //  only on the x-axis
    float maxVelocity = 50f;
    float minThrownMag = 1f;
    float minBotHitMag = 10f;

    bool canMove = true;
    bool grounded = false;
    bool beingThrown = false;
    bool canStopBeingThrown = true;

    Vector2 savedThrownVel;


    private void Awake() {
        cm = FindObjectOfType<CameraMovement>();
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Move.performed += ctx => updateInput(ctx.ReadValue<Vector2>());
        controls.Player.Move.performed += ctx => cm.setCanChangePoint(true);
        controls.Player.Move.canceled += ctx => cm.setCanChangePoint(false);
        controls.Player.Move.canceled += ctx => resetInput();
        controls.Player.Jump.performed += ctx => jump();
        controls.Player.Jump.canceled += ctx => cancelJump();
        controls.Player.Dash.performed += ctx => { if(dasher == null) dasher = StartCoroutine(dash()); };

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        jumpHeight *= rb.gravityScale;
        resetRealSpeed();
    }

    private void Start() {
        if(Saver.loadPlayer() != null) {
            transform.position = Saver.loadPlayer().pos;
        }
    }

    private void OnDisable() {
        controls.Disable();
    }


    private void FixedUpdate() {
        //  not being thrown so move
        if(!beingThrown) {
            move();
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxVelocity, maxVelocity), Mathf.Clamp(rb.velocity.y, -maxVelocity, maxVelocity));
        }

        //  being thrown logic here
        else {
            savedThrownVel = rb.velocity;
            //  check if player isn't being thrown anymore
            if(canStopBeingThrown && rb.velocity.magnitude < minThrownMag && grounded) {
                //  player laying dead pose
                setThrownState(false);
                if(cm.needsNewTarget(3f))
                    cm.findNewTarget();
            }
        }
    }

    void move() {
        if(!canMove)
            savedInput = 0f;
        rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(savedInput, rb.velocity.y), speedAccSpeed * 100f * Time.fixedDeltaTime);
    }

    void jump() {
        if(!canMove || beingThrown)
            return;
        //  if grounded, jump immedietely 
        if(grounded) {
            doJump();
        }

        //  checks if coyote time applies
        else if(coyoteTime != null) {
            StopCoroutine(coyoteTime);
            coyoteTime = null;
            doJump();
        }

        //  queue up the jump
        else if(queuedJump == null) {
            queuedJump = StartCoroutine(jumpWaiter());
        }
    }
    void cancelJump() {
        if(jumpCanceler == null && !grounded)
            jumpCanceler = StartCoroutine(jumpCancelWaiter());
    }
    void doJump() {
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight * 100f * Time.fixedDeltaTime);
        anim.ResetTrigger("EndJump");
        anim.ResetTrigger("JumpDec");
        anim.SetTrigger("Jump");
        StartCoroutine(jumpStateChecker());
        //  resets the jump canceler
        if(jumpCanceler != null)
            StopCoroutine(jumpCanceler);
        jumpCanceler = null;
    }

    void updateInput(Vector2 dir) {
        savedInput = dir.normalized.x * realSpeed * 100f * Time.fixedDeltaTime;
        if(!canMove || beingThrown) {
            cm.setCanChangePoint(false);
            anim.SetBool("walking", false);
            return;
        }
        anim.SetBool("walking", true);
        facingRight = dir.x >= 0;
        //  flips character
        anim.gameObject.transform.rotation = Quaternion.Euler(0f, savedInput < 0f ? 180f : 0f, 0f);
    }
    void resetInput() {
        savedInput = 0f;
        anim.SetBool("walking", false);
    }

    public void modRealSpeed(float mult = 1f, float add = 0f) {
        realSpeed *= mult;
        realSpeed += add;
    }
    public void resetRealSpeed() {
        realSpeed = speed;
    }


    IEnumerator jumpCancelWaiter() {
        float endY = (-jumpHeight) * 100f * Time.deltaTime;
        while(rb.velocity.y > endY && !grounded) {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(rb.velocity.x, endY), jumpAccSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        jumpCanceler = null;
    }   //  if the player cancels their jump, make them fall
    IEnumerator jumpWaiter() {
        float allowedTime = 1f; //  max time allowed for this loop to run before I give up on the player
        float s = Time.time, e = Time.time; //  start and end time, etc.
        while(!grounded && allowedTime > 0f) {
            e = Time.time;
            allowedTime -= e - s;
            s = Time.time;
            yield return new WaitForEndOfFrame();
        }

        //  does the jump if can do the jump
        if(allowedTime > 0f)
            doJump();
        queuedJump = null;
    }   //  for queuing up a jump before the player is back on the ground
    IEnumerator coyoteWaiter() {
        float allowedTime = .125f;   //  max time of ungrounded allowed
        yield return new WaitForSeconds(allowedTime);
        coyoteTime = null;
    }   //  Wille//  if the player hasn't cancelled the jump, but the velocity is 0 or negative, make them fall faster
    IEnumerator jumpStateChecker() {
        //  wait for jump to start
        while(rb.velocity.y <= 0f || grounded)
            yield return new WaitForEndOfFrame();
        //  wait for state to change
        while(rb.velocity.y > 0f && !grounded)
            yield return new WaitForEndOfFrame();

        anim.SetTrigger("JumpDec");
    }

    IEnumerator dash() {
        float timeInDash = .2f;
        col.enabled = false;
        float amt = (facingRight ? dashDist : -dashDist) * 10f;
        float prevScale = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(rb.velocity.x + amt, 0f);
        yield return new WaitForSeconds(timeInDash);
        rb.gravityScale = prevScale;
        col.enabled = true;
        yield return new WaitForSeconds(.5f - timeInDash);

        dasher = null;
    }

    //  ground checks
    public void touchedGround(Collider2D col) {
        if(col.gameObject.tag == "Ground") {
            grounded = true;
            anim.SetTrigger("EndJump");

            if(beingThrown)
                return;

            //  ends coyote time
            if(coyoteTime != null)
                StopCoroutine(coyoteTime);
            coyoteTime = null;

            //  ends jump canceler
            if(jumpCanceler != null)
                StopCoroutine(jumpCanceler);
            jumpCanceler = null;
        }
    }
    public void leftGround(Collider2D col) {
        if(col.gameObject.tag == "Ground" && gameObject.activeInHierarchy) {
            grounded = false;
            canStopBeingThrown = true;

            if(beingThrown)
                return;

            //  start coyote time
            coyoteTime = StartCoroutine(coyoteWaiter());
        }
    }

    public void setCanMove(bool b) {
        canMove = b;
    }
    public bool getIsGrounded() {
        return grounded;
    }

    void setThrownState(bool b) {
        beingThrown = b;
        canStopBeingThrown = false;
        thrownCols.gameObject.SetActive(b);
        GetComponent<Collider2D>().enabled = !b;
        rb.sharedMaterial = b ? frictionry : slippery;
        CameraMovement.toggleBoundsForLayer(false, gameObject.layer);
    }

    public void beThrown(Vector2 force) {
        float minForce = 30f;
        setThrownState(true);
        //  equalizes the dir to make height mod have the same weight as width mod
        force.y *= (float)Screen.height / Screen.width;
        force.x *= (float)Screen.width / Screen.height;
        if(grounded && force.y < 0f)
            force.y *= -1f;

        if(Mathf.Abs(force.x) < minForce)
            force.x = force.x < 0f ? -minForce : minForce;
        if(Mathf.Abs(force.y) < minForce)
            force.y = force.y < 0f ? -minForce : minForce;

        rb.velocity = force;
        savedThrownVel = force;
        CameraMovement.toggleBoundsForLayer(true, gameObject.layer);
    }
    public bool isBeingThrown() {
        return beingThrown;
    }
    public void hitWallDuringThrow(Vector2 mod) {
        var targetDir = savedThrownVel * mod;
        var prevMag = targetDir.magnitude;
        targetDir.Normalize();
        rb.velocity = targetDir * (prevMag * hitWallDuringThrowDecMod);
    }

    public bool isFacingRight() {
        return facingRight;
    }
}
