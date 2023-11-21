using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Club : ScriptableObject {
    public enum ClubType {
        None, Putter, Iron
    }

    public ClubType type;
    public float maxHitAmt;
}
