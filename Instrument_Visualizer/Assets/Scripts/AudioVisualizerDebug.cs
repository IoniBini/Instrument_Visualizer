using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioVisualizerDebug : MonoBehaviour
{
	//https://www.youtube.com/watch?v=Ri1uNPNlaVs&t=652s
	//this script is lightly inspired by this guy's code

	[Tooltip("The audio source where this obj is getting its spectrum from (leave empty in case you want to just capture audio from this object itself)")]
	public AudioSource targetAudio;
	[Tooltip("Each individual frequency from the numberOfSamples has a corresponding obj")]
	public List<Transform> audioSpectrumObjects = new List<Transform>();
	[Tooltip("How high the frequencies grow in size")]
	[Min(1)] public float heightMultiplier;
	[Range(6, 13)] [Tooltip("This number is powered by 2, resulting in a multiple that fits between 64 and 8192")]
	public int numberOfSamples = 6;

	private int convertedSamples;
	[Tooltip("The amount of space that this object occupies in width")]
	public float visualizerSpan = 21f;
	[Tooltip("The speed at which each individual frequency grows and decays")]
	[Range(0.01f, 0.5f)]public float lerpTime = 0.01f;
	[Tooltip("The objects that are spawned for each frequency")]
	public GameObject visualizerObj;

	private void Start()
    {
		//if no target is set, it uses the object's own source, otherwise, it uses the target
		if (targetAudio == null)
        {
			targetAudio = GetComponent<AudioSource>();
			GetComponent<VoiceDetection>().enabled = true;
		}
		else
        {
			GetComponent<VoiceDetection>().enabled = false;
		}

		//clears the previous objects so the list doesn't compound with each iteration
		audioSpectrumObjects.Clear();
		//then generates a fresh batch of new objects
		GenerateVisualizerObjs();
    }

	[ContextMenu("Generate Visualizer Objs")]
	public void GenerateVisualizerObjs()
    {
		//this function multiplies 2 by 2 a number of times which will result in, of course, a multiple of 2, which we can use to capture frequencies
		convertedSamples = Mathf.FloorToInt(Mathf.Pow(2f, Mathf.RoundToInt(numberOfSamples)));

		//clears the previous frequencies so the list doesn't compound with each iteration
		ClearVisualizer();

		//the scale of each obj is equals to the width the visualizer should have divided by the number of objs being spawned
		float samplesScales = visualizerSpan / convertedSamples;

		//loops for each frequency
		for (int i = 0; i < convertedSamples; i++)
		{
			//creates an obj for each frequency
			var instantiatedObj = Instantiate(visualizerObj, new Vector3((samplesScales * i) - (visualizerSpan/2) + transform.position.x, transform.position.y, transform.position.z), new Quaternion(0,0,0,0));
			//sets the scale of the obj that was created
			instantiatedObj.transform.localScale = new Vector3(samplesScales, 1, 1);
			//makes the new obj a child of this obj
			instantiatedObj.transform.parent = transform;
			//renames the child obj to its corresponding frequency
			instantiatedObj.name = "Frequency " + (i + 1);
			//adds the obj in question to the list of objs
			audioSpectrumObjects.Add(instantiatedObj.transform);
		}
	}

	[ContextMenu("Clear Visualizer Objs")]
	public void ClearVisualizer()
    {
		GameObject currentObj;
		int childs = transform.childCount;

		//goes through each obj and deletes it
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

		//clears the list of objs
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
			//sets the frequency of each individual child to each individual corresponding obj
			float intensity = spectrum[j] * heightMultiplier;

			// calculate object's scale
			float lerpY = Mathf.Lerp(audioSpectrumObjects[j].localScale.y, intensity, lerpTime);
			Vector3 newScale = new Vector3(audioSpectrumObjects[j].localScale.x, lerpY, audioSpectrumObjects[j].localScale.z);

			// appply new scale to object
			audioSpectrumObjects[j].localScale = newScale;
		}
	}
}