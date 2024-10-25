using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CharacterUI : MonoBehaviour
{
    // References
    [SerializeField] GameObject healthBar;
    public int direction = -1;

    // Variables
    private Image healthFill;
    private TextMeshProUGUI healthText;

    private Image chrImage;
    Vector3 originalPosition;
    Color originalColor;

    // Properties
    public Character Chr {get; set;}

    public void Awake()
    {
        chrImage = GetComponent<Image>();
        originalPosition = chrImage.transform.localPosition;
        originalColor = chrImage.color;

        healthFill = healthBar.transform.Find("HealthFill").transform.GetComponent<Image>();
        healthText = healthBar.transform.Find("HealthText").transform.GetComponent<TextMeshProUGUI>();
    }   

    // Initalise Character
    public void Setup(Character character, int dir)
    {
       // Variables
       Chr = character;
       direction = dir;
       chrImage.sprite = Chr.ChrStats.Sprite;
       chrImage.color = originalColor; 

       // UI
       PlayEnterAnimation();
       UpdateHealthUI();
    }

    // Battle Animations
    public void PlayEnterAnimation()
    {
        chrImage.transform.localPosition = new Vector3(direction * 500f, originalPosition.y);
        chrImage.transform.DOLocalMoveX(originalPosition.x, 1f);
    }

    public void PlayExitAnimation()
    {
        var sequence = DOTween.Sequence();
        chrImage.transform.localPosition = originalPosition;
        chrImage.transform.DOLocalMoveX(direction * 500f, 1f);
    }

    public void PlayDeathAnimation()
    {
        chrImage.transform.localPosition = originalPosition;
        chrImage.DOFade(0f, 0.5f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(chrImage.transform.DOLocalMoveX(originalPosition.x - (direction * 50f), 0.25f));
        sequence.Append(chrImage.transform.DOLocalMoveX(originalPosition.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(chrImage.DOColor(Color.cyan, 0.1f));
        sequence.Append(chrImage.DOColor(originalColor, 0.1f));
    }

    // Set the health value in UI
    public void SetHealthUI(int health, int maxHealth)
    {      
        // Set Values
        healthFill.fillAmount = (float)(health)/(float)(maxHealth);
        healthText.text = $"HP: {health}/{maxHealth}";
    }

    public void UpdateHealthUI()
    {
        // Set Values
        healthFill.fillAmount = (float)(Chr.ChrStats.Health)/(float)(Chr.ChrStats.MaxHealth);
        healthText.text = $"HP: {Chr.ChrStats.Health}/{Chr.ChrStats.MaxHealth}";
    }

    public IEnumerator UpdateHealthAnimateUI()
    {
        // Variables
        float currPercentHP = healthFill.fillAmount;
        float newPercentHP = (float)(Chr.ChrStats.Health)/(float)(Chr.ChrStats.MaxHealth);
        float difference = currPercentHP - newPercentHP;

        // Reduce Health Slowly
        while (currPercentHP - newPercentHP > Mathf.Epsilon)
        {
            currPercentHP -= difference * Time.deltaTime;
            healthFill.fillAmount = currPercentHP;            
            yield return null;
        }

        // Set Values
        healthFill.fillAmount = (float)(Chr.ChrStats.Health)/(float)(Chr.ChrStats.MaxHealth);  
        healthText.text = $"HP: {Chr.ChrStats.Health}/{Chr.ChrStats.MaxHealth}";        
    }

    
}
