using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroManager : MonoBehaviour
{
    private static GyroManager instance;
    public static GyroManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GyroManager>();
                if (instance == null)
                {
                    instance = new GameObject("Spawned GyroManager", typeof(GyroManager)).GetComponent<GyroManager>();
                }
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    // Start is called before the first frame update
    [Header("Logic")]
    private Gyroscope gyro;
    private Quaternion rotation;
    private bool gyroActive;
    private Quaternion rot = new Quaternion(0, 0, 1, 0);

    public void EnableGyro()
    {
        if (gyroActive)
            return;

        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroActive = gyro.enabled;
        }
    }

    private void Update()
    {
        if (gyroActive)
        {
            transform.localRotation = gyro.attitude * rot;
            rotation = gyro.attitude;
        }
    }

    public Quaternion GetGyroRotation()
    {
        return rotation;
    }

    public Vector3 GetAngles()
    {
        return transform.eulerAngles;
    }
}
