using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShrineHandler {
    static KdTree<Transform> shrines = new KdTree<Transform>();
    public static int shrineCount = -1;
    static Transform curShrine;
    static float interactDist = 3f;

    static Vector2 spawnOffset = new Vector2(0f, 1f);

    static string curShrineTagX = "CurrentShrineTagX";
    static string curShrineTagY = "CurrentShrineTagY";

    static bool inUse;
    static bool initted = false;

    public static void init() {
        inUse = GameObject.FindGameObjectsWithTag("Shrine").Length != 0;
        if(!inUse || initted) return;
        foreach(var i in GameObject.FindGameObjectsWithTag("Shrine"))
            shrines.Add(i.transform);
        shrineCount = shrines.Count;
        var p = new Vector2(SaveData.getFloat(curShrineTagX, -1f), SaveData.getFloat(curShrineTagY, -1f));
        if(p.x == -1 && p.y == -1)
            return;
        curShrine = shrines.FindClosest(p);
        foreach(var i in shrines) {
            i.GetComponent<SpriteRenderer>().color = i == curShrine ? Color.blue : Color.white;
        }
        initted = true;
    }

    public static void updateCurrentShrine(Vector2 playerPos) {
        if(!inUse) return;
        //  checks if player is in range to swap shrines
        if(getClosestDist(playerPos) > interactDist) {
            return;
        }
        curShrine = shrines.FindClosest(playerPos);
        SaveData.setFloat(curShrineTagX, curShrine.position.x);
        SaveData.setFloat(curShrineTagY, curShrine.position.y);
        foreach(var i in shrines) {
            i.GetComponent<SpriteRenderer>().color = i == curShrine ? Color.blue : Color.white;
        }
    }
    public static float getClosestDist(Vector2 pos) {
        return inUse ? Vector2.Distance(shrines.FindClosest(pos).position, pos) : 0f;
    }

    public static KdTree<Transform> getShrines() {
        return shrines;
    }
    public static Transform getCurrentShrine() {
        if(!inUse) return null;
        return shrines.FindClosest(new Vector2(SaveData.getFloat(curShrineTagX), SaveData.getFloat(curShrineTagY)));
    }
    public static Vector2 getRespawnPos() {
        return (Vector2)curShrine.position + spawnOffset;
    }
}
