using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManipulator : MonoBehaviour {

    Coroutine bulletTimer = null;

    public void startBulletTime(float slowAmt, float timeSlowed) {
        if(bulletTimer != null)
            StopCoroutine(bulletTimer);
        bulletTimer = StartCoroutine(bulletTime(slowAmt, timeSlowed));
    }

    IEnumerator bulletTime(float slowAmt, float timeSlowed) {
        Time.timeScale = slowAmt;
        yield return new WaitForSeconds(slowAmt * timeSlowed);
        Time.timeScale = 1f;
        bulletTimer = null;
    }
}
