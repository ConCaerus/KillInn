using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MortalInstance : MonoBehaviour {

    public int maxHealth;
    public int health { get; protected set; }
    [SerializeField] float sliderSpeed;

    [SerializeField] Slider healthBar;

    protected bool invincible = false;
    Coroutine invincTimer = null;

    float invincTime = .5f;


    private void Awake() {
        health = maxHealth;
        setupHealthBar();
        StartCoroutine(sliderAnim());
    }

    protected void setupHealthBar() {
        if(healthBar == null)
            return;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    protected void startInvinc() {
        if(!gameObject.activeInHierarchy)
            return;
        invincTimer = StartCoroutine(invincWaiter(invincTime));
    }

    IEnumerator sliderAnim() {
        if(healthBar == null)
            yield break;
        while(healthBar.value > 0f) {
            healthBar.value = Mathf.Lerp(healthBar.value, health, sliderSpeed * 10f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator invincWaiter(float t) {
        invincible = true;
        yield return new WaitForSeconds(t);
        invincible = false;
        invincTimer = null;
    }
    
    public abstract void getHit(Transform hitter, Transform hittie, int dmg, float throwAmt);
}
