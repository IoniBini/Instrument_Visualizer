using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    [SerializeField] private int _band;
    public AudioVisualizer3 parent;

    void Start()
    {
        _band = transform.GetSiblingIndex();
        parent = GetComponentInParent<AudioVisualizer3>();
    }

    void Update()
    {
        float intensity = parent._freqBand[_band] * parent.heightMultiplier;
        float lerpY = Mathf.Lerp(transform.localScale.y, intensity, parent.lerpTime);
        Vector3 newScale = new Vector3(transform.localScale.x, lerpY, transform.localScale.z);

        transform.localScale = new Vector3(transform.localScale.x, (newScale.y), transform.localScale.z);
    }
}
