using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour {
    [SerializeField] GameObject background, optionsBackground;
    [SerializeField] Transform showPos, hidePos;
    [SerializeField] TextMeshProUGUI text, firstOptionText, secondOptionText;
    Button firstOptionButt, secondOptionButt;

    public bool shown { get; private set; } = false;

    DialogSequence curDialog;
    PlayerAttack pa;
    PlayerMovement pm;
    int curDialogIndex = 0;

    InputMaster controls;

    Coroutine hider = null;

    [System.Serializable]
    public delegate void func();


    private void Start() {
        DOTween.Init();
        pa = FindObjectOfType<PlayerAttack>();
        pm = FindObjectOfType<PlayerMovement>();
        firstOptionButt = firstOptionText.GetComponentInParent<Button>();
        secondOptionButt = secondOptionText.GetComponentInParent<Button>();
        controls = new InputMaster();
        controls.Enable();
        controls.Dialog.Advance.performed += ctx => advanceDialog();
        optionsBackground.SetActive(false);
        hardHide();
    }

    private void OnDisable() {
        controls.Disable();
    }

    public void showDialogForTalker(string t) {
        showDialog(DialogLibrary.getTalkersDialog(DialogLibrary.getTalker(t)));
    }

    public void showDialog(DialogSequence d) {
        //  checks if the box is shown
        if(!shown) {
            background.transform.DOMoveY(showPos.position.y, .15f);
            shown = true;
        }

        //  logic
        pa.setCanAttack(false);
        pm.setCanMove(false);

        //  starts the dialog sequence
        curDialogIndex = -1;
        curDialog = d;
        advanceDialog();
    }

    public void hide() {
        StartCoroutine(hideDialog());
    }
    public void hardHide() {
        text.text = "";
        background.transform.position = hidePos.position;
        optionsBackground.SetActive(false);
        shown = false;
        hider = null;

        //  logic
        pa.setCanAttack(true);
        pm.setCanMove(true);
    }

    IEnumerator hideDialog() {
        text.text = "";
        background.transform.DOMoveY(hidePos.position.y, .25f);
        optionsBackground.SetActive(false);
        yield return new WaitForSeconds(.25f);
        shown = false;
        hider = null;

        //  logic
        pa.setCanAttack(true);
        pm.setCanMove(true);
    }

    void advanceDialog() {
        if(!shown)
            return;
        curDialogIndex++;

        //  checks if dialog ended
        if(curDialogIndex >= curDialog.dialogs.Count) {
            if(hider == null)
                hider = StartCoroutine(hideDialog());
            return;
        }

        var d = curDialog.dialogs[curDialogIndex];
        text.text = d.dialog;
        //  shows options if there are any
        optionsBackground.SetActive(false);
        if(d.option != null && !string.IsNullOrEmpty(d.option.firstOption) && !string.IsNullOrEmpty(d.option.secondOption)) {
            optionsBackground.SetActive(true);
            firstOptionText.text = d.option.firstOption;
            secondOptionText.text = d.option.secondOption;
            firstOptionButt.onClick.RemoveAllListeners();
            secondOptionButt.onClick.RemoveAllListeners();
            firstOptionButt.onClick.AddListener(firstOptionPressed);
            secondOptionButt.onClick.AddListener(secondOptionPressed);
            firstOptionButt.onClick.AddListener(advanceDialog);
            secondOptionButt.onClick.AddListener(advanceDialog);
        }
    }

    public void firstOptionPressed() {
        if(curDialog.dialogs[curDialogIndex].option.firstFunc == null)
            return;
        curDialog.dialogs[curDialogIndex].option.firstFunc();
    }
    public void secondOptionPressed() {
        if(curDialog.dialogs[curDialogIndex].option.secondFunc == null)
            return;
        curDialog.dialogs[curDialogIndex].option.secondFunc();
    }
}

[System.Serializable]
public class DialogSequence {
    public List<Dialog> dialogs = new List<Dialog>();

    public DialogSequence(List<Dialog> s) {
        dialogs = s;
    }
}

[System.Serializable]
public class Dialog {
    public string dialog;
    public DialogOption option;

    public Dialog(string d, DialogOption op) {
        dialog = d;
        option = op;
    }
}
[System.Serializable]
public class DialogOption {
    public string firstOption;
    public string secondOption;

    public DialogUI.func firstFunc;
    public DialogUI.func secondFunc;

    public DialogOption(string fo, string so, DialogUI.func ff, DialogUI.func sf) {
        firstOption = fo;
        secondOption = so;
        firstFunc = ff;
        secondFunc = sf;
    }
}

