using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer5 : MonoBehaviour
{
    public float[] spectrum;
    public float heightMultiplier = 1;
    public Vector2 frequencyRange = new Vector2(0, 10);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // initialize our float array
        spectrum = new float[1024];

        // populate array with fequency spectrum data
        GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        float currentValue = 0;
        int numberOfFrequencies = 0;

        for (int i = 0; spectrum.Length > i; i++)
        {
            if (i > frequencyRange.x && i < frequencyRange.y)
            {
                currentValue += spectrum[i];
                numberOfFrequencies++;
            }
        }

        float average = currentValue / numberOfFrequencies;

        transform.localScale = new Vector3(1, average * heightMultiplier, 1);
    }
}
