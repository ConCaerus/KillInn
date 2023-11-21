using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClydeAttack : EnemyAttack {
    [SerializeField] GameObject skellyArm;
    [SerializeField] float armWaveDist, armWaveRate, armWaveTimeStep;

    Transform playerTrans;

    bool attacking = false;

    List<float> weights = new List<float>() {
        0f, 2f, 0f, 0f
    };

    public override List<Attack> attacks() {
        return new List<Attack>() { attack1, attack2, attack3, attack4 };
    }

    private void Start() {
        DOTween.Init();
        playerTrans = FindObjectOfType<PlayerMovement>().transform;
        StartCoroutine(attackLoop());   //  change this to include an intro to the fight
    }


    public void attack1() {
        attacking = true;
        Debug.Log("Me! I'm the first one!");
    }
    public void attack2() {
        attacking = true;
        StartCoroutine(armWave(playerTrans.position.x > transform.position.x));
    }
    public void attack3() {
        attacking = true;
        Debug.Log("Me! I'm the 3 one!");
    }
    public void attack4() {
        attacking = true;
        Debug.Log("Me! I'm the 4ourthone!");
    }


    IEnumerator attackLoop() {
        attack(weights);
        while(attacking)
            yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(3f);
        StartCoroutine(attackLoop());
    }

    IEnumerator armWave(bool right) {
        float dist = 0f;
        float step = armWaveRate * armWaveDist;
        Vector2 curPos = transform.position;
        while(dist < armWaveDist) {
            curPos += new Vector2(right ? step : -step, 0f);
            dist += step;

            //  arm
            var arm = Instantiate(skellyArm);
            StartCoroutine(armAnim(arm, curPos));
            yield return new WaitForSeconds(armWaveTimeStep);
        }
        attacking = false;
    }
    IEnumerator armAnim(GameObject arm, Vector2 activePos) {
        var hiddenPos = activePos - new Vector2(0f, arm.transform.lossyScale.y);
        arm.transform.position = hiddenPos;
        arm.transform.DOMoveY(activePos.y, .25f);
        yield return new WaitForSeconds(1f);
        arm.transform.DOMoveY(hiddenPos.y, .35f);
        Destroy(arm.gameObject, .36f);
    }
}
