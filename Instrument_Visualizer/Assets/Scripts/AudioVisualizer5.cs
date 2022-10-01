using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer5 : MonoBehaviour
{
    [HideInInspector] public List<Color> currentColor;
    [HideInInspector] public Color colorAverage;

    [HideInInspector] public float shaderFrequencyLerp;
    [HideInInspector] public float shaderSpeedLerp;

    public float maxShaderPositionLerp = 10;

    public AnimationCurve shaderFrequencyMultiplier;
    public float shaderSpeedMultiplier = 2;

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
        public float frequencyGate;
        public float frequencyGateIntensity;
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
            currentColor.Add(Color.black);
        }
    }

    [ContextMenu("Clear Visualizer Objs")]
    public void ClearVisualizer()
    {
        currentColor.Clear();

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

        float instrumentIntensity = 0;
        float currentValue = 0;
        int numberOfFrequencies = 0;
        int currentColorCount = 0;

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
                        //the gate is responsible for filtering dead frequencies that constantly get picked up but that are not loud enough to be audible. This happens more often with
                        //microphone capture than instruments, because you can't efficiently stop capturing all the frequencies of a microphone like you could an instrument by simply
                        //stop playing

                        if(bandParameters[j].frequencyGate > spectrum[i])
                        {
                            currentValue += spectrum[i] / bandParameters[j].frequencyGateIntensity;
                            numberOfFrequencies++;
                        }
                        else
                        {
                            currentValue += spectrum[i];
                            numberOfFrequencies++;
                        }
                    }
                }
                
            }

            float average = (currentValue / numberOfFrequencies) * bandParameters[j].heightMultiplier;
            instrumentIntensity += average;

            shaderFrequencyLerp = Mathf.Lerp(GetComponentInParent<InstrumentColorAverage>().skyShader.GetFloat("_Frequency"), shaderFrequencyMultiplier.Evaluate(average), bandParameters[j].lerpTime);
            shaderSpeedLerp = Mathf.Lerp(GetComponentInParent<InstrumentColorAverage>().skyShader.GetFloat("_Speed"), average * shaderSpeedMultiplier, bandParameters[j].lerpTime);

            float lerpY = Mathf.Lerp(transform.GetChild(j).localScale.y, average, bandParameters[j].lerpTime);

            transform.GetChild(j).localScale = new Vector3(transform.GetChild(j).localScale.x, lerpY, transform.localScale.z);

            var target = transform.GetChild(j).GetComponent<Renderer>();
            var propertyBlock = new MaterialPropertyBlock();

            float colorLerp = Mathf.InverseLerp(0, spectrum.Length, lerpY * bandParameters[j].colorFrequencyMultiplier);
            float emissionLerp = Mathf.InverseLerp(0, spectrum.Length, lerpY * bandParameters[j].emissionMultiplier);

            propertyBlock.SetColor("_Color", bandParameters[j].colorOverFrequency.Evaluate(colorLerp) * bandParameters[j].emissionIntensityCurve.Evaluate(emissionLerp));
            currentColor[j] = bandParameters[j].colorOverFrequency.Evaluate(colorLerp) * bandParameters[j].emissionIntensityCurve.Evaluate(emissionLerp);
            
            target.SetPropertyBlock(propertyBlock);

            colorAverage += currentColor[j];
            currentColorCount++;
        }

        colorAverage /= currentColorCount;

        float instrumentIntensityAveraged = instrumentIntensity / currentColorCount;

        GetComponentInParent<InstrumentColorAverage>().bandIntensity[transform.GetSiblingIndex()] = Mathf.InverseLerp(GetComponentInParent<InstrumentColorAverage>().bandIntensity[transform.GetSiblingIndex()], maxShaderPositionLerp, instrumentIntensityAveraged);
    }
}
