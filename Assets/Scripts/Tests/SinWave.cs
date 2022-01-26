using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWave : MonoBehaviour
{

    public float frecuency = 5;
    public float maginitude = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Mathf.Sin(Time.deltaTime));
        transform.position += transform.up * Mathf.Sin(Time.deltaTime * frecuency) * maginitude;
    }
}
