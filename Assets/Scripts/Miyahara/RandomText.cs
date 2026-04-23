using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RandomText : MonoBehaviour
{
    DiceManager diceManager;
    public TMP_Text randomText;

    private void Start()
    {
        GameObject diceManagerObject = GameObject.Find("DiceManager");
        diceManager = diceManagerObject.GetComponent<DiceManager>();
        randomText.text = "0";
    }
    void Update()
    {

        if (diceManager.isRotating)
        {
            int randomNumber = Random.Range(1, 6);
            randomText.text = randomNumber.ToString();
            Debug.Log("Random Number: " + randomNumber);
        }

    }
}
