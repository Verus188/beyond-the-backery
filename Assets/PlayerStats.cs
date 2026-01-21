using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthBar;
    public Slider levelBar;
    public TextMeshProUGUI levelText; // Цифра уровня
    
    [Header("Mock Data")]
    public float maxHealth = 100f;
    public float currentHealth = 75f;
    public int currentLevel = 5;
    public float maxXP = 100f;
    public float currentXP = 40f;
    
    void Start()
    {
        SetupSliders();
        UpdateUI();
    }
    
    void Update()
    {
        // Моковая логика для теста (удали потом)
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10f);
        if (Input.GetKeyDown(KeyCode.Q))
            Heal(15f);
        if (Input.GetKeyDown(KeyCode.L))
            AddXP(10f);
    }
    
    void SetupSliders()
{
    healthBar.minValue = 0f;
    healthBar.maxValue = maxHealth;
    healthBar.value = currentHealth;
    
    levelBar.minValue = 0f;
    levelBar.maxValue = maxXP;
    levelBar.value = currentXP;
}

void UpdateUI()
{
    if (healthBar != null) 
        healthBar.value = Mathf.Clamp(currentHealth, 0, maxHealth);
    if (levelBar != null)
        levelBar.value = Mathf.Clamp(currentXP, 0, maxXP);
    if (levelText != null)
        levelText.text = $"Lv. {currentLevel}";
}
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);
        UpdateUI();
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateUI();
    }
    
    public void AddXP(float xp)
{
    currentXP += xp;
    
    if (currentXP >= maxXP - 0.1f) 
    {
        currentLevel++;
        currentXP = 0f;
        maxXP += 20f;
        levelBar.maxValue = maxXP;
    }
    
    levelBar.value = currentXP;
    
    UpdateUI();
}
}

// H - получить урон

// Q - восстановить HP

// L - увеличить уровень