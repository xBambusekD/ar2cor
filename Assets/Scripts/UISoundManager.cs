using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class UISoundManager : Singleton<UISoundManager> {

    public AudioClip UiClick;
    public AudioClip HololensClick;
    public AudioClip Error;
    public AudioClip Place;
    public AudioClip Snap;

    public AudioSource Player;

    public void PlayUIButtonClick() {
        Player.PlayClip(UiClick);
    }

    public void PlayHololensClick() {
        Player.PlayClip(HololensClick);
    }

    public void PlayError() {
        Player.PlayClip(Error);
    }

    public void PlayPlace() {
        Player.PlayClip(Place);
    }

    public void PlaySnap() {
        Player.PlayClip(Snap);
    }
}
