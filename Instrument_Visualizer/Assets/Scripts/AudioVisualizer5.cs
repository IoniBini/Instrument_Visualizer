using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer5 : MonoBehaviour
{
    public bool generateObjsOnSpawn = true;
    public float visualizerSpan = 10;
    public GameObject visualizerObj;

    public float[] spectrum;
    //[Range(0.01f, 0.5f)] public float lerpTime = 0.01f;

    public BandParameters[] bandParameters;

    [System.Serializable]
    public struct BandParameters
    {
        public Vector2 bandRange;
        [Min(0.1f)] public float heightMultiplier;
        [Range(0.01f, 0.5f)] public float lerpTime;
        public Gradient colorOverFrequency;
        [Min(1)] public float colorFrequencyMultiplier;
        [Min(1)] public float emissionMultiplier;
        public AnimationCurve emissionIntensityCurve;
    }

    private void Start()
    {
        if (generateObjsOnSpawn == true)
        GenerateVisualizerObjs();
    }

    [ContextMenu("Generate Visualizer Objs")]
    public void GenerateVisualizerObjs()
    {
        ClearVisualizer();

        float samplesScales = visualizerSpan / bandParameters.Length;

        for (int i = 0; i < bandParameters.Length; i++)
        {
            //Debug.Log("test");
            var instantiatedObj = Instantiate(visualizerObj, new Vector3((samplesScales * i) - (visualizerSpan / 2) + transform.position.x, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0));
            instantiatedObj.transform.localScale = new Vector3(samplesScales, 1, 1);
            instantiatedObj.transform.parent = transform;
            instantiatedObj.name = "Band " + i;
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

            if (Application.isPlaying == true)
            {
                Destroy(currentObj);
            }
            else
            {
                DestroyImmediate(currentObj);
            }
        }
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

        //Debug.Log(transform.childCount);

        for (int j = 0; transform.childCount > j; j++)
        {
            for (int i = 0; spectrum.Length > i; i++)
            {
                if (i >= bandParameters[j].bandRange.x && i < bandParameters[j].bandRange.y)
                {
                    //Debug.Log(i + " > " + bandParameters[j].bandRange.x + " & " + i + " < " + bandParameters[j].bandRange.y);

                    currentValue += spectrum[i];
                    numberOfFrequencies++;
                }
            }

            float average = (currentValue / numberOfFrequencies) * bandParameters[j].heightMultiplier;
            float lerpY = Mathf.Lerp(transform.GetChild(j).localScale.y, average, bandParameters[j].lerpTime);
            transform.GetChild(j).localScale = new Vector3(transform.GetChild(j).localScale.x, lerpY, transform.localScale.z);

            var target = transform.GetChild(j).GetComponent<Renderer>();
            var propertyBlock = new MaterialPropertyBlock();

            float colorLerp = Mathf.InverseLerp(0, spectrum.Length, lerpY * bandParameters[j].colorFrequencyMultiplier);
            float emissionLerp = Mathf.InverseLerp(0, spectrum.Length, lerpY * bandParameters[j].emissionMultiplier);

            propertyBlock.SetColor("_Color", bandParameters[j].colorOverFrequency.Evaluate(colorLerp) * bandParameters[j].emissionIntensityCurve.Evaluate(emissionLerp));

            target.SetPropertyBlock(propertyBlock);
        }
    }
}
