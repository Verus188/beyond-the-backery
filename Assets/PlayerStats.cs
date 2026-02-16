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
        if (healthBar == null) CreateUI();
        SetupSliders();
        UpdateUI();
    }

    void CreateUI()
    {
        // Создаем Canvas, если его нет
        GameObject canvasObj = new GameObject("PlayerUI");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Гарантируем, что будет выше фона (-100)
        canvasObj.AddComponent<GraphicRaycaster>();

        // Создаем контейнер для баров
        GameObject panel = new GameObject("StatsPanel");
        panel.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(20, -20);
        panelRect.sizeDelta = new Vector2(200, 60);

        // Создаем Health Bar
        healthBar = CreateSlider(panel.transform, "HealthBar", Color.red, new Vector2(0, 0));
        
        // Создаем Level Bar
        levelBar = CreateSlider(panel.transform, "LevelBar", Color.yellow, new Vector2(0, -30));

        // Создаем текст уровня
        GameObject textObj = new GameObject("LevelText");
        textObj.transform.SetParent(panel.transform, false);
        levelText = textObj.AddComponent<TextMeshProUGUI>();
        levelText.fontSize = 18;
        levelText.text = "Lv. 1";
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(210, 0);
        textRect.sizeDelta = new Vector2(100, 30);
    }

    Slider CreateSlider(Transform parent, string name, Color color, Vector2 pos)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        Slider slider = sliderObj.AddComponent<Slider>();
        
        RectTransform rect = sliderObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(150, 20);

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-10, -4);

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = color;
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;

        slider.fillRect = fillRect;
        slider.targetGraphic = fillImg;
        
        return slider;
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