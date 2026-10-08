using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

namespace TanesHemburud.EditorTools
{
    public class SceneSetupTool
    {
        [MenuItem("Tools/TanesHemburud/Build All Scenes")]
        public static void BuildAllScenes()
        {
            string scenesFolder = "Assets/Scenes";
            if (!Directory.Exists(scenesFolder))
            {
                Directory.CreateDirectory(scenesFolder);
            }

            CreateMainMenuScene($"{scenesFolder}/MainMenu.unity");
            CreateStageSelectionScene($"{scenesFolder}/StageSelection.unity");
            CreateStage1Scene($"{scenesFolder}/Stage1.unity");
            CreateStage2Scene($"{scenesFolder}/Stage2.unity");
            CreateWinScene($"{scenesFolder}/Win.unity");
            CreateGameOverScene($"{scenesFolder}/GameOver.unity");
            CreateOptionsScene($"{scenesFolder}/Options.unity");
            CreateCreditsScene($"{scenesFolder}/Credits.unity");

            // Register in Build Settings
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene($"{scenesFolder}/MainMenu.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/StageSelection.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/Stage1.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/Stage2.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/Win.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/GameOver.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/Options.unity", true),
                new EditorBuildSettingsScene($"{scenesFolder}/Credits.unity", true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Re-open MainMenu
            EditorSceneManager.OpenScene($"{scenesFolder}/MainMenu.unity");
            Debug.Log("Successfully created all 8 Scenes and registered in Build Settings!");
        }

        private static Canvas CreateBaseCanvas(string sceneName, Color bgColor)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            GameObject camGO = new GameObject("Main Camera");
            Camera cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bgColor;
            cam.orthographic = true;
            camGO.AddComponent<AudioListener>();

            // EventSystem
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();

            // Canvas
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Navigation Manager Object
            GameObject navGO = new GameObject("SceneNavigationManager");
            navGO.AddComponent<SceneNavigationManager>();

            return canvas;
        }

        private static TMP_Text CreateText(GameObject parent, string name, string content, Vector2 pos, Vector2 size, float fontSize, TextAlignmentOptions align, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);

            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = color;

            return tmp;
        }

        private static Button CreateButton(GameObject parent, string name, string buttonText, Vector2 pos, Vector2 size, Color bgColor)
        {
            GameObject btnGO = new GameObject(name);
            btnGO.transform.SetParent(parent.transform, false);

            RectTransform rt = btnGO.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = btnGO.AddComponent<Image>();
            img.color = bgColor;

            Button btn = btnGO.AddComponent<Button>();

            // Button Text
            GameObject txtGO = new GameObject("Text");
            txtGO.transform.SetParent(btnGO.transform, false);
            RectTransform txtRT = txtGO.AddComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero;
            txtRT.anchorMax = Vector2.one;
            txtRT.sizeDelta = Vector2.zero;

            TMP_Text tmp = txtGO.AddComponent<TextMeshProUGUI>();
            tmp.text = buttonText;
            tmp.fontSize = 22;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.black;

            return btn;
        }

        private static void CreateMainMenuScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("MainMenu", new Color(0.15f, 0.18f, 0.22f));
            GameObject cGO = canvas.gameObject;

            MainMenuUI uiComp = cGO.AddComponent<MainMenuUI>();

            // Title Label
            CreateText(cGO, "TitleLabel", "168-958321-Practical Examination", new Vector2(0, 250), new Vector2(800, 60), 36, TextAlignmentOptions.Center, Color.white);

            // Student Info
            TMP_Text studentInfo = CreateText(cGO, "StudentInfo", "Student Code: 672110095\nName: ธเณศร์ เห็มบุรุษ (Tanes Hemburud)", new Vector2(0, 180), new Vector2(800, 70), 22, TextAlignmentOptions.Center, new Color(0.9f, 0.9f, 0.9f));
            SerializedObject soUI = new SerializedObject(uiComp);
            soUI.FindProperty("studentInfoText").objectReferenceValue = studentInfo;
            soUI.ApplyModifiedProperties();

