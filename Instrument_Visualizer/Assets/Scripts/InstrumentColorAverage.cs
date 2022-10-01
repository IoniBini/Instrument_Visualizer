using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentColorAverage : MonoBehaviour
{
    public List<GameObject> instrumentColors;
    public Material skyShader;
    [SerializeField] private Color sumOfInstrumentColors;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            instrumentColors.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color colAv = Color.black;
        int numOfCols = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
             colAv += instrumentColors[i].GetComponent<AudioVisualizer5>().colorAverage;
            numOfCols++;
        }

        sumOfInstrumentColors = colAv / numOfCols;

        skyShader.SetColor("_ColorAverage", sumOfInstrumentColors);
    }
}
