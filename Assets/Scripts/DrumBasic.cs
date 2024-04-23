using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumBasic : MonoBehaviour
{
    public LibPdInstance patch;
    float ramp;
    float t;
    int[] mode;
    int count1 = 0;
    int count2 = 0;
    int count3 = 0;

    [SerializeField]
    List<bool> kick;

    [SerializeField]
    List<bool> snare;

    [SerializeField]
    List<bool> sticks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        bool trig = ramp > (ramp + Time.deltaTime) % 1;
        ramp = (ramp + Time.deltaTime) % 1;

        if (trig)
        {
            if (kick[count1])
            {
                patch.SendBang("kick_bang");
            }
            if (snare[count2])
            {
                patch.SendBang("snare_bang");
            }
            if (sticks[count3])
            {
                patch.SendBang("sticks_bang");
            }
            count1 = (count1 + 1) % kick.Count;
            count2 = (count2 + 1) % snare.Count;
            count3 = (count3 + 1) % sticks.Count;
        }

    }
}
