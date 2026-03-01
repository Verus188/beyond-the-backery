using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "WalkingSquare";

    private GameObject menuPanel;
    private GameObject settingsPanel;

    void Start()
    {
        Time.timeScale = 1f;
        EnsureEventSystem();
        BuildMenu();
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();
    }

    private void BuildMenu()
    {
        Canvas canvas = CreateCanvas();

        menuPanel = CreateFullscreenPanel("MainMenuPanel", canvas.transform, new Color(0f, 0f, 0f, 0.8f));
        settingsPanel = CreateFullscreenPanel("SettingsPanel", canvas.transform, new Color(0f, 0f, 0f, 0.8f));
        settingsPanel.SetActive(false);

        GameObject titleObj = CreateText("Title", menuPanel.transform, "BEYOND THE BACKERY", 42, Color.white);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.75f);
        titleRect.anchorMax = new Vector2(0.5f, 0.75f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(900f, 80f);

        VerticalLayoutGroup menuLayout = CreateMenuLayout(menuPanel.transform, new Vector2(0.5f, 0.45f));
        CreateButton("PlayButton", "Играть", menuLayout.transform, OnPlayClicked);
        CreateButton("SettingsButton", "Настройки", menuLayout.transform, OnSettingsClicked);
        CreateButton("QuitButton", "Выход", menuLayout.transform, OnQuitClicked);

        GameObject settingsTitleObj = CreateText("SettingsTitle", settingsPanel.transform, "Настройки", 38, Color.white);
        RectTransform settingsTitleRect = settingsTitleObj.GetComponent<RectTransform>();
        settingsTitleRect.anchorMin = new Vector2(0.5f, 0.7f);
        settingsTitleRect.anchorMax = new Vector2(0.5f, 0.7f);
        settingsTitleRect.pivot = new Vector2(0.5f, 0.5f);
        settingsTitleRect.anchoredPosition = Vector2.zero;
        settingsTitleRect.sizeDelta = new Vector2(500f, 70f);

        GameObject settingsTextObj = CreateText("SettingsText", settingsPanel.transform, "Экран настроек", 26, Color.white);
        RectTransform settingsTextRect = settingsTextObj.GetComponent<RectTransform>();
        settingsTextRect.anchorMin = new Vector2(0.5f, 0.55f);
        settingsTextRect.anchorMax = new Vector2(0.5f, 0.55f);
        settingsTextRect.pivot = new Vector2(0.5f, 0.5f);
        settingsTextRect.anchoredPosition = Vector2.zero;
        settingsTextRect.sizeDelta = new Vector2(500f, 50f);

        VerticalLayoutGroup settingsLayout = CreateMenuLayout(settingsPanel.transform, new Vector2(0.5f, 0.4f));
        CreateButton("BackButton", "Назад", settingsLayout.transform, OnBackClicked);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private GameObject CreateFullscreenPanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private VerticalLayoutGroup CreateMenuLayout(Transform parent, Vector2 anchor)
    {
        GameObject container = new GameObject("MenuLayout");
        container.transform.SetParent(parent, false);

        RectTransform rect = container.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(280f, 220f);

        VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        return layout;
    }

    private Button CreateButton(string name, string label, Transform parent, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(260f, 56f);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.95f);

        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(onClick);

        GameObject textObj = CreateText("Label", buttonObj.transform, label, 28, Color.black);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return button;
    }

    private GameObject CreateText(string name, Transform parent, string text, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        Text uiText = textObj.AddComponent<Text>();
        uiText.text = text;
        uiText.fontSize = fontSize;
        uiText.color = color;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return textObj;
    }

    private void OnPlayClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    private void OnSettingsClicked()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnBackClicked()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
