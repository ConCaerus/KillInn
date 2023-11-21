using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUI : MonoBehaviour {
    [SerializeField] bool lockY = false;
    [SerializeField] float parallaxAmt;
    [SerializeField] List<GameObject> parallaxLayers;
    List<List<Transform>> images = new List<List<Transform>>();
    List<List<Vector2>> origins = new List<List<Vector2>>();

    Transform camTrans;

    Vector2 camOrigin;


    private void Start() {
        camOrigin = Camera.main.transform.position;
        camTrans = Camera.main.transform;

        //  populates
        for(int i = 0; i < parallaxLayers.Count-1; i++) {
            images.Add(new List<Transform>());
            origins.Add(new List<Vector2>());
            foreach(var j in parallaxLayers[i].GetComponentsInChildren<Transform>()) {
                images[i].Add(j);
                origins[i].Add(j.transform.localPosition);
            }
        }
    }

    private void LateUpdate() {
        parallax();
    }

    void parallax() {
        Vector2 camCurrent = camTrans.position;
        var offset = camCurrent - camOrigin;
        float step = parallaxAmt;
        if(lockY)
            offset.y = 0f;

        for(int i = 0; i < images.Count; i++) {
            var targ = -offset * step;
            for(int j = 0; j < images[i].Count; j++) 
                images[i][j].localPosition = (Vector3)(origins[i][j] + targ);
            step -= parallaxAmt / (parallaxLayers.Count-1);
        }
    }
}
