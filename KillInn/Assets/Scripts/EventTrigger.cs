using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {
    [SerializeField] List<string> tags = new List<string>();
    [SerializeField] UnityEvent runOnStart;
    [SerializeField] UnityEvent runOnTrigger;

    private void Start() {
        runOnStart.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(tags.Contains(col.gameObject.tag)) 
            runOnTrigger.Invoke();
    }
}