            // Scene Label
            CreateText(cGO, "SceneLabel", "Main Menu Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.yellow);

            // Buttons Container
            Color btnColor = new Color(0.85f, 0.88f, 0.92f);
            CreateButton(cGO, "BtnStart", "Start", new Vector2(0, 50), new Vector2(240, 50), btnColor);
            CreateButton(cGO, "BtnOptions", "Options", new Vector2(0, -20), new Vector2(240, 50), btnColor);
            CreateButton(cGO, "BtnCredits", "Credits", new Vector2(0, -90), new Vector2(240, 50), btnColor);
            CreateButton(cGO, "BtnExit", "Exit", new Vector2(0, -160), new Vector2(240, 50), new Color(0.95f, 0.6f, 0.6f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateStageSelectionScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("StageSelection", new Color(0.18f, 0.20f, 0.25f));
            GameObject cGO = canvas.gameObject;

            StageSelectionUI uiComp = cGO.AddComponent<StageSelectionUI>();

            // Header
            CreateText(cGO, "TitleLabel", "Stage Selection", new Vector2(0, 250), new Vector2(800, 60), 38, TextAlignmentOptions.Center, Color.white);
            CreateText(cGO, "SceneLabel", "Stage Selection Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.yellow);

            TMP_Text studentInfo = CreateText(cGO, "StudentInfo", "Student Code: 672110095 Name: ธเณศร์ เห็มบุรุษ", new Vector2(0, 180), new Vector2(800, 50), 22, TextAlignmentOptions.Center, Color.lightGray);
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("studentInfoText").objectReferenceValue = studentInfo;
            so.ApplyModifiedProperties();

            // Buttons
            Color btnColor = new Color(0.85f, 0.88f, 0.95f);
            CreateButton(cGO, "BtnStage1", "Stage 1 - Capsule Player", new Vector2(0, 40), new Vector2(360, 55), btnColor);
            CreateButton(cGO, "BtnStage2", "Stage 2 - Humanoid Player", new Vector2(0, -35), new Vector2(360, 55), btnColor);
            CreateButton(cGO, "BtnBack", "Back", new Vector2(-320, -250), new Vector2(160, 45), new Color(0.75f, 0.75f, 0.75f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateStage1Scene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Stage1", new Color(0.2f, 0.25f, 0.3f));
            GameObject cGO = canvas.gameObject;

            cGO.AddComponent<StageTestFlowUI>();

            CreateText(cGO, "SceneLabel", "Stage 1 - Capsule Player Minigame Scene", new Vector2(-250, 320), new Vector2(450, 40), 20, TextAlignmentOptions.Left, Color.yellow);
            CreateText(cGO, "TitleLabel", "Stage 1 - Capsule Player Minigame", new Vector2(0, 250), new Vector2(800, 60), 32, TextAlignmentOptions.Center, Color.cyan);
            CreateText(cGO, "StudentInfo", "Student Code: 672110095 Name: ธเณศร์ เห็มบุรุษ", new Vector2(0, 190), new Vector2(800, 40), 20, TextAlignmentOptions.Center, Color.white);

            // Test trigger buttons for Stage Flow
            CreateButton(cGO, "BtnTestWin", "Test Win -> Win Scene", new Vector2(-150, 0), new Vector2(260, 50), new Color(0.6f, 0.9f, 0.6f));
            CreateButton(cGO, "BtnTestLose", "Test Lose -> Game Over", new Vector2(150, 0), new Vector2(260, 50), new Color(0.9f, 0.6f, 0.6f));
            CreateButton(cGO, "BtnBack", "Back to Selection", new Vector2(-320, -250), new Vector2(200, 45), new Color(0.8f, 0.8f, 0.8f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateStage2Scene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Stage2", new Color(0.25f, 0.2f, 0.3f));
            GameObject cGO = canvas.gameObject;

            cGO.AddComponent<StageTestFlowUI>();

            CreateText(cGO, "SceneLabel", "Stage 2 - Humanoid Player Minigame Scene", new Vector2(-250, 320), new Vector2(450, 40), 20, TextAlignmentOptions.Left, Color.yellow);
            CreateText(cGO, "TitleLabel", "Stage 2 - Humanoid Player Minigame", new Vector2(0, 250), new Vector2(800, 60), 32, TextAlignmentOptions.Center, Color.magenta);
            CreateText(cGO, "StudentInfo", "Student Code: 672110095 Name: ธเณศร์ เห็มบุรุษ", new Vector2(0, 190), new Vector2(800, 40), 20, TextAlignmentOptions.Center, Color.white);

            // Test trigger buttons
            CreateButton(cGO, "BtnTestWin", "Test Win -> Win Scene", new Vector2(-150, 0), new Vector2(260, 50), new Color(0.6f, 0.9f, 0.6f));
            CreateButton(cGO, "BtnTestLose", "Test Lose -> Game Over", new Vector2(150, 0), new Vector2(260, 50), new Color(0.9f, 0.6f, 0.6f));
            CreateButton(cGO, "BtnBack", "Back to Selection", new Vector2(-320, -250), new Vector2(200, 45), new Color(0.8f, 0.8f, 0.8f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateWinScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Win", new Color(0.72f, 0.88f, 0.68f)); // Soft green like Fig 1
            GameObject cGO = canvas.gameObject;

            WinGameOverUI uiComp = cGO.AddComponent<WinGameOverUI>();

            CreateText(cGO, "SceneLabel", "Win Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.darkGray);
            CreateText(cGO, "TitleLabel", "Win", new Vector2(0, 160), new Vector2(600, 80), 54, TextAlignmentOptions.Center, new Color(0.1f, 0.4f, 0.1f));

            TMP_Text scoreTxt = CreateText(cGO, "ScoreText", "Score: 99999", new Vector2(0, 40), new Vector2(600, 60), 32, TextAlignmentOptions.Center, Color.black);
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("scoreText").objectReferenceValue = scoreTxt;
            so.ApplyModifiedProperties();

            CreateButton(cGO, "BtnBackToMain", "Back To Main Menu", new Vector2(0, -100), new Vector2(260, 50), new Color(0.9f, 0.95f, 0.9f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateGameOverScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("GameOver", new Color(0.92f, 0.72f, 0.72f)); // Soft red like Fig 1
            GameObject cGO = canvas.gameObject;

            WinGameOverUI uiComp = cGO.AddComponent<WinGameOverUI>();

            CreateText(cGO, "SceneLabel", "Game Over Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.darkGray);
            CreateText(cGO, "TitleLabel", "Game Over", new Vector2(0, 160), new Vector2(600, 80), 54, TextAlignmentOptions.Center, new Color(0.5f, 0.1f, 0.1f));

            TMP_Text scoreTxt = CreateText(cGO, "ScoreText", "Score: 99999", new Vector2(0, 50), new Vector2(600, 60), 32, TextAlignmentOptions.Center, Color.black);
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("scoreText").objectReferenceValue = scoreTxt;
            so.ApplyModifiedProperties();

            CreateButton(cGO, "BtnTryAgain", "Try Again", new Vector2(0, -30), new Vector2(240, 50), new Color(0.95f, 0.85f, 0.85f));
            CreateButton(cGO, "BtnBackToMain", "Back To Main Menu", new Vector2(0, -100), new Vector2(240, 50), new Color(0.9f, 0.8f, 0.8f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateOptionsScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Options", new Color(0, 0, 0, 0.4f)); // Overlay semi transparent
            GameObject cGO = canvas.gameObject;

            OptionsMenuUI uiComp = cGO.AddComponent<OptionsMenuUI>();

            // Central Panel Box
            GameObject panel = new GameObject("OptionsPanel");
            panel.transform.SetParent(cGO.transform, false);
            RectTransform pRT = panel.AddComponent<RectTransform>();
            pRT.sizeDelta = new Vector2(400, 300);
            Image pImg = panel.AddComponent<Image>();
            pImg.color = new Color(0.9f, 0.95f, 1f, 0.95f);

            CreateText(panel, "SceneLabel", "Options Scene (Additive)", new Vector2(0, 120), new Vector2(360, 30), 16, TextAlignmentOptions.Center, Color.gray);
            CreateText(panel, "TitleLabel", "Options", new Vector2(0, 80), new Vector2(360, 40), 28, TextAlignmentOptions.Center, Color.black);

            // BGM Label
            CreateText(panel, "BGMLabel", "BGM Volume", new Vector2(-100, 20), new Vector2(160, 30), 18, TextAlignmentOptions.Left, Color.black);
            // BGM Slider
            GameObject bgmSliderGO = new GameObject("BGMSlider");
            bgmSliderGO.transform.SetParent(panel.transform, false);
            RectTransform bgmRT = bgmSliderGO.AddComponent<RectTransform>();
            bgmRT.anchoredPosition = new Vector2(50, 20);
            bgmRT.sizeDelta = new Vector2(160, 20);
            Slider bgmSlider = bgmSliderGO.AddComponent<Slider>();

            // Background of slider
            GameObject bgmBg = new GameObject("Background");
            bgmBg.transform.SetParent(bgmSliderGO.transform, false);
            RectTransform bgmBgRT = bgmBg.AddComponent<RectTransform>();
            bgmBgRT.anchorMin = Vector2.zero; bgmBgRT.anchorMax = Vector2.one; bgmBgRT.sizeDelta = Vector2.zero;
            bgmBg.AddComponent<Image>().color = Color.gray;

            // Fill Area
            GameObject bgmFillArea = new GameObject("Fill Area");
            bgmFillArea.transform.SetParent(bgmSliderGO.transform, false);
            RectTransform bgmFillAreaRT = bgmFillArea.AddComponent<RectTransform>();
            bgmFillAreaRT.anchorMin = Vector2.zero; bgmFillAreaRT.anchorMax = Vector2.one; bgmFillAreaRT.sizeDelta = Vector2.zero;
            GameObject bgmFill = new GameObject("Fill");
            bgmFill.transform.SetParent(bgmFillArea.transform, false);
            RectTransform bgmFillRT = bgmFill.AddComponent<RectTransform>();
            bgmFillRT.anchorMin = Vector2.zero; bgmFillRT.anchorMax = Vector2.one; bgmFillRT.sizeDelta = Vector2.zero;
            Image bgmFillImg = bgmFill.AddComponent<Image>();
            bgmFillImg.color = Color.blue;
            bgmSlider.fillRect = bgmFillRT;

            // SFX Label
            CreateText(panel, "SFXLabel", "SFX Volume", new Vector2(-100, -30), new Vector2(160, 30), 18, TextAlignmentOptions.Left, Color.black);
            // SFX Slider
            GameObject sfxSliderGO = new GameObject("SFXSlider");
            sfxSliderGO.transform.SetParent(panel.transform, false);
            RectTransform sfxRT = sfxSliderGO.AddComponent<RectTransform>();
            sfxRT.anchoredPosition = new Vector2(50, -30);
            sfxRT.sizeDelta = new Vector2(160, 20);
            Slider sfxSlider = sfxSliderGO.AddComponent<Slider>();

            // Background of slider
            GameObject sfxBg = new GameObject("Background");
            sfxBg.transform.SetParent(sfxSliderGO.transform, false);
            RectTransform sfxBgRT = sfxBg.AddComponent<RectTransform>();
            sfxBgRT.anchorMin = Vector2.zero; sfxBgRT.anchorMax = Vector2.one; sfxBgRT.sizeDelta = Vector2.zero;
            sfxBg.AddComponent<Image>().color = Color.gray;

            // Fill Area
            GameObject sfxFillArea = new GameObject("Fill Area");
            sfxFillArea.transform.SetParent(sfxSliderGO.transform, false);
            RectTransform sfxFillAreaRT = sfxFillArea.AddComponent<RectTransform>();
            sfxFillAreaRT.anchorMin = Vector2.zero; sfxFillAreaRT.anchorMax = Vector2.one; sfxFillAreaRT.sizeDelta = Vector2.zero;
            GameObject sfxFill = new GameObject("Fill");
            sfxFill.transform.SetParent(sfxFillArea.transform, false);
            RectTransform sfxFillRT = sfxFill.AddComponent<RectTransform>();
            sfxFillRT.anchorMin = Vector2.zero; sfxFillRT.anchorMax = Vector2.one; sfxFillRT.sizeDelta = Vector2.zero;
            Image sfxFillImg = sfxFill.AddComponent<Image>();
            sfxFillImg.color = Color.blue;
            sfxSlider.fillRect = sfxFillRT;

            // Bind Sliders to OptionsMenuUI
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("bgmSlider").objectReferenceValue = bgmSlider;
            so.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
            so.ApplyModifiedProperties();

            // OK Button
            CreateButton(panel, "BtnOK", "OK", new Vector2(0, -90), new Vector2(120, 40), new Color(0.8f, 0.9f, 0.8f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateCreditsScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Credits", new Color(0.95f, 0.95f, 0.95f));
            GameObject cGO = canvas.gameObject;

            CreditsUI uiComp = cGO.AddComponent<CreditsUI>();

            // Panel
            GameObject panel = new GameObject("CreditsPanel");
            panel.transform.SetParent(cGO.transform, false);
            RectTransform pRT = panel.AddComponent<RectTransform>();
            pRT.sizeDelta = new Vector2(500, 350);
            Image pImg = panel.AddComponent<Image>();
            pImg.color = Color.white;

            CreateText(cGO, "SceneLabel", "Credits Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.gray);
            TMP_Text headerTxt = CreateText(panel, "TitleLabel", "Credits", new Vector2(0, 110), new Vector2(400, 50), 36, TextAlignmentOptions.Center, Color.black);
            TMP_Text infoTxt = CreateText(panel, "StudentInfo", "Student Code: 672110095\nName: ธเณศร์ เห็มบุรุษ (Tanes Hemburud)", new Vector2(0, 0), new Vector2(460, 100), 24, TextAlignmentOptions.Center, Color.darkGray);

            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("creditsDetailText").objectReferenceValue = headerTxt;
            so.FindProperty("studentInfoText").objectReferenceValue = infoTxt;
            so.ApplyModifiedProperties();

            CreateButton(panel, "BtnBack", "Back", new Vector2(-180, -130), new Vector2(100, 40), new Color(0.85f, 0.85f, 0.85f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }
    }
}
