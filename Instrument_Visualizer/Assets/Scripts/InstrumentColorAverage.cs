using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentColorAverage : MonoBehaviour
{
    public List<GameObject> instruments;
    public Material skyShader;

    [HideInInspector] public List<float> bandIntensity;
    public List<Vector2> targetPositions;
    public float movementSpeed = 0.005f;

    private float sumOfFreqLerps;
    private float sumOfSpeedLerps;
    private Color sumOfInstrumentColors;

    void Start()
    {
        bandIntensity.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            bandIntensity.Add(0);
            instruments.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color colAv = Color.black;
        float freqAv = 0;
        float speedAv = 0;
        int numOfCols = 0;

        float highestFreq = 0;
        int targetFreqChild = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            colAv += instruments[i].GetComponent<AudioVisualizer5>().colorAverage;
            freqAv += instruments[i].GetComponent<AudioVisualizer5>().shaderFrequencyLerp;
            speedAv += instruments[i].GetComponent<AudioVisualizer5>().shaderSpeedLerp;

            if (bandIntensity[i] > highestFreq)
            {
                highestFreq = bandIntensity[i];
                targetFreqChild = i;
            }

            numOfCols++;
        }

        float lerpTargX = Mathf.Lerp(skyShader.GetVector("_Dir").x, targetPositions[targetFreqChild].x, movementSpeed);
        float lerpTargY = Mathf.Lerp(skyShader.GetVector("_Dir").y, targetPositions[targetFreqChild].y, movementSpeed);

        skyShader.SetVector("_Dir", new Vector4(lerpTargX, lerpTargY, 0,0));

        sumOfInstrumentColors = colAv / numOfCols;
        sumOfFreqLerps = freqAv / numOfCols;
        sumOfSpeedLerps = speedAv / numOfCols;

        skyShader.SetColor("_ColorAverage", sumOfInstrumentColors);
        skyShader.SetFloat("_Frequency", sumOfFreqLerps);
        skyShader.SetFloat("_Speed", sumOfSpeedLerps);
    }
}
