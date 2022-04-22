using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderScript : MonoBehaviour
{

    [SerializeField]
    UnityEngine.UI.Text textBox;

    [SerializeField]
    string sliderName;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void UpdateTextBox(System.Single f)
    {
        textBox.text = sliderName + ": " + f.ToString();
    }
}
