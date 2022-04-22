using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Portalble.Functions.Grab;
using UnityEngine.SceneManagement;

public class ChickenDemoController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> eggs = new List<GameObject>();
    public List<GameObject> chickens = new List<GameObject>();

    public GameObject egg_prefab;
    public GameObject chicken_prefab;
    public GameObject seed_prefab;
    public GameObject PokeBall_prefab;
    public GameObject Spawn_button;
    public GameObject Seed_button;
    public GameObject PokeBall_button;
    public GameObject Grow_button;
    public GameObject Reset_button;

    public GameObject Hand_r;

    /* highlight materials when grab or select object */
    //test vars
    private GameObject new_seed;

    void Start()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Egg");
        foreach (GameObject e in temp)
        {
            eggs.Add(e);
        }

        temp = GameObject.FindGameObjectsWithTag("Chicken");
        foreach (GameObject e in temp)
        {
            chickens.Add(e);
        }
        Spawn_button.GetComponent<Button>().onClick.AddListener(delegate { TaskSpawnOnClick(); });
        Seed_button.GetComponent<Button>().onClick.AddListener(delegate { TaskSeedOnClick(); });
        PokeBall_button.GetComponent<Button>().onClick.AddListener(delegate { TaskPokeBallOnClick(); });
        Grow_button.GetComponent<Button>().onClick.AddListener(delegate { TaskGrowOnClick(); });
        Reset_button.GetComponent<Button>().onClick.AddListener(delegate { TaskResetOnClick(); });
    }

    // Update is called once per frame
    /* The update function allocates target seed for chicken in the scene ny distance*/
    void Update()
    {
        float minDistance = 1e5f;
        GameObject winner = null;
        if (new_seed != null)
        {
            if (!new_seed.GetComponent<Grabbable>().IsBeingGrabbed())
            {
                foreach (GameObject chicken in chickens)
                {
                    if(Vector3.Distance(chicken.transform.position, new_seed.transform.position) < minDistance)
                    {
                        winner = chicken;
                    }
                }
                if (winner != null)
                {
                    winner.GetComponent<ChickenController>().seeds.Add(new_seed);
                }
            }
        }
    }

    public void TaskGrowOnClick()
    {
        for (int i = 0; i < chickens.Count; i++)
        {
            if (chickens[i] == null)
            {
                chickens.RemoveAt(i);
                i--;
                continue;
            }
            ChickenController obj = chickens[i].GetComponent<ChickenController>();
            StartCoroutine(obj.Grow());
        }
    }

    public void TaskSpawnOnClick()
    {
        for (int i = 0; i < eggs.Count; i++)
        {
            if (eggs[i] == null)
            {
                eggs.RemoveAt(i);
                i--;
                continue;
            }
            EggController obj = eggs[i].GetComponent<EggController>();
            GameObject chicken = obj.born_chicken(0.5f);
            chickens.Add(chicken);
            eggs.RemoveAt(i);
            i--;
        }
    }

    public void TaskSeedOnClick()
    {
        if (Hand_r.GetComponent<GestureControl>().bufferedGesture() == "pinch")
        {
            new_seed = Instantiate(seed_prefab, Hand_r.transform.GetChild(0).GetChild(2).gameObject.transform.position, Hand_r.transform.GetChild(0).GetChild(2).gameObject.transform.rotation);
        }
    }

    public void TaskPokeBallOnClick()
    {
        if (Hand_r.GetComponent<GestureControl>().bufferedGesture() == "pinch")
        {
            Instantiate(PokeBall_prefab, Hand_r.transform.GetChild(0).GetChild(2).gameObject.transform.position, Hand_r.transform.GetChild(0).GetChild(2).gameObject.transform.rotation);
        }
    }

    public void TaskResetOnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void toggle_highlight()
    {
        chicken_prefab.GetComponent<Grabbable>().m_grabbedMaterial = null;
        chicken_prefab.GetComponent<Grabbable>().m_selectedMaterial = null;
        egg_prefab.GetComponent<Grabbable>().m_grabbedMaterial = null;
        egg_prefab.GetComponent<Grabbable>().m_selectedMaterial = null;
        PokeBall_prefab.GetComponent<Grabbable>().m_grabbedMaterial = null;
        PokeBall_prefab.GetComponent<Grabbable>().m_selectedMaterial = null;
        seed_prefab.GetComponent<Grabbable>().m_grabbedMaterial = null;
        seed_prefab.GetComponent<Grabbable>().m_selectedMaterial = null;
    }
}
