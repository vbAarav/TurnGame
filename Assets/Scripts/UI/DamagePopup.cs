using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    // Variables
    private TextMeshProUGUI damageText;
    public string txtNew;
    public Color txtColour;
    public Color txtOutlineColour;
    private float timer = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        // Set Properties
        damageText = transform.Find("Canvas").transform.Find("DamageTxt").GetComponent<TextMeshProUGUI>();
        damageText.fontSize += 9;
    }

    // Getters and Setters
    public void SetText(string text)
    {
        txtNew = text;
    }
    public void SetTextColour(Color color)
    {
        txtColour = color;
    }

    public void SetOutlineColour(Color color)
    {
        txtOutlineColour = color;
    }

    void Update()
    {
        // Apply Visuals
        damageText.outlineColor = txtOutlineColour;    
        damageText.color = txtColour;           
        damageText.text = txtNew;
        float speed = 1f;
        transform.position += new Vector3(0, speed) * Time.deltaTime;

        // Apply Effects
        damageText.alpha -= 3f * Time.deltaTime;
        transform.localScale += Vector3.one * 0.2f * Time.deltaTime;

        // Existence Timer
        timer += 1f * Time.deltaTime;

        // Destroy Extra Popups
        if (timer >= 3f)
            Destroy(gameObject);
    }
}
