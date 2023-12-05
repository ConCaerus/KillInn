using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public abstract class Throwable : MonoBehaviour {
    [SerializeField] protected int enemyDmg;
    [SerializeField] List<Collider2D> subCols = new List<Collider2D>();
    Vector2 savedThrownVel;
    Rigidbody2D rb;

    Collider2D mainCol;

    [SerializeField] List<CustSubEventPair> custSubColEvents = new List<CustSubEventPair>();
    Dictionary<string, UnityEvent> custSubEvents = new Dictionary<string, UnityEvent>();

    [System.Serializable]
    public class CustSubEventPair {
        public string tag;
        public UnityEvent ev;
    }


    private void OnTriggerEnter2D(Collider2D col) {
        custTriggerEnter(col);
        if(custSubEvents.ContainsKey(col.gameObject.tag))
            custSubEvents[col.gameObject.tag].Invoke();
    }
    private void OnTriggerExit2D(Collider2D col) {
        custTriggerExit(col);
    }


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        mainCol = GetComponent<Collider2D>();
        rb.velocity = Vector2.zero;
        Physics2D.IgnoreLayerCollision(gameObject.layer, FindObjectOfType<PlayerMovement>().gameObject.layer);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("BallIgnore"));

        custSubColEvents.Reverse();   //  loop starts from the end, so reverse before doing the loop
        foreach(var i in custSubColEvents) {
            //  already has a key with this tag
            if(custSubEvents.ContainsKey(i.tag))
                custSubEvents[i.tag].AddListener(delegate { i.ev.Invoke(); });
            else
                custSubEvents.Add(i.tag, i.ev);
        }
        init();
    }
    private void OnEnable() {
        rb.velocity = Vector2.zero;
    }
    private void FixedUpdate() {
        savedThrownVel = rb.velocity;
    }

    //  abstract funcs
    protected abstract void init();
    protected abstract void custTriggerEnter(Collider2D col);
    protected abstract void custTriggerExit(Collider2D col);
    public abstract float getBorderBouncyMod();

    //  internal funcs
    protected void applySavedThrownVel() {
        rb.velocity = savedThrownVel;
    }

    //  sets
    public void setMainColState(bool b) {
        mainCol.enabled = b;
    }
    public void setSubColTriggerState(bool b) {
        Debug.Log("here");
        foreach(var i in subCols)
            i.isTrigger = b;
    }
    protected void setSavedThrownVel(Vector2 newVel) {
        savedThrownVel = newVel;
    }

    //  gets
    public Rigidbody2D getRb() {
        return rb;
    }
    public Vector2 getSavedThrownVel() {
        return savedThrownVel;
    }
    public UnityEvent getCustSubEvents(string tag) {
        if(!custSubEvents.ContainsKey(tag)) return new UnityEvent();
        return custSubEvents[tag];
    }
}
