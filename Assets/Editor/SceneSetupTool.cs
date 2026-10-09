using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;
using UnityEngine.ProBuilder;

namespace ThanethHamburut.EditorTools
{
    public class SceneSetupTool
    {
        private const string ScenesFolder = "Assets/Scenes";
        private const string PrefabsFolder = "Assets/Prefabs/Items";
        private const string MaterialsFolder = "Assets/Materials";

        [MenuItem("Tools/ThanethHamburut/Build All Scenes")]
        public static void BuildAllScenes()
        {
            EnsureDirectories();
            CreateMaterials();
            CreateItemPrefabs();

            CreateMainMenuScene($"{ScenesFolder}/MainMenu.unity");
            CreateStageSelectionScene($"{ScenesFolder}/StageSelection.unity");
            CreateStage1Scene($"{ScenesFolder}/Stage1.unity");
            CreateStage2Scene($"{ScenesFolder}/Stage2.unity");
            CreateWinScene($"{ScenesFolder}/Win.unity");
            CreateGameOverScene($"{ScenesFolder}/GameOver.unity");
            CreateOptionsScene($"{ScenesFolder}/Options.unity");
            CreateCreditsScene($"{ScenesFolder}/Credits.unity");

            // Register in Build Settings
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene($"{ScenesFolder}/MainMenu.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/StageSelection.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Stage1.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Stage2.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Win.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/GameOver.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Options.unity", true),
                new EditorBuildSettingsScene($"{ScenesFolder}/Credits.unity", true)
            };
            EditorBuildSettings.scenes = buildScenes;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Re-open MainMenu
            EditorSceneManager.OpenScene($"{ScenesFolder}/MainMenu.unity");
            Debug.Log("Successfully generated all 8 scenes and registered in Build Settings!");
        }

        private static void EnsureDirectories()
        {
            if (!Directory.Exists(ScenesFolder)) Directory.CreateDirectory(ScenesFolder);
            if (!Directory.Exists(PrefabsFolder)) Directory.CreateDirectory(PrefabsFolder);
            if (!Directory.Exists(MaterialsFolder)) Directory.CreateDirectory(MaterialsFolder);
        }

