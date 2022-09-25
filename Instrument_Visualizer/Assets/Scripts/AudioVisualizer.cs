using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioVisualizer : MonoBehaviour
{
	public AudioSource targetAudio;
	public List<Transform> audioSpectrumObjects = new List<Transform>();
	[Min(1)] public float heightMultiplier;
	[Range(6, 13)] [Tooltip("This number is powered by 2, resulting in a multiple that fits between 64 and 8192")]
	public int numberOfSamples = 6;

	private int convertedSamples;
	public float visualizerSpan = 21f;
	[Range(0.01f, 0.5f)]public float lerpTime = 0.01f;
	public GameObject visualizerObj;

	/*
	 * The intensity of the frequencies found between 0 and 44100 will be
	 * grouped into 1024 elements. So each element will contain a range of about 43.06 Hz.
	 * The average human voice spans from about 60 hz to 9k Hz
	 * we need a way to assign a range to each object that gets animated. that would be the best way to control and modify animatoins.
	*/

	private void Start()
    {
		if (targetAudio == null)
        {
			targetAudio = GetComponent<AudioSource>();
			GetComponent<VoiceDetection>().enabled = true;
		}
		else
        {
			GetComponent<VoiceDetection>().enabled = false;
		}

		audioSpectrumObjects.Clear();
		GenerateVisualizerObjs();
    }

	[ContextMenu("Generate Visualizer Objs")]
	public void GenerateVisualizerObjs()
    {
		convertedSamples = Mathf.FloorToInt(Mathf.Pow(2f, Mathf.RoundToInt(numberOfSamples)));

		ClearVisualizer();

		float samplesScales = visualizerSpan / convertedSamples;

		for (int i = 0; i < convertedSamples; i++)
		//for (int i = 0; i < 170; i++)
		{
			//Debug.Log("ACTUAL override: " + overrideValue);
			//Debug.Log(i);
			var instantiatedObj = Instantiate(visualizerObj, new Vector3((samplesScales * i) - (visualizerSpan/2) + transform.position.x, transform.position.y, transform.position.z), new Quaternion(0,0,0,0));
			instantiatedObj.transform.localScale = new Vector3(samplesScales, 1, 1);
			instantiatedObj.transform.parent = transform;
			instantiatedObj.name = "Frequency " + i;
			audioSpectrumObjects.Add(instantiatedObj.transform);
		}
	}

	[ContextMenu("Clear Visualizer Objs")]
	public void ClearVisualizer()
    {
		GameObject currentObj;
		int childs = transform.childCount;

		for (int i = childs - 1; i >= 0; i--)
		{
			currentObj = transform.GetChild(i).gameObject;
			
			if(Application.isPlaying == true)
            {
				Destroy(currentObj);
            }
			else
            {
				DestroyImmediate(currentObj);
            }
		}

		audioSpectrumObjects.Clear();
	}

    void Update()
	{
		// initialize our float array
		float[] spectrum = new float[convertedSamples];

		// populate array with fequency spectrum data
		targetAudio.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

		for (int j = 0; j < convertedSamples; j++)
		{
			float intensity = spectrum[j] * heightMultiplier;

			//this one below doesn't work for some reason... the idea is for it to average the spectrum data by dividing it into the number of cubes that have been created
			//that way you can manually set how many cubes you want to use instead of just hte exact amount of frequencies captured

			//float intensity = correctedSpectrum[j];
			//Debug.Log("type 1 = " + spectrum[i] * heightMultiplier + " type 2 = " + frequencyRange * heightMultiplier);

			// calculate object's scale
			float lerpY = Mathf.Lerp(audioSpectrumObjects[j].localScale.y, intensity, lerpTime);
			Vector3 newScale = new Vector3(audioSpectrumObjects[j].localScale.x, lerpY, audioSpectrumObjects[j].localScale.z);

			// appply new scale to object
			audioSpectrumObjects[j].localScale = newScale;

		}
	}
}