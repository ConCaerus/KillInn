using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalHealthInstance : MonoBehaviour {
    [SerializeField] MortalInstance reference;
    [SerializeField] string[] checkedTags;
    [SerializeField] int dmgGiven;

    private void OnCollisionEnter2D(Collision2D col) {
        if(checkedTags.Length == 0)
            return;
        foreach(var i in checkedTags) {
            if(col.gameObject.tag == i) {
                reference.getHit(col.gameObject.transform, transform, dmgGiven, 0f);
                GetComponent<SpriteRenderer>().color = Random.ColorHSV();
            }
        }
    }
}
