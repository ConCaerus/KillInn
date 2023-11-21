using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public static class SpriteFX {

    public static void hurtFX(SpriteRenderer sr, CoroutineRunner me, UnityEvent runOnDun) {
        me.StartCoroutine(hurtFXWaiter(sr, runOnDun));
    }

    static IEnumerator hurtFXWaiter(SpriteRenderer sr, UnityEvent runOnDun) {
        int flashes = 5;
        var prevColor = sr.color;
        float t1 = .075f, t2 = .15f;
        float a = .35f;
        for(int i = 0; i < flashes; i++) {
            sr.color = new Color(1f, 0f, 0f, a);
            yield return new WaitForSeconds(t1);
            if(i == 0)
                yield return new WaitForSeconds(t1);
            sr.color = new Color(prevColor.r, prevColor.g, prevColor.b, a);
            yield return new WaitForSeconds(t2);
        }
        sr.color = prevColor;
        runOnDun.Invoke();
    } 
}
