using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmationUI : MenuInstance {
    [SerializeField] GameObject background;
    [SerializeField] Button yButton, nButton;

    string savedSceneName = "";
    Interactable questioner = null;

    PlayerMovement pm;

    public delegate void func();

    private void Start() {
        pm = FindObjectOfType<PlayerMovement>();
        hide();
    }

    public void showUI(func yes, func no) {
        isOpen = true;
        yButton.onClick.RemoveAllListeners();
        yButton.onClick.AddListener(delegate { yes(); });
        yButton.onClick.AddListener(delegate { hide(); });
        nButton.onClick.RemoveAllListeners();
        nButton.onClick.AddListener(delegate { no(); });
        nButton.onClick.AddListener(delegate { hide(); });
        background.SetActive(true);
    }


    public void setSelectedScene(string sceneName) {
        savedSceneName = sceneName;
        hide();
    }
    public void loadSelectedSave(Interactable thing) {
        questioner = thing;
        if(string.IsNullOrEmpty(savedSceneName)) {
            Debug.LogError("No Saved Scene");
            return;
        }
        showUI(delegate { SceneManager.LoadScene(savedSceneName); }, delegate { Debug.Log("didn't"); });
    }

    public void hide() {
        isOpen = false;
        background.SetActive(false);
        if(questioner != null)
            questioner.toggleInteraction(pm);
        questioner = null;
    }
}
