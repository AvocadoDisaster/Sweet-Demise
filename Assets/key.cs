using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class key : MonoBehaviour
{
   // private int keytimer = 5;
    public GameObject Door;
   // public GameObject ActivationKey;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            //keytimer--;
            Destroy(gameObject);
            Destroy(Door);
            Debug.LogWarning("works");
           // gameObject.SetActive(true);
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

