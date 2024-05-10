using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockControl : MonoBehaviour
{
    [SerializeField]
    public GameObject Fog;

    [HideInInspector]
    public float t;
    [HideInInspector]
    public bool beatTrig;
    [SerializeField]
    public int beatMs = 300;
    [SerializeField]

    public int timeSignature = 4;
    [HideInInspector]
    public int counterBeatUnbound;
    [HideInInspector]
    public int counterBeat;
    [HideInInspector]
    public int counterMeasure;
    [HideInInspector]
    public float ramp;
    [SerializeField]
    List<int> sections;

    [SerializeField]
    int sect1, sect2, sect3;

    [HideInInspector]
    public bool[] isOn;

    int Latch = 0;


    // Start is called before the first frame update
    void Start()
    {
        sect1 = sections[0];
        sect2 = sections[0]+sections[1];
        sect3 = sections[2]+sect2;
        isOn = new bool[sections.Count];
      
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        int dMs = Mathf.RoundToInt(Time.deltaTime * beatMs);
        bool beatTrig = ramp > ((ramp + dMs) % beatMs);
        ramp = (ramp + dMs) % beatMs;

        if (beatTrig) counterBeatUnbound += 1;
        counterBeat = counterBeatUnbound % 4;
        counterMeasure = counterBeatUnbound / timeSignature;

        if (counterMeasure < sect1) {
            isOn[0] = true;
            isOn[1] = false;
            isOn[2] = false;
        }
        else if (counterMeasure < sect2)
        {
            if (Latch == 0)
            {
                Latch++;
                Fog = Instantiate(Fog);
                Fog.SetActive(true);
            }
            isOn[0] = false;
            isOn[1] = true;
            isOn[2] = false;
        }
        else if (counterMeasure < sect3)
        {
            isOn[0] = false;
            isOn[1] = false;
            isOn[2] = true;
        }


        //Debug.Log("beat " + counterBeat);
        //Debug.Log("measure " + counterMeasure);
    }
}
