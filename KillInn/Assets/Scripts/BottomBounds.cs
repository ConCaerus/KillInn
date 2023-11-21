using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BottomBounds : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Ball")
            col.gameObject.GetComponent<BallInstance>().resetBall();
        else if(col.gameObject.tag == "Player")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
