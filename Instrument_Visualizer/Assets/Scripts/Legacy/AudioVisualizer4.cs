using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer4 : MonoBehaviour
{
    AudioSource _audioSource;
    public float[] _samples = new float[512];
    public float[] _freqBand = new float[8];

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
    }

    public void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    public void MakeFrequencyBands()
    {
        int count = 0;
        float average = 0;

        for (int i = 0; i < 8; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                    count++;
            }

            average /= count;

            _freqBand[i] = average * 10;
        }
    }
}
