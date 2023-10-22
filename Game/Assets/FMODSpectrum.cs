using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSpectrum : MonoBehaviour
{
    [SerializeField]
    SimpleSpectrum spect;
    [SerializeField]
    FMODBGM source;
    // Start is called before the first frame update
    void Start()
    {
        var system = FMODWrapper.GetSystem();
    }

    // Update is called once per frame
    void Update()
    {
        spect.spectrumInputData = source.GetSpectrumData();
    }
}