        private static Material GetOrCreateMaterial(string name, Color color, float metallic = 0f, float smoothness = 0.5f)
        {
            string path = $"{MaterialsFolder}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Standard");
                mat = new Material(shader);
                mat.color = color;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
                AssetDatabase.CreateAsset(mat, path);
            }
            else
            {
                mat.color = color;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                EditorUtility.SetDirty(mat);
            }
            return mat;
        }

        private static void CreateMaterials()
        {
            GetOrCreateMaterial("Mat_Player", new Color(0.1f, 0.6f, 0.95f), 0.1f, 0.7f); // Blue Cyan
            GetOrCreateMaterial("Mat_Platform", new Color(0.22f, 0.25f, 0.3f), 0.0f, 0.3f); // Dark Slate
            GetOrCreateMaterial("Mat_NarrowBridge", new Color(0.35f, 0.32f, 0.28f), 0.0f, 0.2f); // Wood/Stone
            GetOrCreateMaterial("Mat_ScoreBonus", new Color(1f, 0.85f, 0.1f), 0.8f, 0.8f); // Gold
            GetOrCreateMaterial("Mat_ScorePenalty", new Color(0.6f, 0.1f, 0.6f), 0.2f, 0.6f); // Purple
            GetOrCreateMaterial("Mat_TimeBonus", new Color(0.1f, 0.9f, 0.4f), 0.3f, 0.8f); // Emerald Green
            GetOrCreateMaterial("Mat_TimePenalty", new Color(0.95f, 0.2f, 0.2f), 0.2f, 0.5f); // Danger Red
            AssetDatabase.SaveAssets();
        }

        private static void CreateItemPrefabs()
        {
            // 1. Score Bonus Prefab (+500)
            CreateSingleItemPrefab("Item_ScoreBonus", PrimitiveType.Cylinder, new Vector3(0.7f, 0.15f, 0.7f),
                Quaternion.Euler(90f, 0f, 0f), "Mat_ScoreBonus", ItemEffectType.ScoreBonus, 0f, 500);

            // 2. Score Penalty Prefab (-500)
            CreateSingleItemPrefab("Item_ScorePenalty", PrimitiveType.Cube, new Vector3(0.6f, 0.6f, 0.6f),
                Quaternion.Euler(45f, 45f, 0f), "Mat_ScorePenalty", ItemEffectType.ScorePenalty, 0f, 500);

            // 3. Time Bonus Prefab (+3s)
            CreateSingleItemPrefab("Item_TimeBonus", PrimitiveType.Capsule, new Vector3(0.45f, 0.6f, 0.45f),
                Quaternion.identity, "Mat_TimeBonus", ItemEffectType.TimeBonus, 3f, 0);

            // 4. Time Penalty Prefab (-5s)
            CreateSingleItemPrefab("Item_TimePenalty", PrimitiveType.Cylinder, new Vector3(0.55f, 0.55f, 0.55f),
                Quaternion.identity, "Mat_TimePenalty", ItemEffectType.TimePenalty, 5f, 0);

            AssetDatabase.SaveAssets();
        }

        private static void CreateSingleItemPrefab(string prefabName, PrimitiveType primType, Vector3 scale,
            Quaternion rot, string matName, ItemEffectType type, float timeMod, int scoreMod)
        {
            string path = $"{PrefabsFolder}/{prefabName}.prefab";
            GameObject tempGO = GameObject.CreatePrimitive(primType);
            tempGO.name = prefabName;
            tempGO.transform.localScale = scale;
            tempGO.transform.localRotation = rot;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/{matName}.mat");
            if (mat != null)
            {
                tempGO.GetComponent<MeshRenderer>().sharedMaterial = mat;
            }

            Collider col = tempGO.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            GameItem item = tempGO.AddComponent<GameItem>();
            item.Configure(type, timeMod, scoreMod);

            PrefabUtility.SaveAsPrefabAsset(tempGO, path);
            GameObject.DestroyImmediate(tempGO);
        }

        private static Canvas CreateBaseCanvas(string sceneName, Color bgColor)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            GameObject camGO = new GameObject("Main Camera");
            Camera cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = bgColor;
            cam.orthographic = true;
            camGO.AddComponent<AudioListener>();

            // EventSystem with InputSystemUIInputModule for New Input System support!
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

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
            Canvas canvas = CreateBaseCanvas("MainMenu", new Color(0.12f, 0.15f, 0.2f));
            GameObject cGO = canvas.gameObject;

            MainMenuUI uiComp = cGO.AddComponent<MainMenuUI>();

            // AudioManager Singleton in MainMenu
            GameObject audioGO = new GameObject("AudioManager");
            AudioManager audioMgr = audioGO.AddComponent<AudioManager>();
            AudioClip bgm = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/PracticalExamResources/Sounds/BGM/Tiny_Blocks.wav");
            AudioClip posSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/PracticalExamResources/Sounds/SFX/Coin Pickup 4.wav");
            AudioClip negSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/PracticalExamResources/Sounds/SFX/Debuff Downgrade 1.wav");
            AudioClip winSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/PracticalExamResources/Sounds/SFX/Win sound 1.wav");
            AudioClip loseSFX = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/PracticalExamResources/Sounds/SFX/Game Over 2.wav");
            audioMgr.SetClips(bgm, posSFX, negSFX, winSFX, loseSFX);

            // Title Label
            CreateText(cGO, "TitleLabel", "168-958321-Practical Examination", new Vector2(0, 250), new Vector2(800, 60), 36, TextAlignmentOptions.Center, Color.white);

            // Student Info
            TMP_Text studentInfo = CreateText(cGO, "StudentInfo", $"Student Code: {GameData.StudentCode}\nName: {GameData.StudentNameEnglish}", new Vector2(0, 180), new Vector2(800, 70), 22, TextAlignmentOptions.Center, new Color(0.9f, 0.9f, 0.9f));
            SerializedObject soUI = new SerializedObject(uiComp);
            soUI.FindProperty("studentInfoText").objectReferenceValue = studentInfo;
            soUI.ApplyModifiedProperties();

            // Scene Label (Fig. 1 requirement)
            CreateText(cGO, "SceneLabel", "Main Menu Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.yellow);

            // Buttons Container
            Color btnColor = new Color(0.85f, 0.88f, 0.92f);
            CreateButton(cGO, "BtnStart", "Start", new Vector2(0, 50), new Vector2(260, 50), btnColor);
            CreateButton(cGO, "BtnOptions", "Options", new Vector2(0, -20), new Vector2(260, 50), btnColor);
            CreateButton(cGO, "BtnCredits", "Credits", new Vector2(0, -90), new Vector2(260, 50), btnColor);
            CreateButton(cGO, "BtnExit", "Exit", new Vector2(0, -160), new Vector2(260, 50), new Color(0.95f, 0.6f, 0.6f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateStageSelectionScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("StageSelection", new Color(0.15f, 0.17f, 0.22f));
            GameObject cGO = canvas.gameObject;

            StageSelectionUI uiComp = cGO.AddComponent<StageSelectionUI>();

            // Header
            CreateText(cGO, "TitleLabel", "Stage Selection", new Vector2(0, 250), new Vector2(800, 60), 38, TextAlignmentOptions.Center, Color.white);
            CreateText(cGO, "SceneLabel", "Stage Selection Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.yellow);

            TMP_Text studentInfo = CreateText(cGO, "StudentInfo", $"Student Code: {GameData.StudentCode} Name: {GameData.StudentName}", new Vector2(0, 180), new Vector2(800, 50), 22, TextAlignmentOptions.Center, Color.lightGray);
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("studentInfoText").objectReferenceValue = studentInfo;
            so.ApplyModifiedProperties();

            // Buttons
            Color btnColor = new Color(0.85f, 0.88f, 0.95f);
            CreateButton(cGO, "BtnStage1", "Stage 1 - Capsule Player", new Vector2(0, 40), new Vector2(380, 55), btnColor);
            CreateButton(cGO, "BtnStage2", "Stage 2 - Humanoid Player", new Vector2(0, -35), new Vector2(380, 55), btnColor);
            CreateButton(cGO, "BtnBack", "Back", new Vector2(-320, -250), new Vector2(160, 45), new Color(0.75f, 0.75f, 0.75f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateStage1Scene(string path)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Light
            GameObject lightGO = new GameObject("Directional Light");
            Light light = lightGO.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.intensity = 1.2f;
            lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Materials
            Material matPlatform = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Mat_Platform.mat");
            Material matNarrow = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Mat_NarrowBridge.mat");
            Material matPlayer = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Mat_Player.mat");

            // --- PROBUILDER LEVEL DESIGN (Q.3) ---
            GameObject levelRoot = new GameObject("Level_ProBuilder");

            // 1. Spawn Platform (Wide: 10m x 10m)
            CreatePBPlatform("Platform_Spawn", new Vector3(0, -0.5f, 0), new Vector3(10f, 1f, 10f), matPlatform, levelRoot.transform);

            // 2. Narrow Bridge 1 (Width: 2.2m, Length: 14m)
            CreatePBPlatform("Bridge_Narrow1", new Vector3(0, -0.5f, 12f), new Vector3(2.2f, 1f, 14f), matNarrow, levelRoot.transform);

            // 3. Mid Platform (Wide: 8m x 8m)
            CreatePBPlatform("Platform_Mid", new Vector3(0, -0.5f, 23f), new Vector3(8f, 1f, 8f), matPlatform, levelRoot.transform);

            // 4. Left Branch Narrow Bridge (Width: 1.6m, Length: 12m)
            CreatePBPlatform("Bridge_LeftNarrow", new Vector3(-4.5f, -0.5f, 31f), new Vector3(1.6f, 1f, 12f), matNarrow, levelRoot.transform);
            CreatePBPlatform("Platform_LeftSecret", new Vector3(-8f, -0.5f, 39f), new Vector3(6f, 1f, 6f), matPlatform, levelRoot.transform);

            // 5. Right Branch Narrow Bridge (Width: 1.6m, Length: 12m)
            CreatePBPlatform("Bridge_RightNarrow", new Vector3(4.5f, -0.5f, 31f), new Vector3(1.6f, 1f, 12f), matNarrow, levelRoot.transform);
            CreatePBPlatform("Platform_Right", new Vector3(8f, -0.5f, 39f), new Vector3(6f, 1f, 6f), matPlatform, levelRoot.transform);

            // 6. Connector Platform & Elevated Bridge (Ramp to High Ground)
            CreatePBPlatform("Platform_PreFinal", new Vector3(0, -0.5f, 44f), new Vector3(4f, 1f, 4f), matPlatform, levelRoot.transform);
            CreatePBPlatform("Bridge_HighRamp", new Vector3(0, 0.5f, 52f), new Vector3(2.5f, 1f, 12f), matNarrow, levelRoot.transform);

            // 7. Grand Final Arena (Wide: 14m x 14m at Y = 1)
            CreatePBPlatform("Platform_FinalArena", new Vector3(0, 0.5f, 65f), new Vector3(14f, 1f, 14f), matPlatform, levelRoot.transform);

            // Fall Zone (Void trigger)
            GameObject fallZoneGO = new GameObject("FallZone");
            fallZoneGO.transform.position = new Vector3(0f, -8f, 35f);
            BoxCollider fallCol = fallZoneGO.AddComponent<BoxCollider>();
            fallCol.size = new Vector3(250f, 4f, 250f);
            fallCol.isTrigger = true;
            fallZoneGO.AddComponent<FallZone>();

            // --- CAPSULE PLAYER CONFIGURATION (Q.2) ---
            GameObject playerGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerGO.name = "CapsulePlayer";
            playerGO.tag = "Player";
            playerGO.transform.position = new Vector3(0f, 1.1f, 0f);
            if (matPlayer != null) playerGO.GetComponent<MeshRenderer>().sharedMaterial = matPlayer;

            // CharacterController setup
            CharacterController cc = playerGO.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0f, 1f, 0f);

            // CapsulePlayerController
            playerGO.AddComponent<CapsulePlayerController>();

            // Camera Setup
            GameObject camGO = new GameObject("Main Camera");
            Camera cam = camGO.AddComponent<Camera>();
            cam.fieldOfView = 60f;
            camGO.AddComponent<AudioListener>();
            SmoothCameraFollow camFollow = camGO.AddComponent<SmoothCameraFollow>();
            camFollow.SetTarget(playerGO.transform);
            camGO.transform.position = new Vector3(0f, 5.5f, -6f);
            camGO.transform.LookAt(playerGO.transform.position + Vector3.up * 1f);

            // --- ITEMS & OBSTACLES PLACEMENT (Q.4) ---
            GameObject itemsRoot = new GameObject("Items_Prefabs");
            GameObject prefabScoreBonus = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_ScoreBonus.prefab");
            GameObject prefabScorePenalty = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_ScorePenalty.prefab");
            GameObject prefabTimeBonus = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_TimeBonus.prefab");
            GameObject prefabTimePenalty = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_TimePenalty.prefab");

            // Place Score Coins (+500 each, 27 coins = 13,500 total points possible -> >10,000 to win)
            Vector3[] scoreCoinPositions = new Vector3[]
            {
                // Spawn Platform (3)
                new Vector3(0f, 0.8f, 2f), new Vector3(-2f, 0.8f, 3f), new Vector3(2f, 0.8f, 3f),
                // Narrow Bridge 1 (3)
                new Vector3(0f, 0.8f, 8f), new Vector3(0f, 0.8f, 12f), new Vector3(0f, 0.8f, 16f),
                // Mid Platform (2)
                new Vector3(-2f, 0.8f, 23f), new Vector3(2f, 0.8f, 23f),
                // Left Branch (4)
                new Vector3(-7f, 0.8f, 37f), new Vector3(-9f, 0.8f, 37f),
                new Vector3(-7f, 0.8f, 39f), new Vector3(-9f, 0.8f, 39f),
                // Right Branch (3)
                new Vector3(7f, 0.8f, 37f), new Vector3(9f, 0.8f, 37f), new Vector3(8f, 0.8f, 40f),
                // High Ramp (3)
                new Vector3(0f, 1.8f, 48f), new Vector3(0f, 1.8f, 52f), new Vector3(0f, 1.8f, 56f),
                // Final Arena (9)
                new Vector3(-4f, 1.8f, 62f), new Vector3(0f, 1.8f, 62f), new Vector3(4f, 1.8f, 62f),
                new Vector3(-4f, 1.8f, 65f), new Vector3(4f, 1.8f, 65f),
                new Vector3(-4f, 1.8f, 68f), new Vector3(0f, 1.8f, 68f), new Vector3(4f, 1.8f, 68f),
                new Vector3(0f, 1.8f, 70f)
            };

            foreach (var pos in scoreCoinPositions)
            {
                SpawnItem(prefabScoreBonus, pos, itemsRoot.transform);
            }

            // Time Bonus Items (+3s)
            Vector3[] timeBonusPositions = new Vector3[]
            {
                new Vector3(0f, 0.8f, 14f),
                new Vector3(0f, 0.8f, 23f),
                new Vector3(-8f, 0.8f, 39f),
                new Vector3(0f, 1.8f, 65f)
            };
            foreach (var pos in timeBonusPositions)
            {
                SpawnItem(prefabTimeBonus, pos, itemsRoot.transform);
            }

            // Obstacles / Negative Items (TimePenalty -5s, ScorePenalty -500)
            SpawnItem(prefabTimePenalty, new Vector3(-8f, 0.8f, 41f), itemsRoot.transform);
            SpawnItem(prefabTimePenalty, new Vector3(8f, 0.8f, 37f), itemsRoot.transform);
            SpawnItem(prefabTimePenalty, new Vector3(-2f, 1.8f, 66f), itemsRoot.transform);
            SpawnItem(prefabTimePenalty, new Vector3(2f, 1.8f, 66f), itemsRoot.transform);

            SpawnItem(prefabScorePenalty, new Vector3(0f, 0.8f, 26f), itemsRoot.transform);
            SpawnItem(prefabScorePenalty, new Vector3(-3f, 1.8f, 63f), itemsRoot.transform);
            SpawnItem(prefabScorePenalty, new Vector3(3f, 1.8f, 63f), itemsRoot.transform);

            // --- HUD CANVAS (Q.5) ---
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            GameObject canvasGO = new GameObject("Canvas_HUD");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Navigation Manager
            GameObject navGO = new GameObject("SceneNavigationManager");
            navGO.AddComponent<SceneNavigationManager>();

            // Scene Label (Fig. 1 requirement)
            CreateText(canvasGO, "SceneLabel", "Stage 1 - Capsule Player Minigame Scene", new Vector2(-250, 320), new Vector2(450, 40), 20, TextAlignmentOptions.Left, Color.yellow);

            // Timer (Top-Left)
            GameObject timerGO = new GameObject("TimerHUD");
            timerGO.transform.SetParent(canvasGO.transform, false);
            RectTransform timerRT = timerGO.AddComponent<RectTransform>();
            timerRT.anchorMin = new Vector2(0f, 1f);
            timerRT.anchorMax = new Vector2(0f, 1f);
            timerRT.pivot = new Vector2(0f, 1f);
            timerRT.anchoredPosition = new Vector2(30f, -25f);
            timerRT.sizeDelta = new Vector2(250f, 50f);
            TMP_Text timerText = timerGO.AddComponent<TextMeshProUGUI>();
            timerText.text = "Time: 30.0s";
            timerText.fontSize = 32;
            timerText.fontStyle = FontStyles.Bold;
            timerText.color = Color.yellow;

            // Score (Top-Right)
            GameObject scoreGO = new GameObject("ScoreHUD");
            scoreGO.transform.SetParent(canvasGO.transform, false);
            RectTransform scoreRT = scoreGO.AddComponent<RectTransform>();
            scoreRT.anchorMin = new Vector2(1f, 1f);
            scoreRT.anchorMax = new Vector2(1f, 1f);
            scoreRT.pivot = new Vector2(1f, 1f);
            scoreRT.anchoredPosition = new Vector2(-30f, -25f);
            scoreRT.sizeDelta = new Vector2(350f, 50f);
            TMP_Text scoreText = scoreGO.AddComponent<TextMeshProUGUI>();
            scoreText.text = "Score: 0 / 10,000";
            scoreText.fontSize = 28;
            scoreText.fontStyle = FontStyles.Bold;
            scoreText.alignment = TextAlignmentOptions.Right;
            scoreText.color = Color.cyan;

            // Goal Info (Top-Center)
            GameObject goalGO = new GameObject("GoalHUD");
            goalGO.transform.SetParent(canvasGO.transform, false);
            RectTransform goalRT = goalGO.AddComponent<RectTransform>();
            goalRT.anchorMin = new Vector2(0.5f, 1f);
            goalRT.anchorMax = new Vector2(0.5f, 1f);
            goalRT.pivot = new Vector2(0.5f, 1f);
            goalRT.anchoredPosition = new Vector2(0f, -25f);
            goalRT.sizeDelta = new Vector2(400f, 40f);
            TMP_Text goalText = goalGO.AddComponent<TextMeshProUGUI>();
            goalText.text = "Target: >10,000 pts to Win (Fall/Timeout = Lose)";
            goalText.fontSize = 20;
            goalText.alignment = TextAlignmentOptions.Center;
            goalText.color = Color.white;

            // Back to Selection button (Bottom-Left)
            Button btnBack = CreateButton(canvasGO, "BtnBack", "Back", new Vector2(-320, -250), new Vector2(160, 45), new Color(0.8f, 0.8f, 0.8f));
            btnBack.onClick.AddListener(() => SceneNavigationManager.Instance.LoadStageSelection());

            // Stage1Manager setup
            GameObject mgrGO = new GameObject("Stage1Manager");
            Stage1Manager stageMgr = mgrGO.AddComponent<Stage1Manager>();
            SerializedObject soMgr = new SerializedObject(stageMgr);
            soMgr.FindProperty("timerText").objectReferenceValue = timerText;
            soMgr.FindProperty("scoreText").objectReferenceValue = scoreText;
            soMgr.FindProperty("goalText").objectReferenceValue = goalText;
            soMgr.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreatePBPlatform(string name, Vector3 pos, Vector3 size, Material mat, Transform parent)
        {
            ProBuilderMesh pb = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            pb.gameObject.name = name;
            pb.transform.position = pos;
            pb.transform.SetParent(parent);
            if (mat != null) pb.GetComponent<MeshRenderer>().sharedMaterial = mat;

            MeshCollider col = pb.gameObject.GetComponent<MeshCollider>();
            if (col == null) col = pb.gameObject.AddComponent<MeshCollider>();

            pb.ToMesh();
            pb.Refresh();
        }

        private static void SpawnItem(GameObject prefab, Vector3 pos, Transform parent)
        {
            if (prefab == null) return;
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = pos;
            instance.transform.SetParent(parent);
        }

        private static void CreateStage2Scene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Stage2", new Color(0.25f, 0.2f, 0.3f));
            GameObject cGO = canvas.gameObject;

            cGO.AddComponent<StageTestFlowUI>();

            CreateText(cGO, "SceneLabel", "Stage 2 - Humanoid Player Minigame Scene", new Vector2(-250, 320), new Vector2(450, 40), 20, TextAlignmentOptions.Left, Color.yellow);
            CreateText(cGO, "TitleLabel", "Stage 2 - Humanoid Player Minigame", new Vector2(0, 250), new Vector2(800, 60), 32, TextAlignmentOptions.Center, Color.magenta);
            CreateText(cGO, "StudentInfo", $"Student Code: {GameData.StudentCode} Name: {GameData.StudentName}", new Vector2(0, 190), new Vector2(800, 40), 20, TextAlignmentOptions.Center, Color.white);

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

            TMP_Text scoreTxt = CreateText(cGO, "ScoreText", "Score: 0", new Vector2(0, 40), new Vector2(600, 60), 32, TextAlignmentOptions.Center, Color.black);
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("scoreText").objectReferenceValue = scoreTxt;
            so.ApplyModifiedProperties();

            CreateButton(cGO, "BtnTryAgain", "Play Again", new Vector2(0, -40), new Vector2(260, 50), new Color(0.9f, 0.95f, 0.9f));
            CreateButton(cGO, "BtnBackToMain", "Back To Main Menu", new Vector2(0, -110), new Vector2(260, 50), new Color(0.9f, 0.95f, 0.9f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateGameOverScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("GameOver", new Color(0.92f, 0.72f, 0.72f)); // Soft red like Fig 1
            GameObject cGO = canvas.gameObject;

            WinGameOverUI uiComp = cGO.AddComponent<WinGameOverUI>();

            CreateText(cGO, "SceneLabel", "Game Over Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.darkGray);
            CreateText(cGO, "TitleLabel", "Game Over", new Vector2(0, 160), new Vector2(600, 80), 54, TextAlignmentOptions.Center, new Color(0.6f, 0.1f, 0.1f));

            TMP_Text scoreTxt = CreateText(cGO, "ScoreText", "Score: 0", new Vector2(0, 40), new Vector2(600, 60), 32, TextAlignmentOptions.Center, Color.black);
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("scoreText").objectReferenceValue = scoreTxt;
            so.ApplyModifiedProperties();

            CreateButton(cGO, "BtnTryAgain", "Try Again", new Vector2(0, -40), new Vector2(260, 50), new Color(0.98f, 0.9f, 0.9f));
            CreateButton(cGO, "BtnBackToMain", "Back To Main Menu", new Vector2(0, -110), new Vector2(260, 50), new Color(0.98f, 0.9f, 0.9f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static void CreateOptionsScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Options", new Color(0.18f, 0.22f, 0.28f));
            GameObject cGO = canvas.gameObject;

            OptionsMenuUI uiComp = cGO.AddComponent<OptionsMenuUI>();

            // Panel Box
            GameObject panel = new GameObject("OptionsPanel");
            panel.transform.SetParent(cGO.transform, false);
            RectTransform pRT = panel.AddComponent<RectTransform>();
            pRT.sizeDelta = new Vector2(440, 340);
            Image pImg = panel.AddComponent<Image>();
            pImg.color = new Color(0.9f, 0.95f, 1f, 0.95f);

            CreateText(cGO, "SceneLabel", "Options Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.yellow);
            CreateText(panel, "TitleLabel", "Options", new Vector2(0, 100), new Vector2(360, 40), 32, TextAlignmentOptions.Center, Color.black);

            // BGM Label & Slider
            CreateText(panel, "BGMLabel", "BGM Volume", new Vector2(-110, 30), new Vector2(160, 30), 18, TextAlignmentOptions.Left, Color.black);
            Slider bgmSlider = CreateSlider(panel, "BGMSlider", new Vector2(60, 30), new Vector2(180, 20));

            // SFX Label & Slider
            CreateText(panel, "SFXLabel", "SFX Volume", new Vector2(-110, -25), new Vector2(160, 30), 18, TextAlignmentOptions.Left, Color.black);
            Slider sfxSlider = CreateSlider(panel, "SFXSlider", new Vector2(60, -25), new Vector2(180, 20));

            // Bind Sliders to OptionsMenuUI
            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("bgmSlider").objectReferenceValue = bgmSlider;
            so.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
            so.ApplyModifiedProperties();

            // OK / Back Button
            CreateButton(panel, "BtnOK", "Back / OK", new Vector2(0, -95), new Vector2(150, 45), new Color(0.8f, 0.9f, 0.8f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }

        private static Slider CreateSlider(GameObject parent, string name, Vector2 pos, Vector2 size)
        {
            GameObject sliderGO = new GameObject(name);
            sliderGO.transform.SetParent(parent.transform, false);
            RectTransform sRT = sliderGO.AddComponent<RectTransform>();
            sRT.anchoredPosition = pos;
            sRT.sizeDelta = size;
            Slider slider = sliderGO.AddComponent<Slider>();

            // Background
            GameObject bgGO = new GameObject("Background");
            bgGO.transform.SetParent(sliderGO.transform, false);
            RectTransform bgRT = bgGO.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one; bgRT.sizeDelta = Vector2.zero;
            bgGO.AddComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform faRT = fillArea.AddComponent<RectTransform>();
            faRT.anchorMin = Vector2.zero; faRT.anchorMax = Vector2.one; faRT.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fRT = fill.AddComponent<RectTransform>();
            fRT.anchorMin = Vector2.zero; fRT.anchorMax = Vector2.one; fRT.sizeDelta = Vector2.zero;
            Image fImg = fill.AddComponent<Image>();
            fImg.color = new Color(0.2f, 0.5f, 0.9f);
            slider.fillRect = fRT;

            slider.minValue = 0f;
            slider.maxValue = 1f;

            return slider;
        }

        private static void CreateCreditsScene(string path)
        {
            Canvas canvas = CreateBaseCanvas("Credits", new Color(0.15f, 0.18f, 0.22f));
            GameObject cGO = canvas.gameObject;

            CreditsUI uiComp = cGO.AddComponent<CreditsUI>();

            // Panel
            GameObject panel = new GameObject("CreditsPanel");
            panel.transform.SetParent(cGO.transform, false);
            RectTransform pRT = panel.AddComponent<RectTransform>();
            pRT.sizeDelta = new Vector2(560, 380);
            Image pImg = panel.AddComponent<Image>();
            pImg.color = Color.white;

            CreateText(cGO, "SceneLabel", "Credits Scene", new Vector2(-320, 320), new Vector2(300, 40), 20, TextAlignmentOptions.Left, Color.yellow);
            TMP_Text titleTxt = CreateText(panel, "TitleLabel", "Credits", new Vector2(0, 130), new Vector2(400, 45), 32, TextAlignmentOptions.Center, Color.black);
            TMP_Text infoTxt = CreateText(panel, "StudentInfo", $"Course: 958321 Game Development 3\nPractical Examination 1/2569\n\nStudent Code: {GameData.StudentCode}\nName: {GameData.StudentNameEnglish}", new Vector2(0, 20), new Vector2(480, 140), 22, TextAlignmentOptions.Center, new Color(0.2f, 0.2f, 0.2f));

            SerializedObject so = new SerializedObject(uiComp);
            so.FindProperty("creditsDetailText").objectReferenceValue = titleTxt;
            so.FindProperty("studentInfoText").objectReferenceValue = infoTxt;
            so.ApplyModifiedProperties();

            CreateButton(panel, "BtnBack", "Back", new Vector2(0, -130), new Vector2(160, 45), new Color(0.85f, 0.85f, 0.85f));

            EditorSceneManager.SaveScene(canvas.gameObject.scene, path);
        }
    }
}
