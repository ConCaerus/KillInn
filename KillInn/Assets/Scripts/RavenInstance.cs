using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RavenInstance : MonoBehaviour {
    [SerializeField] Sprite swoopingSprite;
    [SerializeField] float startSwoopSpeed = 15f;    //  speed used at the start and end of the swoop
    [SerializeField] float fastSwoopSpeed = 100f;    //  speed used at the middle of the swoop
    [SerializeField] float leaveSwoopSpeed = 50f;    //  min leaving speed

    int pointsAlongPath = 10;

    PlayerMovement pm;
    Coroutine swooper = null;

    bool swooped = false;
    bool normalOrientation;

    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
        normalOrientation = transform.position.x > pm.transform.position.x;
        if(!normalOrientation)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void swoop() {
        if(swooped || swooper != null)
            return;
        StartCoroutine(waitForGrounded());
    }

    float func(float x, float mod) {
        return mod * Mathf.Pow(x, (float)System.Math.E); //  no idea why it needs to be moded by 4, but it works
    }

    IEnumerator swooping(List<Vector2> points) {
        bool changeSpeed = true;
        var swoopDiff = fastSwoopSpeed - startSwoopSpeed;
        var swoopInc = swoopDiff / pointsAlongPath;
        float speed = startSwoopSpeed;
        for(int i = 0; i < points.Count; i++) {
            var target = points[i];
            //  rotates towards the target
            var x = target.x - transform.position.x;    //  same everytime but fuck it
            var d = Vector2.Distance(target, transform.position);
            if(d > 0f) {
                var theta = Mathf.Acos(x / d) * Mathf.Rad2Deg;
                if(normalOrientation)
                    transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, i <= pointsAlongPath + 1 ? 90f - theta + 90f : theta + 180f);
                else
                    transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, i <= pointsAlongPath + 1 ? theta : 90f - theta - 90f);
            }
            else
                transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            while((Vector2)transform.position != target) {
                transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            //  changes the speed
            if(changeSpeed) {
                if(i == pointsAlongPath + 1)    //  sets speed to fast at dip of the swoop
                    speed = fastSwoopSpeed;

                else if(i < pointsAlongPath + 1)    //  increases swoop speed as swooping down
                    speed += swoopInc;

                else if(i > pointsAlongPath + 1) {  //  decreases swoop speed after dip untill speed = leaveSpeed
                    if(speed > leaveSwoopSpeed)
                        speed -= swoopInc;
                    else {
                        speed = leaveSwoopSpeed;
                        changeSpeed = false;
                    }
                }
            }
        }
        swooper = null;
    }

    IEnumerator waitForGrounded() {
        while(!pm.getIsGrounded())
            yield return new WaitForEndOfFrame();

        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().sprite = swoopingSprite;
        swooped = true;
        var origin = pm.transform.position;
        var rPoint = transform.position;
        var xRange = Mathf.Abs(rPoint.x - origin.x);
        var yRange = Mathf.Abs(rPoint.y - origin.y);
        var a = yRange / Mathf.Pow(xRange, (float)System.Math.E);
        var step = xRange / pointsAlongPath;

        //var lr = GetComponent<LineRenderer>();
        //lr.positionCount = pointsAlongPath + 1;

        //  find points along the downward trend
        List<Vector2> downPoints = new List<Vector2>();
        for(int i = 0; i < pointsAlongPath + 1; i++) {
            var x = step * i;
            var t = origin + new Vector3(x, func(x, a));
            //lr.SetPosition(i, t);

            downPoints.Add(t);
        }
        downPoints.Reverse();

        //  find points along the upward trend (goes until off screen)
        int index = 0;
        Vector2 prevPoint = origin;
        float endY = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f)).y + 2f;
        List<Vector2> upPoints = new List<Vector2>();

        while(prevPoint.y < endY) {
            var x = step * index++;
            prevPoint = origin + new Vector3(-x, func(x, a));

            upPoints.Add(prevPoint);
        }

        List<Vector2> allPoints = new List<Vector2>();
        allPoints.AddRange(downPoints);
        allPoints.AddRange(upPoints);

        if(!normalOrientation) {
            for(int i = 0; i < allPoints.Count; i++) {
                var offsetFromOri = allPoints[i].x - origin.x;
                allPoints[i] -= new Vector2(offsetFromOri * 2f, 0f);
            }
        }

        //  does the swooping
        swooper = StartCoroutine(swooping(allPoints));
    }
}
