using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSending : MonoBehaviour
{
    [SerializeField]
    List<bool> Steps;
    GameObject[] StepsObjs;

    public LibPdInstance patch;
    float ramp;
    float t;
    int[] mode;
    // Start is called before the first frame update
    void Start()
    {
        mode = new int[]{2,2,1,2,2,1};

        StepsObjs = new GameObject[Steps.Count];
        for (int i = 0; i < Steps.Count; i++)
        {
            StepsObjs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            StepsObjs[i].transform.position = new Vector3(-2*Steps.Count+4*i,0,0);

        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
