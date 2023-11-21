using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnseenAttack : EnemyAttack {
    [SerializeField] GameObject armPreset;
    [SerializeField] Transform armHolder;
    [SerializeField] float minArmCooldown, maxArmCooldown, armCooldownInc;
    [SerializeField] float spawnDistFromPlayer;
    [SerializeField] float armSpeed;
    [SerializeField] bool spawn = true;

    Transform playerTrans;
    static Coroutine attacker = null;

    static int grabbingCount = 0, maxGrabbingCount = 10;

    private void Start() {
        grabbingCount = 0;
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        if(spawn)
            attacker = StartCoroutine(attackWaiter(maxArmCooldown));
    }

    public override List<Attack> attacks() {
        return new List<Attack>() { armAttack };
    }

    void armAttack() {
        var obj = Instantiate(armPreset, armHolder);
        obj.transform.position = getPosAlongCircle(playerTrans.position, spawnDistFromPlayer);
        obj.transform.LookAt(playerTrans.position);
        obj.GetComponent<UnseenArmInstance>().armStart(armSpeed, this);
    }

    public void incGrabbing() {
        grabbingCount++;

        //  checks if ended
        if(grabbingCount >= maxGrabbingCount)
            SceneManager.LoadScene("Curator");
    }

    Vector3 getPosAlongCircle(Vector3 center, float radius) {
        float ang = Random.Range(-90f, 90f);
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    IEnumerator attackWaiter(float curArmCooldown) {
        attack();
        yield return new WaitForSeconds(curArmCooldown);
        StartCoroutine(attackWaiter(Mathf.Clamp(curArmCooldown - armCooldownInc, minArmCooldown, maxArmCooldown)));
    }
}
