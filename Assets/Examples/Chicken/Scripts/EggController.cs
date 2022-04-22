using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble.Functions.Grab;

public class EggController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public Grabbable grab;
    private bool grabbed;
    public GameObject chicken_prefab;
    private bool borned;

    public GameObject DC;
    void Start()
    {
        DC = GameObject.Find("DemoController");
        this.gameObject.tag = "Egg";
        animator = this.GetComponent<Animator>();
        grabbed = false;
        borned = false;
    }

    // Update is called once per frame
    /* The update function is to detect the drop of the egg
     * born_chicken is called when egg is dropped by hand
     */
    void Update()
    {
        if (!grabbed & grab.IsBeingGrabbed())
        {
            grabbed = true;
        }
        if (grabbed & !grab.IsBeingGrabbed() &!borned)
        {
            GameObject new_chicken = born_chicken(0.5f);
            DC.GetComponent<ChickenDemoController>().chickens.Add(new_chicken);
        }
    }

    /*
     * This is the function for born the chicken out
     * Disable the collider to avoid collision during birth
     */
    public GameObject born_chicken(float scale=0.5f)
    {
        GetComponent<BoxCollider>().enabled = false;
        animator.Play("Shake");
        GameObject new_chicken = Instantiate(chicken_prefab, transform.position, transform.rotation);
        new_chicken.transform.localScale = new Vector3(scale, scale, scale);
        borned = true;
        return new_chicken;
    }
}
