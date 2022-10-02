using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class InstrumentColorAverage : MonoBehaviour
{
    public List<GameObject> instruments;
    public Material skyShader;

    [HideInInspector] public List<float> bandIntensity;
    public List<Vector2> targetPositions;
    public float movementSpeed = 0.005f;

    public List<UnityEvent> ParentEvents;
    [Range(0,1)] public float eventFreqTriggerParent = 0.3f;
    private bool canTriggerEventsParent = true;
    public bool randomlyPickOneEventParent = false;

    public ChromaticAberration chrAb;
    private bool chrAbIsRunning = false;

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

        //this loop HAS to be self contained, cant be together with the other one looping through the children because if it gets to the "break" function,
        //it confilcts with the things that are running with the other methods

        for (int j = 0; j < bandIntensity.Count; j++)
        {
            if (bandIntensity[j] >= eventFreqTriggerParent && j != bandIntensity.Count - 1)
            {
                continue;
            }
            else if (bandIntensity[j] >= eventFreqTriggerParent && j == bandIntensity.Count - 1)
            {
                if (canTriggerEventsParent == true)
                {
                    TriggerInstrumentEventParent();
                }
            }
            else if (bandIntensity[j] <= eventFreqTriggerParent - 0.1f && j == bandIntensity.Count - 1)
            {
                canTriggerEventsParent = true;
            }
            else
            {
                break;
            }
        }
    }

    private void TriggerInstrumentEventParent()
    {
        canTriggerEventsParent = false;

        if (randomlyPickOneEventParent == false)
        {
            for (int i = 0; ParentEvents.Count > i; i++)
            {
                ParentEvents[i].Invoke();
            }
        }
        else
        {
            ParentEvents[Random.Range(0, ParentEvents.Count)].Invoke();
        }
    }

    public void ChromaticAberrationEvent()
    {
        if (chrAbIsRunning == false) StartCoroutine(ChromaticAberrationRise());
    }

    IEnumerator ChromaticAberrationRise()
    {
        chrAbIsRunning = true;

        GetComponent<PostProcessVolume>().profile.TryGetSettings(out chrAb);

        chrAb.intensity.value = Mathf.Lerp(chrAb.intensity.value, 1, 0.1f);

        yield return new WaitForSeconds(0.1f);

        if (chrAb.intensity.value <= 0.9f)
        {
            StartCoroutine(ChromaticAberrationRise());
        }
        else
        {
            StartCoroutine(ChromaticAberrationFall());
        }
    }

    IEnumerator ChromaticAberrationFall()
    {
        GetComponent<PostProcessVolume>().profile.TryGetSettings(out chrAb);

        chrAb.intensity.value = Mathf.Lerp(chrAb.intensity.value, 0, 0.1f);

        yield return new WaitForSeconds(0.1f);

        if (chrAb.intensity.value >= 0.05f)
        {
            StartCoroutine(ChromaticAberrationFall());
        }
        else
        {
            chrAbIsRunning = false;
        }
    }
}
