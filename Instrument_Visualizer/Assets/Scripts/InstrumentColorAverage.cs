using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentColorAverage : MonoBehaviour
{
    public List<GameObject> instruments;
    public Material skyShader;
    [SerializeField] private float sumOfFreqLerps;
    [SerializeField] private float sumOfSpeedLerps;
    [SerializeField] private Color sumOfInstrumentColors;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
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

        for (int i = 0; i < transform.childCount; i++)
        {
            colAv += instruments[i].GetComponent<AudioVisualizer5>().colorAverage;
            freqAv += instruments[i].GetComponent<AudioVisualizer5>().shaderFrequencyLerp;
            speedAv += instruments[i].GetComponent<AudioVisualizer5>().shaderSpeedLerp;
            numOfCols++;
        }

        sumOfInstrumentColors = colAv / numOfCols;
        sumOfFreqLerps = freqAv / numOfCols;
        sumOfSpeedLerps = speedAv / numOfCols;

        skyShader.SetColor("_ColorAverage", sumOfInstrumentColors);
        skyShader.SetFloat("_Frequency", sumOfFreqLerps);
        skyShader.SetFloat("_Speed", sumOfSpeedLerps);
    }
}
