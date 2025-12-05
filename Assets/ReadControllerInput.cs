using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ReadControllerInput : MonoBehaviour
{
    public InputActionProperty TriggerAction;
    public InputActionProperty GripAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*float triggerValue = TriggerAction.action.ReadValue<float>();
        float gripValue = GripAction.action.ReadValue<float>();
        Debug.Log("Trigger: " + triggerValue + "Grip: " +gripValue);*/
    }
}
