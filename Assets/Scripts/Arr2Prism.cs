using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arr2Prism : MonoBehaviour
{


   
    [SerializeField]
    ClockControl Clock;
    [SerializeField]
    float x; //X-value of the top left corner of the Array of Cubes
    [SerializeField]
    float z; //Z-value of the top left corner of the Array of Cubes
    [SerializeField]
    float Dim; //Scale of the Cubes

    [SerializeField]
    List<bool> Steps;

    GameObject[][] StepsObjs;

    [SerializeField]
    public LibPdInstance patch;
    int ramp;
    float t;

    [SerializeField]
    int beat;

    int[] mode;

    int Counter;


    [Range(0, 1000)] [SerializeField] int A;

    [Range(0, 1000)] [SerializeField] int D;

    [Range(0f, 1f)] [SerializeField] float S;

    [Range(0, 1000)] [SerializeField] int R;

    Material[][] ObjMats; //My Array for changing color


    //DRUM SEQ
    int count = 0;
    

    [SerializeField]
    List<bool> kick;

    [SerializeField]
    List<bool> snare;

    [SerializeField]
    List<bool> sticks;

    public List<AudioClip> sounds;

    string[] drum_type = new string[] { "Kick", "Snare", "Sticks" };
    List<float> envelopes;

    List<bool>[] gates = new List<bool>[3];

    Vector4 adsr_params;

    Color Shade = new Color(1,1,1);


    List<bool> Kick0 = new List<bool> {false,false,false,false};
    List<bool> Snare0 = new List<bool> { false, true, true, false };
    List<bool> Sticks0 = new List<bool> { false, false, false, false };

    List<bool> Kick1 = new List<bool> { false, false, false, false };
    List<bool> Snare1 = new List<bool> { false, true, true, false };
    List<bool> Sticks1 = new List<bool> { true, true, true, true };

    List<bool> Kick2 = new List<bool> { true, false, false, false };
    List<bool> Snare2 = new List<bool> { false, true, true, false };
    List<bool> Sticks2 = new List<bool> { true, true, true, true };

    // Start is called before the first frame update
    void Start()
    {
        mode = new int[] { 2, 2, 1, 2, 2, 1 };

        StepsObjs = new GameObject[Steps.Count][];
        ObjMats = new Material[Steps.Count][];
        for (int i = 0; i < Steps.Count; i++)
        {
            StepsObjs[i] = new GameObject[Steps.Count];
            ObjMats[i] = new Material[Steps.Count];

            for (int j = 0; j < Steps.Count; j++)
            {
                StepsObjs[i][j] = GameObject.CreatePrimitive(PrimitiveType.Cube); //Create a cube
                StepsObjs[i][j].transform.position = new Vector3(x + (j * Dim), -5, z - (i * Dim)); //Move it to its spot in the array of cubes
                StepsObjs[i][j].transform.localScale = new Vector3(Dim, 1, Dim); //Scale it
                ObjMats[i][j] = StepsObjs[i][j].GetComponent<Renderer>().material; //Store its material in the materials array

            }
        }
        //DRUM SEQ
        envelopes = new List<float>();
        for (int i = 0; i < sounds.Count; i++)
        {
            //send sound files names to patch
            //add .wav
            //drum type is both the name of receive obj 
            //and of Drums folder subdirectory for sound
            string name = sounds[i].name + ".wav";
            patch.SendSymbol(drum_type[i], name);
            //build list of envelopes
            envelopes.Add(0);
        }
        gates[0] = kick;
        gates[1] = snare;
        gates[2] = sticks;
        adsr_params = new Vector4(100, 50, 0.4f, 200);


    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Clock.isOn);
        if (Clock.isOn[0])
        {
            kick = Kick0;
            snare = Snare0;
            sticks = Sticks0;
        }
        else if(Clock.isOn[1])
        {
            kick = Kick1;
            snare = Snare1;
            sticks = Sticks1;
        }
        else if (Clock.isOn[2])
        {
            kick = Kick2;
            snare = Snare2;
            sticks = Sticks2;
        }


        //Behind the curtain lfo and triggers
        t += Time.deltaTime;
        int MS = Mathf.RoundToInt(Time.deltaTime * 1000);

        float lfo = ControlFunctions.Sin(t, 0.5f, 0);
        int[] pitchArr = ControlFunctions.PitchArray(0, new Vector2Int(24, 36), mode);
        int pitch_index = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (pitchArr.Length - 1));
        int pitch = pitchArr[pitch_index];

        int[] pitchArr2 = ControlFunctions.PitchArray(0, new Vector2Int(36, 48), mode);
        int pitch2 = pitchArr2[pitch_index];


        bool trig = ramp > (ramp + MS) % beat;
        
        ramp = (ramp + MS) % beat;
        if (trig)
        {
            if (kick[count])
            {
                patch.SendBang("kick_bang");
                Shade = new Color(.7f,.7f,1.0f);
            }
            if (snare[count])
            {
                patch.SendBang("snare_bang");
                Shade = new Color(.7f, 1.0f, .7f);
            }
            if (sticks[count])
            {
                patch.SendBang("sticks_bang");
                Shade = new Color(1.0f, .7f, .7f);
            }
            count = (count + 1) % kick.Count;

            Counter = (Counter + 1) % Steps.Capacity;
            if (Steps[Counter])
            {
                patch.SendMidiNoteOn(0, pitch, 80);
            }
            else {
                patch.SendMidiNoteOn(0, pitch2, 80);
            }
            //Moving every cube in the same collumn

       


            for (int j = 0; j < Steps.Count; j++)
            {
                int Num = Random.Range(0, 6);
                Vector3 EndScale = new Vector3(Dim, lfo * (5-Counter)+Num, Dim);
                StepsObjs[Counter][j].transform.localScale = Vector3.Lerp(StepsObjs[Counter][j].transform.localScale, EndScale, Time.deltaTime * 250);
                ObjMats[Counter][j].color = Color.Lerp(ObjMats[Counter][j].color, Shade, Time.deltaTime * 5);
                //new Color(StepsObjs[Counter][j].transform.position.y, .8f, StepsObjs[Counter][j].transform.position.y / 2) * 0.1f
            }


        }


        /*
        if (!Steps[Counter])
        {
            StepsObjs[Counter].transform.position = new Vector3(StepsObjs[Counter].transform.position.x, 0, 0);
            StepsObjs[Counter].GetComponent<Renderer>().material.color = new Color(1, 1, 1);
        }
        

        //ADSR Management
        Vector4 adsr_par = new Vector4(A, D, S, R);
        float gate_len = beat / 2;
        patch.SendList("ADSR", gate_len, A, D, S, R);
        bool gate = ramp < gate_len;
        float ADSR = ControlFunctions.ADSR(ramp, gate, adsr_par);
        */
    }
}
