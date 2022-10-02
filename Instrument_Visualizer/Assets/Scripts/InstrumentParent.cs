using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class InstrumentParent : MonoBehaviour
{
    #region Instruments
    //the children of this obj are the instruments
    [HideInInspector] public List<GameObject> instruments;
    //the average intensity of output in each instrument
    [HideInInspector] public List<float> instrumentIntensity;
    private Color sumOfInstrumentColors;
    #endregion

    #region Sky Shader
    [Header("Sky Shader")] [Space]
    [Tooltip("the material which contains the skybox shader")]
    public Material skyShader;
    [Tooltip("the positions from which the skybox shader moves between")]
    public List<Vector2> targetPositions;
    [Tooltip("the speed at which the above movement occurs")]
    public float movementSpeed = 0.005f;
    private float sumOfFreqLerps;
    private float sumOfSpeedLerps;
    #endregion

    #region Events System
    [Space]
    //the event system here works the same way as in each individual instrument. The difference is that these events
    //will ONLY trigger when all the instruments simultaniously reach a certain level of output, in unison, as opposed to separately

    //the list of events
    public List<UnityEvent> ParentEvents;
    //the level of input that is required to activate the events
    [Range(0,1)] public float eventFreqTriggerParent = 0.3f;
    //ensures that one event won't trigger twice, and that it waits until one event is done at a time
    private bool canTriggerEventsParent = true;
    //randomly picks one event as opposed to all at once
    public bool randomlyPickOneEventParent = false;

    //the post processing variables that are used to triggered post process events, while also checking to make sure that one event is only played at a time
    private ChromaticAberration chrAb;
    private bool chrAbIsRunning = false;
    private LensDistortion lenDis;
    private bool lenDisIsRunning = false;
    #endregion

    void Start()
    {
        //the intensities need to be cleared so that they don't stack with the previous iterations
        instrumentIntensity.Clear();

        //populate the intensities and instruments lists with the number of children
        for (int i = 0; i < transform.childCount; i++)
        {
            instrumentIntensity.Add(0);
            instruments.Add(transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        //preemptively establishing variables for later
        Color colAv = Color.black;
        float freqAv = 0;
        float speedAv = 0;
        int numOfIterations = 0;
        float highestFreq = 0;
        int targetFreqChild = 0;

        //repeats this loop for each child/instrument
        for (int i = 0; i < transform.childCount; i++)
        {
            //the usual averaging techinque to average, respectively, the colors, frequency and speed provided by the children
            colAv += instruments[i].GetComponent<AudioVisualizer5>().colorAverage;
            freqAv += instruments[i].GetComponent<AudioVisualizer5>().shaderFrequencyLerp;
            speedAv += instruments[i].GetComponent<AudioVisualizer5>().shaderSpeedLerp;

            //this condition tests to see which of the children's output is greatest each frame
            if (instrumentIntensity[i] > highestFreq)
            {
                //the child with the greatest output provides its sibling index, which tells the shader what position it should lerp towards from the list of positions
                highestFreq = instrumentIntensity[i];
                targetFreqChild = i;
            }

            //keeps track of how many times the loop has been executed
            numOfIterations++;
        }

        //lerping ensures that the shader movement is smooth from one position to the next, bot in the x and y axis
        float lerpTargX = Mathf.Lerp(skyShader.GetVector("_Dir").x, targetPositions[targetFreqChild].x, movementSpeed);
        float lerpTargY = Mathf.Lerp(skyShader.GetVector("_Dir").y, targetPositions[targetFreqChild].y, movementSpeed);
        //using these lerps, this sends back the results to the shader within the material
        skyShader.SetVector("_Dir", new Vector4(lerpTargX, lerpTargY, 0,0));

        //averages the sums by the number of iterations/children/instruments
        sumOfInstrumentColors = colAv / numOfIterations;
        sumOfFreqLerps = freqAv / numOfIterations;
        sumOfSpeedLerps = speedAv / numOfIterations;
        //then sets all of those averages into the shader
        skyShader.SetColor("_ColorAverage", sumOfInstrumentColors);
        skyShader.SetFloat("_Frequency", sumOfFreqLerps);
        skyShader.SetFloat("_Speed", sumOfSpeedLerps);

        //the loop below keeps track to see if the parent event is allowed to trigger, but it has a caveat of difference...
        //this loop HAS to be self contained, cant be together with the other one looping through the children because if it gets to the "break" function,
        //it confilcts with the things that are running with the other methods
        for (int j = 0; j < instrumentIntensity.Count; j++)
        {
            //if the current child's output is sufficient, but it is not the last child in the list, continue
            if (instrumentIntensity[j] >= eventFreqTriggerParent && j != instrumentIntensity.Count - 1)
            {
                continue;
            }
            //if the current child's output is sufficient, AND it is the last child in the list, then it sends the signal to trigger the event
            else if (instrumentIntensity[j] >= eventFreqTriggerParent && j == instrumentIntensity.Count - 1)
            {
                if (canTriggerEventsParent == true)
                {
                    TriggerInstrumentEventParent();
                }
            }
            //once the event has activated, it will eventually be deactivated by this condition 
            else if (instrumentIntensity[j] <= eventFreqTriggerParent - 0.1f && j == instrumentIntensity.Count - 1)
            {
                canTriggerEventsParent = true;
            }
            //if the current child's output is NOT sufficient, then it breaks the loop and no event is triggered
            else
            {
                break;
            }
        }
    }

    private void TriggerInstrumentEventParent()
    {
        //as soon as an event is triggered, this bool acts as a gate to prevent that the event keeps getting triggered multiple times
        canTriggerEventsParent = false;

        //just like for each instrument, if random is false, all events are triggered, if yes, then only one is picked at random
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
        //made public so it can be accessed by the events system
        if (chrAbIsRunning == false) StartCoroutine(ChromaticAberrationRise());
    }

    IEnumerator ChromaticAberrationRise()
    {
        //lets the event system know that this coroutine is running
        chrAbIsRunning = true;

        //collects the desired post process override
        GetComponent<PostProcessVolume>().profile.TryGetSettings(out chrAb);

        //lerps the intensity of the chromatic aberration of the volume so it smoothly grows
        chrAb.intensity.value = Mathf.Lerp(chrAb.intensity.value, 1, 0.1f);

        //waits for a frame to repeat this process
        yield return new WaitForSeconds(0.1f);

        //if the intensity has reached its top limit, then it stops growing and begins the coroutine to shrink instead, otherwise, keep growing
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

        //lerps the intensity of the chromatic aberration of the volume so it smoothly shrinks
        chrAb.intensity.value = Mathf.Lerp(chrAb.intensity.value, 0, 0.1f);

        //waits for a frame to repeat this process
        yield return new WaitForSeconds(0.1f);

        //if the intensity has reached its low limit, then it stops shrinking and stops the repetition of the coroutines, otherwise, keep shrinking
        if (chrAb.intensity.value >= 0.05f)
        {
            StartCoroutine(ChromaticAberrationFall());
        }
        else
        {
            chrAbIsRunning = false;
            canTriggerEventsParent = true;
        }
    }

    public void LensDistortionEvent()
    {
        //made public so it can be accessed by the events system
        if (lenDisIsRunning == false) StartCoroutine(LensDistortionRise());
    }

    IEnumerator LensDistortionRise()
    {
        //lets the event system know that this coroutine is running
        lenDisIsRunning = true;

        //collects the desired post process override
        GetComponent<PostProcessVolume>().profile.TryGetSettings(out lenDis);

        //lerps the intensity of the lens distortion of the volume so it smoothly grows
        lenDis.intensity.value = Mathf.Lerp(lenDis.intensity.value, 100, 0.2f);

        //waits for a frame to repeat this process
        yield return new WaitForSeconds(0.1f);

        //if the intensity has reached its top limit, then it stops growing and begins shrinking, otherwise, keep growing
        if (lenDis.intensity.value <= 99f)
        {
            StartCoroutine(LensDistortionRise());
        }
        else
        {
            StartCoroutine(LensDistortionFall());
        }
    }

    IEnumerator LensDistortionFall()
    {
        GetComponent<PostProcessVolume>().profile.TryGetSettings(out lenDis);

        //lerps the intensity of the lens distortion of the volume so it smoothly shrinks
        lenDis.intensity.value = Mathf.Lerp(lenDis.intensity.value, 0, 0.2f);

        //waits for a frame to repeat this process
        yield return new WaitForSeconds(0.1f);

        //if the intensity has reached its low limit, then it stops shrinking and stops the repetition of the coroutines, otherwise, keep shrinking
        if (lenDis.intensity.value >= 2f)
        {
            StartCoroutine(LensDistortionFall());
        }
        else
        {
            lenDisIsRunning = false;
            canTriggerEventsParent = true;
        }
    }
}
