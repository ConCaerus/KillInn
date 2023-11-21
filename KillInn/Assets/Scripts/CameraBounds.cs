using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour {

    enum boundsDir {
        None, Top, Left, Bottom, Right
    }


    [SerializeField] boundsDir dir;

    public void reposition() {
        var t1 = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
        var t2 = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));
        //  scale
        switch(dir) {
            case boundsDir.Top:
            case boundsDir.Bottom:
                transform.localScale = new Vector2(t2.x - t1.x, 1f);
                break;
            case boundsDir.Left:
            case boundsDir.Right:
                transform.localScale = new Vector3(1f, t2.x - t1.x);
                break;
        }

        //  position
        switch(dir) {
            case boundsDir.Top:
                transform.position = new Vector3(Camera.main.transform.position.x, t2.y, -Camera.main.transform.position.z);
                break;
            case boundsDir.Bottom:
                transform.position = new Vector3(Camera.main.transform.position.x, t1.y, -Camera.main.transform.position.z);
                break;
            case boundsDir.Left:
                transform.position = new Vector3(t1.x, Camera.main.transform.position.y, -Camera.main.transform.position.z);
                break;
            case boundsDir.Right:
                transform.position = new Vector3(t2.x, Camera.main.transform.position.y, -Camera.main.transform.position.z);
                break;
        }
    }
}
