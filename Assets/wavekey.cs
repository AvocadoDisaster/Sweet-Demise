using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wavekey : MonoBehaviour
{
    // private int keytimer = 5;
   // public GameObject Door;
    public GameObject AppearingKey;
    public GameObject AppearingKey2;
    public GameObject AppearingKey3;
    public GameObject AppearingKey4;


    // Start is called before the first frame update
    void Start()
    {
        AppearingKey.SetActive(false);
        AppearingKey2.SetActive(false);
        AppearingKey3.SetActive(false);
        AppearingKey4.SetActive(false);
    }

    /* private void OnDestroy()
     {

     }*/
    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            //keytimer--;
            Destroy(gameObject);
            // Destroy(Door);
            AppearingKey.SetActive(true);
            AppearingKey2.SetActive(true);
            AppearingKey3.SetActive(true);
            AppearingKey4.SetActive(true);
            // Door.SetActive(false);

            //gameObject.SetActive(false);

        }
        /* if (keyCount == 3)
          {
              Debug.Log("keycount 1");
              Destroy(Door);
          }*/
    }
}
