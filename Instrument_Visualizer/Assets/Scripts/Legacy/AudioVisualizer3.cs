using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioVisualizer3 : MonoBehaviour
{
	AudioSource _audioSource;
	public float[] _samples = new float[512];
	public float[] _freqBand = new float[8];



	public List<Vector2> audioSpectrumRanges = new List<Vector2>();
	public List<float> averages = new List<float>();
	public List<Transform> audioSpectrumObjects = new List<Transform>();
	[Min(1)] public float heightMultiplier = 1;
	[Range(6, 13)] [Tooltip("This number is powered by 2, resulting in a multiple that fits between 64 and 8192")]
	public int numberOfSamples = 6;
	private int convertedSamples;
	public float visualizerSpan = 21f;
	public FFTWindow fftWindow;
	[Range(0.01f, 0.5f)]public float lerpTime = 0.01f;
	public GameObject visualizerObj;
	private float currentFoundRangeAverage;

    /*
	 * The intensity of the frequencies found between 0 and 44100 will be
	 * grouped into 1024 elements. So each element will contain a range of about 43.06 Hz.
	 * The average human voice spans from about 60 hz to 9k Hz
	 * we need a way to assign a range to each object that gets animated. that would be the best way to control and modify animatoins.
	*/

    private void Start()
    {
		convertedSamples = Mathf.FloorToInt(Mathf.Pow(2f, Mathf.RoundToInt(numberOfSamples)));
		_audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		GetSpectrumAudioSource();
		MakeFrequencyBands();
	}

	public void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples, 0, fftWindow);
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

			_freqBand[i] = average * heightMultiplier;
		}
	}
}