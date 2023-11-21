using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthTick : MonoBehaviour {
    [SerializeField] RectMask2D halfer;

    public enum tickState {
        Empty, Half, Full
    }

    public void setFillState(tickState state) {
        switch(state) {
            case tickState.Empty:
                halfer.enabled = true;
                halfer.padding = new Vector4(0f, 0f, 25f, 0f);
                return;

            case tickState.Half:
                halfer.enabled = true;
                halfer.padding = new Vector4(0f, 0f, 12.5f, 0f);
                return;

            case tickState.Full:
                halfer.enabled = false;
                return;
        }
    }
}
