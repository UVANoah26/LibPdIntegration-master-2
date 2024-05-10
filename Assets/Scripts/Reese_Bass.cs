using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reese_Bass : MonoBehaviour
{
    [SerializeField]
    ClockControl clock;

    [SerializeField]
    public LibPdInstance patch;

    [SerializeField]
    public Material SHADER;

    Color[] Colors = new Color[] {
        new Color(.878f,.078f,.541f), //pink
        new Color(.878f,.639f,.082f), //yellow
        new Color(.878f,.094f,.094f), //red
        new Color(.671f,.082f,.878f) //purple
    };

    Color[] TileCol = new Color[] {
       new Color(0.094f, 0.035f, 0.082f), // paired with pink
       new Color(0.17f, 0.15f, 0.06f), // paired with yellow
       new Color(0.22f, 0.1f, 0.1f), // paired with red
       new Color(0.11f, 0.06f, 0.16f)  // paired with purple
    };



    int ramp;
    float t;

    [SerializeField]
    int beat;

    [SerializeField]
    [Range(0f, 1f)]
    float Reverb;

    int Divide;

    int Counter = 0;
    int index = 0;


    [Range(0, 1000)] [SerializeField] int A;

    [Range(0, 1000)] [SerializeField] int D;

    [Range(0f, 1f)] [SerializeField] float S;

    [Range(0, 1000)] [SerializeField] int R;

    //26 28 29 31 34

    int[] Notes = new int[] {7, 10, 5, 6};


    // Start is called before the first frame update
    void Start()
    {
        SHADER.SetColor("_GridColor", Colors[0]);
        SHADER.SetColor("_TileColor", TileCol[0]);

    }

    // Update is called once per frame
    void Update()
    {
        if(clock.isOn[0])
            Divide = 4;
        if (clock.isOn[1])
            Divide = 12;
        if (clock.isOn[2])
        {
            Divide = 8;
            Notes = new int[] { 5, 10, 7, 6 };
        }
        t += Time.deltaTime;
        int MS = Mathf.RoundToInt(Time.deltaTime * 1000);

        bool trig = ramp > (ramp + MS) % beat;
        ramp = (ramp + MS) % beat;


        if (trig) {
            Counter++;
            if (Counter % Divide == 0)
            {
                index++;
                SHADER.SetColor("_GridColor",Colors[index%4]);
                SHADER.SetColor("_TileColor", TileCol[index % 4]);
            }
            patch.SendMidiNoteOn(17,Notes[index%4]+32,20);
        }



        //ADSR Management
        Vector4 adsr_par = new Vector4(A, D, S, R);
        float gate_len = clock.beatMs / 2;
        patch.SendList("ADSR", gate_len, A, D, S, R);
        bool gate = clock.ramp < gate_len;
        float ADSR = ControlFunctions.ADSR(clock.ramp, gate, adsr_par);

        //patch.SendFloat("reverb", Reverb);

    }
}
