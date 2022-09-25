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
        [ExecuteInEditMode] public List<Vector2> bandRange;
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

        //the first for loop is to apply it for each child
        for (int j = 0; transform.childCount > j; j++)
        {

            //the second for loop is to go over each frequency in the spectrum to collect only what is relevant
            for (int i = 0; spectrum.Length > i; i++)
            {

                //the third for loop goes through every range within the array of ranges 
                for (int w = 0; bandParameters[j].bandRange.Count > w; w++)
                {
                    //checks to see if the current frequency falls within the range proposed by the previous for loop
                    if (i >= bandParameters[j].bandRange[w].x && i < bandParameters[j].bandRange[w].y)
                    {                        
                        currentValue += spectrum[i];
                        numberOfFrequencies++;
                        
                    }
                }
                
            }

            float average = (currentValue / numberOfFrequencies) * bandParameters[j].heightMultiplier;
            float lerpY = Mathf.Lerp(transform.GetChild(j).localScale.y, average, bandParameters[j].lerpTime);

            //if (lerpY <= 0.001f) lerpY = 0.001f;

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
