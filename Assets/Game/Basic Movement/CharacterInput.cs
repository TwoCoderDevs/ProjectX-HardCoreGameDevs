﻿using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    [Header("Player Inputs")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string turnAxis = "Mouse X";
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Camera")]
    public Transform target;
    [HideInInspector] public float h;
    [HideInInspector] public float v;
    [HideInInspector] public float t;
    

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis(horizontalAxis);
        v = Input.GetAxis(verticalAxis);
        t = Input.GetAxis(turnAxis);

        if(Input.GetKey(sprintKey))
        {
            v = v * 2;
        }                
    }
}
