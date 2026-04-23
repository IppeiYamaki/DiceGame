using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateSloat : MonoBehaviour
{
    DiceManager diceManager;
    public float rotateSpeed = 1f;
    private void Start()
    {
        GameObject diceManagerObject = GameObject.Find("DiceManager");
        diceManager = diceManagerObject.GetComponent<DiceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }
    void Rotate()
    {
        float y = transform.eulerAngles.x;
        //float threshold = 5f;
        if (diceManager.isRotating)
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.angularVelocity = new Vector3(rotateSpeed, 0, 0);
        }
        else
        {
            Rigidbody rb = this.GetComponent<Rigidbody>();
            rb.angularVelocity = Vector3.zero;
        }

    }

}
