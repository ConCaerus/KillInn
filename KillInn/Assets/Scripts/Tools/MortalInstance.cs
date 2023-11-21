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
    Coroutine invicTimer = null;


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

    public void startInvinc(float t) {
        if(!gameObject.activeInHierarchy)
            return;
        StartCoroutine(invincWaiter(t));
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
        invicTimer = null;
    }
    
    public abstract void getHit(Transform hitter, Transform hittie, int dmg, float throwAmt);
}
