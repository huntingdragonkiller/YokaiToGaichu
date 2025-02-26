using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ActiveObject : MonoBehaviour
{
    public GameObject objectToToggle;
    public bool isOn;

    void Update()
    {
        if (isOn == true)
        {
            objectToToggle.SetActive(true);
        }
        else if (isOn == false)
        {
            objectToToggle.SetActive(false);
        }
    }

    public void ToggleOn()
    {
        isOn = true;
    }

    public void ToggleOff()
    {
        isOn = false;
    }
}
