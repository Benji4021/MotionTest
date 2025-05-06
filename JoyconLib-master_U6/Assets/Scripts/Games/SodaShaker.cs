using System;
using UnityEngine;

public class SodaShaker : MonoBehaviour
{
    [SerializeField]
    private JoyconMovement JM;
    public float SodaLvL;
    public int sensitivity = 10;
    private float shake;
    public float threshold = 150;
    private void Awake()
    {
        if (JM == null)
            JM = GetComponentInParent<JoyconMovement>();
        SodaLvL = 0;
        gameObject.GetComponent<Renderer>().material.color = Color.blue;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shake = JM.finalAccel.magnitude * Time.deltaTime * sensitivity;
        
        /*
        float amp = Mathf.Clamp(SodaLvL * 0.05f, 0f, 1f);
        float freqLow = Mathf.Clamp(160 + SodaLvL * 3f, 40f, 640f);
        float freqHigh = Mathf.Clamp(320 + SodaLvL * 5f, 80f, 640f);
        JM.Rumble(freqLow, freqHigh, amp, 200);
        */
        float amp = Mathf.Clamp(SodaLvL / threshold * 1.25f, 0f, 1f);

        if (shake >= 1.25)
        {
            SodaLvL += shake;
        }
        
        if (SodaLvL < threshold)
        {
            JM.Rumble(160, 320, amp, 200);
        }
        else
        {
            Explode();   
            
        }
    }

    void Explode()
    {
        Debug.Log("Der Korken ist geplatzt!!!");
        gameObject.GetComponent<Renderer>().material.color = Color.red;
        //JM.Rumble(160, 180, 1f, 300);//Geplatzer rumble muss sich differenzieren; entweder gleicher, pulsierend oder tief oder so
    }
}
