using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAttack : MonoBehaviour {
    List<MenuInstance> menus = new List<MenuInstance>();
    bool canAttack = true;

    private void Awake() {
        foreach(var i in FindObjectsOfType<MenuInstance>())
            menus.Add(i);
    }

    protected abstract void custAttack();
    protected abstract bool custCanAttack();

    public void attack() {
        //  checks if can attack
        foreach(var i in menus)
            if(i.isOpen)
                return;
        if(!canAttack || !custCanAttack())
            return;

        custAttack();
    }

    public void setCanAttack(bool b) {
        canAttack = b;
    }
}
