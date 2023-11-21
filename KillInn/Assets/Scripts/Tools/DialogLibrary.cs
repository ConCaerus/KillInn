using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogLibrary {
    public enum Talker {
        Carl, Marv, None
    }

    static Dictionary<string, Talker> talkerPairs = new Dictionary<string, Talker>();

    static bool initted = false;
    public static void init() {
        int index = 0;
        talkerPairs.Clear();
        Talker curTalker = (Talker)index;

        //  adds all talkers and their string pairs into the dictionary
        while(curTalker != Talker.None) {
            talkerPairs.Add(curTalker.ToString(), curTalker);
            curTalker = (Talker)(++index);
        }
        //  pair Talker.None with an empty string
        talkerPairs.Add(string.Empty, curTalker);
        initted = true;
    }

    public static DialogSequence getTalkersDialog(Talker talker) {
        switch(talker) {
            case Talker.None: return null;
            case Talker.Carl: return carlDialog();
            case Talker.Marv: return marvDialog();
        }
        return null;
    }

    #region TESTS
    static DialogSequence carlDialog() {
        return new DialogSequence(new List<Dialog>() {
                new Dialog("Hey, names Carl.", null), new Dialog("...Apparently", new DialogOption("yes", "no", null, null))
            });
    }
    static DialogSequence marvDialog() {
        return new DialogSequence(new List<Dialog>() {
                new Dialog("Marv here.", null), new Dialog("...Apparently", new DialogOption("no", "yes", null, delegate {Debug.Log("It's Marvin time!"); })),
                new Dialog("Okay, Greate!", new DialogOption("yessir", "nossir", delegate {Debug.Log("yessir"); }, delegate {Debug.Log("nossir"); }))
            });
    }
    #endregion

    public static Talker getTalker(string t) {
        //  checks if initted
        if(!initted)
            init();
        return talkerPairs[t];
    }
}
