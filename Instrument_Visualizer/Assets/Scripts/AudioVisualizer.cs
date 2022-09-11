using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioVisualizer : MonoBehaviour
{
	public List<Transform> audioSpectrumObjects = new List<Transform>();
	[Min(1)] public float heightMultiplier;
	[Range(6, 13)] [Tooltip("This number is powered by 2, resulting in a multiple that fits between 64 and 8192")]
	public int numberOfSamples = 6;
	[Tooltip("When false, the number of cubes generated = number of samples, when false, you can choose whatever number you want in overrideNumber")]
	public bool overrideSamples = false;
	[Min(1)] 
	public int overrideNumber = 1024;
	private int convertedSamples;
	public float visualizerSpan = 21f;
	public FFTWindow fftWindow;
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
		audioSpectrumObjects.Clear();
		GenerateVisualizerObjs();

		Transform currentObj;

		for (int i = 0; i < transform.childCount; i ++)
        {
			currentObj = transform.GetChild(i);

			audioSpectrumObjects.Add(currentObj);
		}
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
		}
		else
        {
			overrideValue = convertedSamples;
		}

		float samplesScales = visualizerSpan / overrideValue;

		for (int i = 0; i < overrideValue; i++)
		//for (int i = 0; i < 170; i++)
		{
			var instantiatedObj = Instantiate(visualizerObj, new Vector3((samplesScales * i) - (visualizerSpan/2) + transform.position.x, transform.position.y, transform.position.z), new Quaternion(0,0,0,0));
			instantiatedObj.transform.parent = transform;
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
		int overrideValue;

		if (overrideSamples == true)
		{
			overrideValue = overrideNumber;
		}
		else
		{
			overrideValue = convertedSamples;
		}

		// initialize our float array
		float[] spectrum = new float[convertedSamples];

		// populate array with fequency spectrum data
		GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, fftWindow);

		// loop over audioSpectrumObjects and modify according to fequency spectrum data
		// this loop matches the Array element to an object on a One-to-One basis.
		for (int i = 0; i < overrideValue; i++)
		//for (int i = 0; i < 170; i++)
		{
			// apply height multiplier to intensity
			float intensity = spectrum[i] * heightMultiplier;

			// calculate object's scale
			float lerpY = Mathf.Lerp(audioSpectrumObjects[i].localScale.y, intensity, lerpTime);
			Vector3 newScale = new Vector3(audioSpectrumObjects[i].localScale.x, lerpY, audioSpectrumObjects[i].localScale.z);

			// appply new scale to object
			audioSpectrumObjects[i].localScale = newScale;

		}
	}

}