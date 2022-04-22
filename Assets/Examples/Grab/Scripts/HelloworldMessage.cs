using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using Portalble.Functions.Grab;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using Mediapipe.HandTracking;

public class HelloworldMessage : MonoBehaviour
{
    private Volume volume;

    private ColorAdjustments colorAdjustments;

    private ARPlaneManager planeManager;

    private ARPlaneController planeController;

    private Text message;

    private GameObject blackOutSquare;

    private Transform p_cube, p_ring, p_explode, hand_pos;

    private float timer = 0f;

    private bool scanner_triggered = false;

    private bool initialize_ready = false;


    private AudioManager camera_audioManager, planet_audioManager;

    bool audio_played_check = false;

    private bool KEY_TRIGGERED = false;

    private bool next_clicked = false;
    private GameObject next_button;
    private Quaternion rot = new Quaternion(0,0,1,0);

    [SerializeField]
    private GameObject introInfo;

    [SerializeField]
    private GameObject planet;


    [SerializeField]
    private TextMeshPro p_instruction;


    // Start is called before the first frame update
    void Start()
    {
        planeManager = GameObject.Find("AR Session Origin").GetComponent<ARPlaneManager>();
        message = GameObject.Find("HelloWorldMessage_txt").GetComponent<Text>();
        blackOutSquare = GameObject.Find("BlackOutScreen");
        planeController = GameObject.Find("AR Session Origin").GetComponent<ARPlaneController>();
        planet = GameObject.Find("Planet");
        p_cube = planet.transform.Find("Metal_cube");
        p_ring = planet.transform.Find("ring");
        p_explode = planet.transform.Find("explode");
        hand_pos = GameObject.Find("Hand_r").transform.Find("palm").transform;
        volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        camera_audioManager = GameObject.Find("AR Camera").GetComponent<AudioManager>();
        planet_audioManager = p_cube.GetComponent<AudioManager>();
        next_button = GameObject.Find("Next");


        //p_cube.gameObject.SetActive(false);
        p_explode.GetComponent<ParticleSystem>().Stop();
        p_ring.GetComponent<ParticleSystem>().Stop();

        //volume settings
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);

        introInfo.SetActive(false);
        planet.SetActive(false);

        //Gyroscope
        GyroManager.Instance.EnableGyro();



    }


    private void instruction_routine()
    {
        if (Vector3.Distance(Camera.main.transform.position, p_cube.position) < 0.8)
        {
  
            if (p_cube.GetComponent<Grabbable>().IsReadyForGrab)
            {
                p_instruction.text = "Try Pinch";
                introInfo.SetActive(false);
                if (!audio_played_check)
                {
                    planet_audioManager.Play("Beep");
                    audio_played_check = true;
                }
            }
            else
            {
                p_instruction.text = "Feel it with your hand";
                audio_played_check = false;
            }
        }
        else
            p_instruction.text = "Move closer to the planet";
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            KEY_TRIGGERED = true;
        }

        /* use a detection for this condition */
        /* or maximum wait time, wait 10 seconds and a button shows up */
        if (!next_clicked)
        {
            return;
        }
        else
        {
            if (!scanner_triggered)
            {
                message.text = "Keep your phone leveled and look around your environment";
            }
        }

        /* found the planes */
        if (planeManager.trackables.count > 0 || KEY_TRIGGERED)
        {
            if (!initialize_ready)
            {
                if (checkLevel(GyroManager.Instance.GetAngles()) || KEY_TRIGGERED){
                    /* turn on hand tracking */
                    GameObject.Find("Process Tracking").GetComponent<Process>().Portalble_on = true;
                    message.text = "";
                    planet.SetActive(true);
                    planet.transform.SetParent(null);
                    StartCoroutine(ChangeVolume());
                    p_explode.GetComponent<ParticleSystem>().Play();
                    p_ring.GetComponent<ParticleSystem>().Play();
                    // p_instruction.text = "HERE";
                    scanner_triggered = true;
                    camera_audioManager.Play("Beep");
                    // StartCoroutine(FadeBlackoutSquare());
                    initialize_ready = true;

                    introInfo.SetActive(true);
                }
            }
            instruction_routine();
        }
    }

    public IEnumerator FadeInPlanet() {
        yield return null;
    }

    public IEnumerator FadeBlackoutSquare(bool fade=true, int fadeSpeed = 2)
    {
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;
        if (fade)
        {
            while (blackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        /* else fade to black */
        else
        {
            while (blackOutSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }

    private IEnumerator ChangeVolume(float duration=5)
    {
        float time = 0;
        float start_postExp = colorAdjustments.postExposure.value;
        float start_saturation = colorAdjustments.saturation.value;

        while (time < duration)
        {
            colorAdjustments.postExposure.value = Mathf.Lerp(start_postExp, 0, time / duration);
            colorAdjustments.saturation.value = Mathf.Lerp(start_saturation, 0, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void nextOnClick()
    {
        next_clicked = true;
        next_button.SetActive(false);
    }

    private bool checkLevel(Vector3 angle)
    {
        /* The exact horizontal y  value for gyroscope is around 280 */
        return (angle.y > 270 && angle.y < 300) || (angle.y < 100 && angle.y > 70);
    }
}
