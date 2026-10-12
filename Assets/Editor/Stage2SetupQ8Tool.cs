using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace ThanethHamburut.EditorTools
{
    public static class Stage2SetupQ8Tool
    {
        [MenuItem("Tools/Thaneth Hamburut/Setup Stage 2 Q8 and Performance")]
        public static void SetupStage2()
        {
            string scenePath = "Assets/Scenes/Stage2.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. Performance Optimization: Enable GPU Instancing on all Materials
            string[] matGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" });
            foreach (var guid in matGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null)
                {
                    mat.enableInstancing = true;
                    EditorUtility.SetDirty(mat);
                }
            }

            // 2. Performance Optimization: Set Static Batching Flags on ProBuilder Level
            var levelGO = GameObject.Find("Level_ProBuilder_Stage2");
            if (levelGO != null)
            {
                SetStaticRecursively(levelGO);
            }

            // 3. Canvas & UI Setup
            var canvasGO = GameObject.Find("Canvas");
            if (canvasGO == null)
            {
                canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                var c = canvasGO.GetComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;
            }

            // 4. Reposition Title and Student Info at Top Center (not cut off)
            var titleGO = GameObject.Find("TitleLabel");
            if (titleGO != null)
            {
                var rt = titleGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0, -15);
                rt.sizeDelta = new Vector2(700, 40);
                var tmp = titleGO.GetComponent<TMP_Text>();
                if (tmp != null)
                {
                    tmp.fontSize = 28;
                    tmp.alignment = TextAlignmentOptions.Top;
                }
            }

            var infoGO = GameObject.Find("StudentInfo");
            if (infoGO != null)
            {
                var rt = infoGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0, -58);
                rt.sizeDelta = new Vector2(500, 45);
                var tmp = infoGO.GetComponent<TMP_Text>();
                if (tmp != null)
                {
                    tmp.fontSize = 20;
                    tmp.text = "Student Code: 672110095\nName: Thaneth Hamburut";
                    tmp.alignment = TextAlignmentOptions.Top;
                }
            }

            // Back button
            var backGO = GameObject.Find("BtnBack");
            if (backGO != null)
            {
                var rt = backGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(25, -20);
                rt.sizeDelta = new Vector2(130, 40);
            }

            // Move Test Win & Test Lose to right margin (neat and matching fig. 1, not blocking player!)
            var winBtnGO = GameObject.Find("BtnTestWin");
            if (winBtnGO != null)
            {
                var rt = winBtnGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 0.5f);
                rt.anchorMax = new Vector2(1f, 0.5f);
                rt.pivot = new Vector2(1f, 0.5f);
                rt.anchoredPosition = new Vector2(-25, 30);
                rt.sizeDelta = new Vector2(170, 42);
            }

            var loseBtnGO = GameObject.Find("BtnTestLose");
            if (loseBtnGO != null)
            {
                var rt = loseBtnGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1f, 0.5f);
                rt.anchorMax = new Vector2(1f, 0.5f);
                rt.pivot = new Vector2(1f, 0.5f);
                rt.anchoredPosition = new Vector2(-25, -30);
                rt.sizeDelta = new Vector2(170, 42);
            }

            // 5. Create / Configure HUD elements
            TMP_Text timerText = GetOrCreateTMP(canvasGO, "TimerHUD", new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -75), new Vector2(260, 40), 26, Color.yellow, "Time: 30.0s");
            TMP_Text scoreText = GetOrCreateTMP(canvasGO, "ScoreHUD", new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -120), new Vector2(320, 40), 26, Color.green, "Score: 0 / 10,000");
            TMP_Text goalText = GetOrCreateTMP(canvasGO, "GoalHUD", new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(25, -165), new Vector2(380, 35), 20, Color.white, "Target: >10,000 to Win");

            // 6. Interaction Prompt HUD (shows "[E] Collect Item" at bottom center)
            GameObject promptPanel = null;
            Transform existingPanel = canvasGO.transform.Find("InteractionPromptPanel");
            if (existingPanel != null)
            {
                promptPanel = existingPanel.gameObject;
            }
            else
            {
                promptPanel = new GameObject("InteractionPromptPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                promptPanel.transform.SetParent(canvasGO.transform, false);
            }
            var panelRT = promptPanel.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.5f, 0f);
            panelRT.anchorMax = new Vector2(0.5f, 0f);
            panelRT.pivot = new Vector2(0.5f, 0f);
            panelRT.anchoredPosition = new Vector2(0, 90);
            panelRT.sizeDelta = new Vector2(420, 50);
            var panelImg = promptPanel.GetComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.8f);

            TMP_Text promptText = GetOrCreateTMP(promptPanel, "InteractionPromptText", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, 22, Color.cyan, "[E] Collect Item");
            promptText.alignment = TextAlignmentOptions.Center;
            promptPanel.SetActive(false);

            // 7. Setup Stage2Manager
            var mgrGO = GameObject.Find("Stage2Manager");
            if (mgrGO == null)
            {
                mgrGO = new GameObject("Stage2Manager");
            }
            var s2Mgr = mgrGO.GetComponent<Stage2Manager>() ?? mgrGO.AddComponent<Stage2Manager>();

            var so = new SerializedObject(s2Mgr);
            so.FindProperty("timerText").objectReferenceValue = timerText;
            so.FindProperty("scoreText").objectReferenceValue = scoreText;
            so.FindProperty("goalText").objectReferenceValue = goalText;
            so.FindProperty("interactionPromptText").objectReferenceValue = promptText;
            so.FindProperty("interactionPromptPanel").objectReferenceValue = promptPanel;
            if (backGO != null) so.FindProperty("btnBack").objectReferenceValue = backGO.GetComponent<Button>();
            so.ApplyModifiedProperties();

            // 8. HumanoidPlayer configuration
            var playerGO = GameObject.Find("HumanoidPlayer");
            if (playerGO != null)
            {
                if (!playerGO.CompareTag("Player"))
                {
                    playerGO.tag = "Player";
                }
                var detector = playerGO.GetComponent<PlayerInteractionDetector>() ?? playerGO.AddComponent<PlayerInteractionDetector>();
                detector.enabled = true;
                EditorUtility.SetDirty(playerGO);
            }

            // 9. Items configuration: ensure all items have requireInteraction = true (Q.8)
            var allItems = Object.FindObjectsByType<GameItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var item in allItems)
            {
                item.RequireInteraction = true;
                EditorUtility.SetDirty(item);
            }

            // 10. FallZone configuration
            var fallZone = GameObject.Find("FallZone_Stage2");
            if (fallZone != null)
            {
                if (fallZone.GetComponent<FallZone>() == null)
                {
                    fallZone.AddComponent<FallZone>();
                }
                EditorUtility.SetDirty(fallZone);
            }

            // 11. EventSystem verification
            var eventSys = GameObject.Find("EventSystem");
            if (eventSys != null)
            {
                var standalone = eventSys.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                if (standalone != null) Object.DestroyImmediate(standalone);
#if ENABLE_INPUT_SYSTEM
                if (eventSys.GetComponent<InputSystemUIInputModule>() == null)
                {
                    eventSys.AddComponent<InputSystemUIInputModule>();
                }
#endif
                EditorUtility.SetDirty(eventSys);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log($"[Stage2SetupQ8Tool] Successfully set up Stage 2 for Q.8! Items updated: {allItems.Length}, HUD created, UI polished, Performance batching flags configured.");
        }

        private static void SetStaticRecursively(GameObject go)
        {
            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.ReflectionProbeStatic);
            foreach (Transform child in go.transform)
            {
                SetStaticRecursively(child.gameObject);
            }
        }

        private static TMP_Text GetOrCreateTMP(GameObject parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta, float fontSize, Color color, string initialText)
        {
            Transform existing = parent.transform.Find(name);
            GameObject go;
            if (existing != null)
            {
                go = existing.gameObject;
            }
            else
            {
                go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                go.transform.SetParent(parent.transform, false);
            }

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = initialText;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.fontStyle = FontStyles.Bold;
            return tmp;
        }
    }
}
