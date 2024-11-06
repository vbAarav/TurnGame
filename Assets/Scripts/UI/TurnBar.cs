using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnBar : MonoBehaviour
{
    // References
    [SerializeField] GameObject characterIcon;
    [SerializeField] GameObject icons;

    // Variables
    private int maxTurnIcons = 5;
    

    // Methods
    public void UpdateTurnOrder(Queue<Character> turnOrder)
    {
       // Variables
       List<Character> characters = turnOrder.ToArray().ToList();

       // Remove Old Values
        foreach (Transform icon in icons.transform)
        {
            Destroy(icon.gameObject);
        }

        // Set New Values
        for (int i=0; i<maxTurnIcons; i++)
        {
            // Create Icon
            GameObject icon = new GameObject("CharacterIcon");
            icon.AddComponent<Image>().sprite = characters[i % characters.Count].ChrBase.Sprite;
            icon.transform.SetParent(icons.transform);

            // Set Parameters
            icon.transform.localPosition = new Vector3(87 - (i * 40), 0, 0);
            icon.transform.localScale = new Vector3(1, 1, 1);
            icon.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        }
    }
}
