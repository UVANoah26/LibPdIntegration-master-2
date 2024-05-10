using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melody : MonoBehaviour
{
    [SerializeField]
    ClockControl clock;

    [SerializeField]
    public GameObject prefab;

    [SerializeField]
    public GameObject THIS;

    [SerializeField]
    public LibPdInstance patch;

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

    //2 4 5 7 9 10

    int[] Notes = new int[] { 7, 10, 10, 5, 4, 7, 9, 10};


    // Start is called before the first frame update
    void Start()
    {


    }
   
    // Update is called once per frame
    void Update()
    {
        if (clock.isOn[1]||clock.isOn[2])
        {
            if (clock.isOn[2])
                Notes = new int[] { 10, 9, 7, 5, 10, 7, 7, 5 };
            t += Time.deltaTime;
            int MS = Mathf.RoundToInt(Time.deltaTime * 1000);

            bool trig = ramp > (ramp + MS) % beat;
            ramp = (ramp + MS) % beat;


            if (trig)
            {
                Counter++;
                if (Counter % 1 == 0)
                {
                    index++;
                }
                patch.SendMidiNoteOn(1, Notes[index % 8] + 60, 40);
                
                float randomX = Random.Range(-8f, 8f);
                float randomZ = Random.Range(-8f, 8f);
                //prefab.transform.position = new Vector3(randomX,prefab.transform.position.y,randomZ);



                // Instantiate the RedFog prefab at the random position
                Vector3 spawnPosition = new Vector3(randomX, 0f, randomZ);
                Instantiate(prefab, spawnPosition, Quaternion.identity);

                THIS.transform.position = new Vector3(randomX, 0f, randomZ);
            }



            //ADSR Management
            Vector4 adsr_par = new Vector4(A, D, S, R);
            float gate_len = clock.beatMs / 2;
            patch.SendList("ADSR", gate_len, A, D, S, R);
            bool gate = clock.ramp < gate_len;
            float ADSR = ControlFunctions.ADSR(clock.ramp, gate, adsr_par);

            patch.SendFloat("reverb", Reverb);

        }
    }

    
}