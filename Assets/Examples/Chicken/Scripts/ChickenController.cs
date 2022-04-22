using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Portalble.Functions.Grab;


public class ChickenController : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public float lerpTime = 1;

    // time to pet to trigger animation
    public float petEffectiveTime = 1f;

    public Grabbable grab;
    private bool touched;

    // happiness score, reach 100 to lay an egg
    private int happiness = 0;
    public GameObject FloatingText;

    // enable and disable chicken ability to move
    public bool canmove;
    public List<GameObject> seeds = new List<GameObject>();

    public GameObject egg_prefab;
    public GameObject heart_prefab;

    private GameObject target;
    private bool sit = false;
    public GameObject DC;
    private GameObject heart;

    private float petTime = 0.0f;
    private float falltime = 0.0f;

    void Start()
    {
        DC = GameObject.Find("DemoController");
        this.gameObject.tag = "Chicken";
        animator = this.GetComponent<Animator>();
        GetComponent<Rigidbody>().useGravity = true;
        canmove = true;
        touched = false;
        target = null;
        GameObject[] temp = GameObject.FindGameObjectsWithTag("seed");
        foreach (GameObject e in temp)
        {
            seeds.Add(e);
        }
    }


    /* The function to help chicken lay the egg
     * The coroutine here is to help chicken move away
     * from the new egg to avoid collision.
     */
    public GameObject lay_egg(float scale=0.5f){
        GetComponent<BoxCollider>().enabled = false;
        animator.Play("Sit");
        Vector3 lay_position = transform.position;
        canmove = false;
        happiness = 0;
        animator.Play("Stand");
        canmove = true;
        GameObject clone = (GameObject)GameObject.Instantiate(egg_prefab, lay_position, transform.rotation);
        clone.transform.localScale = new Vector3(scale, scale, scale);
        Vector3 dis = clone.GetComponent<BoxCollider>().size;
        StartCoroutine(move(transform.position + new Vector3(dis.x * scale, 0, dis.z * scale)));
        return clone;
    }

    /* This function is to let chicken find the target
     * and perform the eat action.
     */
    public void find_and_eat(GameObject seed)
    {
        animator.Play("Walk");
        Vector3 target_pos = new Vector3(seed.transform.position.x, transform.position.y, seed.transform.position.z);
        transform.LookAt(target_pos);
        if (!seed.GetComponent<Grabbable>().IsBeingGrabbed())
        {
            transform.position = Vector3.MoveTowards(transform.position, target_pos, 0.2f * Time.deltaTime);
        }
    }

    /* This function is to detect the petting action from hand
     * and let chieck do animations when being pet.
     */
    public void pet_routine()
    {
        if (touched & !grab.IsReadyForGrab)
        {
            touched = false;
            canmove = true;
            animator.Play("Stand");
            Destroy(heart);
            petTime = 0;
        }
        else if (!touched & grab.IsReadyForGrab)
        {
            touched = true;
            canmove = false;
            petTime = 0;
        }
        if (touched)
        {
            petTime += Time.deltaTime;
            if (happiness == 100)
            {
                GameObject new_egg = lay_egg(0.5f);
                DC.GetComponent<ChickenDemoController>().eggs.Add(new_egg);
            }
            else if (petTime % 60 >= petEffectiveTime & !sit)
            {
                animator.Play("Sit");
                if (heart == null) {
                    heart = Instantiate(heart_prefab, transform.position, transform.rotation);
                }
            }
        }
        return;
    }

    /* This function is to let chicken grow big asychronizedly */
    public IEnumerator Grow()
    {
        float i = 0.0f;
        float rate = (1.0f / 5f);
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(5, 5, 5), i);
            yield return null;
        }
    }

    /* This function is to let chicken move asychronizedly to a target position */
    IEnumerator move(Vector3 goal, float speed=0.2f, string animation="Walk")
    {
        animator.Play(animation);
        while (true)
        {
            Vector3 start = transform.position;
            if (start == goal)
                break;
            transform.position = Vector3.MoveTowards(start, goal, speed * Time.deltaTime);
            yield return null;
        }
        animator.Play("Idle");
        GetComponent<BoxCollider>().enabled = true;
        yield break;
    }


    /* This function is to let chicken stand if it falls down to the ground */
    private void stand()
    {
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, Vector3.up, Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


    /* This function is to detect whether chicken has fallen.
     * 0.8 here is a threshold for current chicken model */
    private bool isFall()
    {
        return transform.up.y < 0.8;
    }
    // Update is called once per frame
    /* Update function detect every siganl in the environment that may trigger chicken actions */
    void Update()
    {
        FloatingText.GetComponent<TextMesh>().text = "Score:" + happiness;
        if (!grab.IsReadyForGrab && isFall())
        {
            falltime += Time.deltaTime;
            if (falltime % 60 > 2)
            {
                stand();
            }
        }
        else { falltime = 0; }


        pet_routine();

        if (!canmove)
        {
            return;
        }
        if (target != null)
        {
            find_and_eat(target);
            if (Vector3.Distance(transform.position, target.transform.position) < 0.27f)
            {
                Destroy(target);
                happiness += 20;
                animator.Play("Idle");
            }
        }
        else if (seeds.Count > 0)
        {
            target = seeds[0];
            seeds.RemoveAt(0);
        }
    }
}
