using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {
    [SerializeField] GameObject tickPreset;
    [SerializeField] Transform tickHolder;
    List<HealthTick> ticks = new List<HealthTick>();
    Transform mouser;

    PlayerHealth ph;

    float spacing = 30f;


    private void Start() {
        ph = FindObjectOfType<PlayerHealth>();
        //  spawns ticks
        int tickCount = (int)((ph.maxHealth / 2f) + .5f);
        for(int i = 0; i < tickCount; i++) 
            ticks.Add(Instantiate(tickPreset.gameObject, tickHolder).GetComponent<HealthTick>());
        rearrangeTicks();
        setTickStates();

        PlayerHealth.extraHitLogic += setTickStates;

        mouser = new GameObject().transform;
    }

    private void OnDisable() {
        PlayerHealth.extraHitLogic -= setTickStates;
    }

    private void Update() {
        mouser.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetKeyDown(KeyCode.H))
            ph.getHit(mouser, ph.transform, 1, 400f);
    }


    void rearrangeTicks() {
        for(int i = 0; i < ticks.Count; i++) {
            ticks[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(i * spacing, 0f);
        }
    }
    void setTickStates(bool doesNothing = false) {
        var curHealth = ph.health;
        var releventIndex = (int)(curHealth / 2f + .5f) - 1;
        for(int i = 0; i < ticks.Count; i++) {
            if(i < releventIndex)
                ticks[i].setFillState(HealthTick.tickState.Full);
            else if(i == releventIndex)
                ticks[i].setFillState((float)curHealth % 2 == 0 ? HealthTick.tickState.Full : HealthTick.tickState.Half);
            else
                ticks[i].setFillState(HealthTick.tickState.Empty);
        }
    }
}
