using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour
{
    bool isPlaying;
    private Button button;

    private static AudioSource audioClip;
    private static AudioSource AudioClip
    {
        get
        {
            if (audioClip == null)
            {
                var obj = Instantiate(Resources.Load<AudioSource>("ButtonClicker"));
                audioClip = obj.GetComponent<AudioSource>();
            }

            return audioClip;
        }
    }

    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        button.onClick.AddListener(() =>
        {
            AudioClip.Play();
        });
    }
}
