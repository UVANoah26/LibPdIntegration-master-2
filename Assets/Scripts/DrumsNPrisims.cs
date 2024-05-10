using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumsNPrisims : MonoBehaviour
{
    List<bool> Steps = new List<bool> { true, false, true, false };
    [SerializeField]
    float x; //X-value of the top left corner of the Array of Cubes
    [SerializeField]
    float z; //Z-value of the top left corner of the Array of Cubes
    [SerializeField]
    float Dim; //Scale of the Cubes

    GameObject[][] Prisms;
    Color[][] ObjMats;


    [SerializeField]
    public LibPdInstance patch;

    [SerializeField]
    ClockControl clock;

    int ramp;
    float t;


    [SerializeField]
    int beat;

    //DRUM SEQ
    int count = 0;
    int Counter = 0;


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

    //Vector4 adsr_params;

    Color Shade = new Color(1, 1, 1);

    List<bool> Kick0 = new List<bool> { false, false, false, false };
    List<bool> Snare0 = new List<bool> { false, true, true, false };
    List<bool> Sticks0 = new List<bool> { false, false, false, false };

    List<bool> Kick1 = new List<bool> { true, false, false, false };
    List<bool> Snare1 = new List<bool> { false, true, true, false };
    List<bool> Sticks1 = new List<bool> { false, false, false, false };

    List<bool> Kick2 = new List<bool> { true, false, true, false };
    List<bool> Snare2 = new List<bool> { false, true, true, true };
    List<bool> Sticks2 = new List<bool> { false, false, false, false };


    // Start is called before the first frame update
    void Start()
    {

        Prisms = new GameObject[Steps.Count][];
        ObjMats = new Color[Steps.Count][];

        for (int i = 0; i < Steps.Count; i++)
        {
            Prisms[i] = new GameObject[Steps.Count];
            ObjMats[i] = new Color[Steps.Count];

            for (int j = 0; j < Steps.Count; j++)
            {
                Prisms[i][j] = GameObject.CreatePrimitive(PrimitiveType.Cube); //Create a cube
                Prisms[i][j].transform.position = new Vector3(x - (j * Dim), -10, z - (i * Dim)); //Move it to its spot in the array of cubes
                Prisms[i][j].transform.localScale = new Vector3(Dim, 1, Dim); //Scale it
                ObjMats[i][j] = Prisms[i][j].GetComponent<Renderer>().material.color = new Color(0.113f, 0.042f, 0.098f);
                //(.094f, .035f, .082f)

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
        //adsr_params = new Vector4(100, 50, 0.4f, 200);

    }

    // Update is called once per frame
    void Update()
    {
        if (clock.isOn[0])
        {
            kick = Kick0;
            snare = Snare0;
            sticks = Sticks0;
        }
        else if (clock.isOn[1])
        {
            kick = Kick1;
            snare = Snare1;
            sticks = Sticks1;
        }
        else if (clock.isOn[2])
        {
            kick = Kick2;
            snare = Snare2;
            sticks = Sticks2;
        }


        //Behind the curtain lfo and triggers
        t += Time.deltaTime;
        int MS = Mathf.RoundToInt(Time.deltaTime * 1000);


        bool trig = ramp > (ramp + MS) % beat;

        ramp = (ramp + MS) % beat;
        if (trig)
        {


            if (kick[Counter])
            {

                patch.SendBang("kick_bang");
                Shade = new Color(.7f, .7f, 1.0f);
                for (int i = 0; i < Steps.Count; i++)
                {
                    for (int j = 0; j < Steps.Count; j++)
                    {
                        Prisms[i][j].transform.localScale = new Vector3(Dim, Dim, Dim);

                    }
                }
            }
            if (snare[Counter])
                {
                    
                    patch.SendBang("snare_bang");
                    for (int i = 0; i < 2; i++)
                    {
                        int Num = Random.Range(0, 4);
                        int Num2 = Random.Range(0, 4);
                        int Num3 = Random.Range(0, 6);
                        Prisms[Num][Num2].transform.localScale = new Vector3(Dim, Dim + Num3, Dim);
                    }
                }

             if (sticks[Counter])
                {
                    patch.SendBang("sticks_bang");
                    Shade = new Color(1.0f, .7f, .7f);
                }

                Counter = (Counter + 1) % Steps.Capacity;
       

            }
        }
    }
