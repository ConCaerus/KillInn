using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour {

    public delegate void Attack();

    public abstract List<Attack> attacks();

    //  NOTE: weights are local!!!!
    public void attack(List<float> weights = null) {
        //  determines what the weights will be
        if(weights == null || weights.Count != attacks().Count) {
            weights = new List<float>();
            foreach(var i in attacks()) {
                weights.Add(1f / attacks().Count);
            }
        }

        //  finds the number that needs comparing to
        float wTot = 0f;
        foreach(var i in weights)
            wTot += i;

        //  determines the output from the randomness
        float depth = 1000f;
        float rand = Random.Range(0f, depth);
        float guess = (rand / depth) * wTot;

        //  find the index of the weights that won
        int foundInd = -1;
        float savedInd = 0;
        for(int i = 0; i < weights.Count; i++) {
            savedInd += weights[i];
            if(guess <= savedInd) {
                foundInd = i;
                break;
            }
        }

        //  do the attack with the same index
        attacks()[foundInd]();
    }
}
