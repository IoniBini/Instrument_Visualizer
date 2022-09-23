using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PitchChanger : MonoBehaviour
{
    public AudioMixer masterMixer;
    [Range(-30, 54)] public int halfSteps = 0;
    [Range(-80, 20)] public int volume = 0;
    public Gradient colourGradient;

    // Update is called once per frame
    void Update()
    {
        masterMixer.SetFloat("_pitch", Mathf.Pow(1.0594631f, halfSteps));
        masterMixer.SetFloat("_volume", volume);

        float tmpLocation = Mathf.InverseLerp(-30, 54, halfSteps);
        var tmpColor = colourGradient.Evaluate(tmpLocation);

        //GetComponentInChildren<ObjUpdater>().SetColor(tmpColor);
    }
}
