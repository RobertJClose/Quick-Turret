using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteButton : MonoBehaviour
{
    bool isMuted;

    private void Awake()
    {
        isMuted = true;
        AudioListener.volume = 0;
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
    }
}
