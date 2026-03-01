using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthBar;
    public Slider levelBar;
    public TextMeshProUGUI levelText;
    public GameObject deathScreen;

    [Header("Audio")]
    public AudioClip playerDamageSound;
    private const float PlayerDamageSoundVolume = 0.35f;
    [Header("Damage Flash")]
    public Color damageFlashColor = new Color(1f, 0.35f, 0.35f, 1f);
    public float damageFlashDuration = 0.08f;

    [Header("Mock Data")]
    public float maxHealth = 100f;
    public float currentHealth = 75f;
    public int currentLevel = 1;
    public float maxXP = 50f;
    public float currentXP = 0f;

    [Header("Level Progression")]
    public float xpForLevel2 = 50f;
    public float flatGrowthPerLevel = 5f;
    public float growthMultiplier = 1.22f;

    private bool isDead;
    private TextMeshProUGUI timerText;
    private float elapsedTime;
    private AudioSource damageAudioSource;
    private bool missingPlayerDamageSoundWarningShown;
    private SpriteRenderer playerSpriteRenderer;
    private Color defaultSpriteColor = Color.white;
    private Coroutine damageFlashCoroutine;

    void Start()
    {
        currentLevel = Mathf.Max(1, currentLevel);
        maxXP = GetRequiredXPForNextLevel(currentLevel);
        currentXP = Mathf.Clamp(currentXP, 0f, maxXP);

        SetupDamageAudioSource();
        SetupDamageFlash();

        if (healthBar == null)
        {
            CreateUI();
        }

        SetupSliders();
        UpdateUI();
        UpdateTimerUI();
    }

    void Update()
    {
        if (isDead) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();

        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(10f);
        if (Input.GetKeyDown(KeyCode.Q)) Heal(15f);
        if (Input.GetKeyDown(KeyCode.L)) AddXP(10f);
    }

    void CreateUI()
    {
        GameObject canvasObj = GameObject.Find("PlayerUI");
        if (canvasObj == null)
        {
            canvasObj = new GameObject("PlayerUI");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        CreateDeathScreen(canvasObj.transform);

        GameObject panel = new GameObject("StatsPanel");
        panel.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(20, -20);
        panelRect.sizeDelta = new Vector2(200, 60);

        healthBar = CreateSlider(panel.transform, "HealthBar", Color.red, new Vector2(0, 0));
        levelBar = CreateSlider(panel.transform, "LevelBar", Color.yellow, new Vector2(0, -30));

        GameObject textObj = new GameObject("LevelText");
        textObj.transform.SetParent(panel.transform, false);
        levelText = textObj.AddComponent<TextMeshProUGUI>();
        levelText.fontSize = 18;
        levelText.text = "Lv. 1";
        levelText.color = Color.black;
        levelText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(0, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.anchoredPosition = new Vector2(160, -30);
        textRect.sizeDelta = new Vector2(110, 20);

        CreateTimerText(canvasObj.transform);
    }

    void CreateTimerText(Transform parent)
    {
        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(parent, false);

        timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.fontSize = 22;
        timerText.text = "00:00";
        timerText.color = Color.black;
        timerText.alignment = TextAlignmentOptions.TopRight;

        RectTransform timerRect = timerObj.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(1, 1);
        timerRect.anchorMax = new Vector2(1, 1);
        timerRect.pivot = new Vector2(1, 1);
        timerRect.anchoredPosition = new Vector2(-20, -20);
        timerRect.sizeDelta = new Vector2(130, 32);
    }

    void CreateDeathScreen(Transform parent)
    {
        deathScreen = new GameObject("DeathScreen");
        deathScreen.transform.SetParent(parent, false);

        RectTransform dsRect = deathScreen.AddComponent<RectTransform>();
        dsRect.anchorMin = Vector2.zero;
        dsRect.anchorMax = Vector2.one;
        dsRect.sizeDelta = Vector2.zero;

        Image bg = deathScreen.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);

        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(deathScreen.transform, false);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "GAME OVER";
        text.fontSize = 50;
        text.color = Color.red;
        text.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(400, 100);
        textRect.anchoredPosition = new Vector2(0, 50);

        GameObject btnObj = new GameObject("RestartButton");
        btnObj.transform.SetParent(deathScreen.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = Color.white;
        Button btn = btnObj.AddComponent<Button>();

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160, 45);
        btnRect.anchoredPosition = new Vector2(0, -50);

        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "RESTART";
        btnText.color = Color.black;
        btnText.fontSize = 20;
        btnText.alignment = TextAlignmentOptions.Center;

        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.sizeDelta = Vector2.zero;

        btn.onClick.AddListener(RestartGame);
        deathScreen.SetActive(false);
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

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-10, -4);

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

    void SetupSliders()
    {
        if (healthBar != null)
        {
            healthBar.minValue = 0f;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (levelBar != null)
        {
            levelBar.minValue = 0f;
            levelBar.maxValue = maxXP;
            levelBar.value = currentXP;
        }
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = Mathf.Clamp(currentHealth, 0f, maxHealth);
        }

        if (levelBar != null)
        {
            levelBar.value = Mathf.Clamp(currentXP, 0f, maxXP);
        }

        if (levelText != null)
        {
            levelText.text = $"Lv. {currentLevel}";
        }
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int totalSeconds = Mathf.FloorToInt(elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        PlayPlayerDamageSound();
        TriggerDamageFlash();
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        UpdateUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    public void AddXP(float xp)
    {
        if (isDead) return;

        currentXP += xp;

        while (currentXP >= maxXP)
        {
            currentXP -= maxXP;
            currentLevel++;
            maxXP = GetRequiredXPForNextLevel(currentLevel);
        }

        if (levelBar != null)
        {
            levelBar.maxValue = maxXP;
            levelBar.value = currentXP;
        }

        UpdateUI();
    }

    float GetRequiredXPForNextLevel(int level)
    {
        int levelStep = Mathf.Max(0, level - 1);
        float baseValue = xpForLevel2 + (levelStep * flatGrowthPerLevel);
        float scaledValue = baseValue * Mathf.Pow(growthMultiplier, levelStep);
        return Mathf.Max(1f, Mathf.Round(scaledValue));
    }

    void Die()
    {
        isDead = true;
        Time.timeScale = 0f;

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetupDamageAudioSource()
    {
        damageAudioSource = gameObject.AddComponent<AudioSource>();
        damageAudioSource.playOnAwake = false;
        damageAudioSource.spatialBlend = 0f;
        damageAudioSource.loop = false;
        damageAudioSource.volume = 1f;
    }

    void PlayPlayerDamageSound()
    {
        if (PlayerDamageSoundVolume <= 0f || damageAudioSource == null)
        {
            return;
        }

        if (playerDamageSound != null)
        {
            damageAudioSource.PlayOneShot(playerDamageSound, PlayerDamageSoundVolume);
            return;
        }

        if (!missingPlayerDamageSoundWarningShown)
        {
            missingPlayerDamageSoundWarningShown = true;
            Debug.LogWarning("[PlayerStats] Player damage sound is not assigned. Set 'Player Damage Sound' in the inspector.");
        }
    }

    void SetupDamageFlash()
    {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            defaultSpriteColor = playerSpriteRenderer.color;
        }
    }

    void TriggerDamageFlash()
    {
        if (playerSpriteRenderer == null || damageFlashDuration <= 0f)
        {
            return;
        }

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(DamageFlashRoutine());
    }

    IEnumerator DamageFlashRoutine()
    {
        playerSpriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);
        playerSpriteRenderer.color = defaultSpriteColor;
        damageFlashCoroutine = null;
    }
}
