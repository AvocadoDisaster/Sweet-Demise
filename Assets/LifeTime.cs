using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{

    [SerializeField] public float lifetiming;
    public float originallife;
    // Start is called before the first frame update
    void Start()
    {
        originallife = lifetiming;
        Destroy(gameObject, lifetiming);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (lifetiming == 0)
        {
            lifetiming = originallife;
        }
    }


   


}
