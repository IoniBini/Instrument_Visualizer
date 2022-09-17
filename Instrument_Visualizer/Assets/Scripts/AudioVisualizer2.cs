using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioVisualizer2 : MonoBehaviour
{
	public List<Transform> audioSpectrumObjects = new List<Transform>();
	[Min(1)] public float heightMultiplier;
	[Range(6, 13)] [Tooltip("This number is powered by 2, resulting in a multiple that fits between 64 and 8192")]
	public int numberOfSamples = 6;
	[Tooltip("When false, the number of cubes generated = number of samples, when false, you can choose whatever number you want in overrideNumber")]
	public bool overrideSamples = false;
	[Range(1, 8192)] 
	public int overrideNumber = 1024;
	private int convertedSamples;
	public float visualizerSpan = 21f;
	public FFTWindow fftWindow;
	[Range(0.01f, 0.5f)]public float lerpTime = 0.01f;
	public GameObject visualizerObj;
	[SerializeField] private List<float> correctedSpectrum;

	/*
	 * The intensity of the frequencies found between 0 and 44100 will be
	 * grouped into 1024 elements. So each element will contain a range of about 43.06 Hz.
	 * The average human voice spans from about 60 hz to 9k Hz
	 * we need a way to assign a range to each object that gets animated. that would be the best way to control and modify animatoins.
	*/

	private void Start()
    {
		audioSpectrumObjects.Clear();
		GenerateVisualizerObjs();
    }

	[ContextMenu("Generate Visualizer Objs")]
	public void GenerateVisualizerObjs()
    {
		convertedSamples = Mathf.FloorToInt(Mathf.Pow(2f, Mathf.RoundToInt(numberOfSamples)));

		ClearVisualizer();

		float overrideValue;

		if (overrideSamples == true)
		{
			overrideValue = overrideNumber;
			//Debug.Log("overwritten: " + overrideValue);
		}
		else
        {
			overrideValue = convertedSamples;
			//Debug.Log("NOT overwritten: " + overrideValue);
		}

		float samplesScales = visualizerSpan / overrideValue;

		for (int i = 0; i < overrideValue; i++)
		//for (int i = 0; i < 170; i++)
		{
			//Debug.Log("ACTUAL override: " + overrideValue);
			//Debug.Log(i);
			var instantiatedObj = Instantiate(visualizerObj, new Vector3((samplesScales * i) - (visualizerSpan/2) + transform.position.x, transform.position.y, transform.position.z), new Quaternion(0,0,0,0));
			instantiatedObj.transform.localScale = new Vector3(samplesScales, 1, 1);
			instantiatedObj.transform.parent = transform;
			audioSpectrumObjects.Add(instantiatedObj.transform);
		}

		for (int j = 0; j < overrideValue; j++)
		{
			correctedSpectrum.Add(0);
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
		correctedSpectrum.Clear();
	}

    void Update()
	{
		int overrideValue;

		if (overrideSamples == true)
		{
			overrideValue = convertedSamples / overrideNumber;
			//Debug.Log("overwritten: " + overrideValue);
		}
		else
		{
			overrideValue = convertedSamples;
			//Debug.Log("NOT overwritten: " + overrideValue);
		}

		// initialize our float array
		float[] spectrum = new float[convertedSamples];

		// populate array with fequency spectrum data
		GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, fftWindow);

		// loop over audioSpectrumObjects and modify according to fequency spectrum data
		// this loop matches the Array element to an object on a One-to-One basis.

		//I could solve the issue by making the i skip numbers by adding more than just 1 at a time ***********
		//The second possible solution would be dividing the number of frequencies captured by number of objs, making htat into an array with the averages, and using it as a base

		for (int j = 0; j < convertedSamples; j++)
		{
			float intensity;

			//checks to see if the current number is a multiple of overrideValue, and if so, then apply the scale change, otherwise leave it 0
			if (j % overrideValue == 0)
            {
				intensity = spectrum[j] * heightMultiplier;
			}
			else
            {
				intensity = 0;
			}

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