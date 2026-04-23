using UnityEngine;
using UnityEngine.InputSystem;

public class DiceManager : MonoBehaviour
{
    public bool isRotating = false;
    public float rotateSpeed = 1f;


    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            isRotating = !isRotating;
        }
    }

}
