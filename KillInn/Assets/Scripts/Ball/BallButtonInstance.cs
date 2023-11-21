using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class BallButtonInstance : MonoBehaviour {
    [SerializeField] int hitsTaken = 1;
    [SerializeField] float punchAmt;
    [SerializeField] SpriteRenderer colorChangedSprite;
    public UnityEvent imediateEvents;
    [SerializeField] float happenTime;
    public UnityEvent happenEvents;

    Coroutine takeHitWaiter = null;
    float cooldownTime = .25f;

    private void OnCollisionEnter2D(Collision2D col) {
        if(takeHitWaiter == null && col.gameObject.tag == "Ball") {
            hitsTaken--;
            if(hitsTaken <= 0) {
                if(happenEvents != null && happenEvents.GetPersistentEventCount() > 0)
                    StartCoroutine(runThings());
                if(imediateEvents != null && imediateEvents.GetPersistentEventCount() > 0)
                    imediateEvents.Invoke();
            }

            //  color
            if(gameObject.activeInHierarchy && colorChangedSprite != null) {
                colorChangedSprite.DOComplete();
                var prevC = colorChangedSprite.color;
                colorChangedSprite.color = Color.red;
                colorChangedSprite.DOColor(prevC, cooldownTime);
            }

            //  shake
            if(gameObject.activeInHierarchy && punchAmt > 0f) {
                transform.DOComplete();
                var dir = transform.position.x < col.gameObject.transform.position.x ? punchAmt : -punchAmt;
                transform.DOPunchPosition(new Vector3(-dir, 0f), cooldownTime);
                takeHitWaiter = StartCoroutine(takeHitCooldown());
            }
        }
    }

    IEnumerator runThings() {
        yield return new WaitForSeconds(happenTime);
        happenEvents.Invoke();
    }

    IEnumerator takeHitCooldown() {
        yield return new WaitForSeconds(cooldownTime);
        takeHitWaiter = null;
    }
}
