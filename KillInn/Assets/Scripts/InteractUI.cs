using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractUI : MonoBehaviour {
    [SerializeField] GameObject image;
    [SerializeField] float offset;
    [SerializeField] float showTime, hideTime;

    Transform curTrans;

    [HideInInspector] public bool isShowing = false;


    private void Awake() {
        image.transform.localScale = Vector3.zero;
    }

    public void showAtPos(Transform trans) {    //  called from playerInteracting script
        if(isShowing) {
            if(curTrans != trans) {
                image.transform.DOComplete();
                hide();
                showAtPos(trans);
                return;
            }
            else
                image.transform.position = new Vector3(trans.position.x, image.transform.position.y);
        }
        else
            image.transform.DOComplete();
        isShowing = true;
        curTrans = trans;
        image.transform.DOScale(1f, showTime);
        image.transform.position = trans.position;
        image.transform.DOMoveY(trans.position.y + offset, showTime);
    }
    public void hide() {
        if(!isShowing)
            return;
        isShowing = false;
        image.transform.DOComplete();
        image.transform.DOScale(0f, hideTime);
        image.transform.DOMoveY(image.transform.position.y - offset, hideTime);
    }
}
