using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrismArr : MonoBehaviour
{
    [SerializeField]
    float x;
    [SerializeField]
    float z;
    [SerializeField]
    float Dim;

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

    Material[][] ObjMats;


    // Start is called before the first frame update
    void Start()
    {
        mode = new int[] { 2, 2, 1, 2, 2, 1 };

        StepsObjs = new GameObject[Steps.Count][];
        ObjMats = new Material[Steps.Count][];
        for(int i = 0; i < Steps.Count; i++){
            StepsObjs[i] = new GameObject[Steps.Count];
            ObjMats[i] = new Material[Steps.Count];

            for (int j = 0; j < Steps.Count; j++){
                StepsObjs[i][j] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                StepsObjs[i][j].transform.position.Set(x+(j*Dim), -5, z-(i*Dim));
                StepsObjs[i][j].transform.localScale.Set(Dim, 1, Dim);
                ObjMats[i][j] = StepsObjs[i][j].GetComponent<Renderer>().material;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        int MS = Mathf.RoundToInt(Time.deltaTime * 1000);

        float lfo = ControlFunctions.Sin(t, 0.5f, 0);
        int[] pitchArr = ControlFunctions.PitchArray(0, new Vector2Int(48, 60), mode);
        int pitch_index = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (pitchArr.Length - 1));
        int pitch = pitchArr[pitch_index];

        bool trig = ramp > (ramp + MS) % beat;
        ramp = (ramp + MS) % beat;
        if (trig){


            Counter = (Counter + 1) % Steps.Capacity;
            //if (Steps[Counter])
            patch.SendMidiNoteOn(0, pitch, 80);
            for (int j = 0; j < Steps.Count; j++)
            {
                StepsObjs[Counter][j].transform.position = new Vector3(StepsObjs[Counter][j].transform.position.x, lfo * 4, 0);
                ObjMats[Counter][j].color = new Color(StepsObjs[Counter][j].transform.position.y, .8f, StepsObjs[Counter][j].transform.position.y / 2) * 0.1f; //Expensive Initialize array
            }

        }
        /*
        if (!Steps[Counter])
        {
            StepsObjs[Counter].transform.position = new Vector3(StepsObjs[Counter].transform.position.x, 0, 0);
            StepsObjs[Counter].GetComponent<Renderer>().material.color = new Color(1, 1, 1);
        }
        */
        Vector4 adsr_par = new Vector4(A, D, S, R);
        float gate_len = beat / 2;
        patch.SendList("ADSR", gate_len, A, D, S, R);
        bool gate = ramp < gate_len;
        float ADSR = ControlFunctions.ADSR(ramp, gate, adsr_par);
    }
}
