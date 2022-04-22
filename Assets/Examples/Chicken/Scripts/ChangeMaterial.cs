using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    // Start is called before the first frame update

    public Material transparent;
    public Material featheredplane;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle(bool value)
    {
        if (value)
        {
            this.GetComponent<MeshRenderer>().material = featheredplane;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material = transparent;
        }
    }
}
