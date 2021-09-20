using UnityEngine;

public class AudioScript : MonoBehaviour
{
    //A very simple audio manager just to play click sounds
    public AudioSource titleTheme;
    public AudioSource clickSound;
    public AudioSource buttonPressedSound;

    public void PlaySound(Sounds which)
    {
        switch (which)
        {
            case Sounds.Theme:
                titleTheme.Play();
                break;
            case Sounds.Click:
                clickSound.Play();
                break;
            case Sounds.ButtonPress:
                buttonPressedSound.Play();
                break;
        }
    }
}

public enum Sounds //easier to understand than an int
{
    Theme,
    Click,
    ButtonPress
};