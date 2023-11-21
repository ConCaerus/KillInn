using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BallShotUI : MonoBehaviour {
    [SerializeField] GameObject background;
    [SerializeField] Slider chargeSlider;

    Transform ballTrans;

    bool shown = false;

    private void Start() {
        DOTween.Init();
        ballTrans = FindObjectOfType<BallInstance>().transform;
        background.gameObject.SetActive(false);
    }

    private void LateUpdate() {
        if(shown) {
            background.transform.position = ballTrans.position;
        }
    }

    public void show() {
        shown = true;
        background.gameObject.SetActive(true);
    }
    public void hide() {
        shown = false;
        background.gameObject.SetActive(false);
    }

    public void setChargeVal(float val) {
        chargeSlider.value = val;
    }
}
