using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace ThanethHamburut.EditorTools
{
    public static class PolishStage2UI
    {
        [MenuItem("Tools/Thaneth Hamburut/Polish Stage 2 UI To Match Stage 1")]
        public static void PolishUI()
        {
            string scenePath = "Assets/Scenes/Stage2.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // 1. Configure Canvas
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
                // Match Stage 1's responsive & readable scaling
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(960, 540);
                scaler.matchWidthOrHeight = 0.5f;
            }

            // 2. Hide clutter elements from gameplay view (Title, StudentInfo, SceneLabel, Test buttons)
            // Exactly matching Stage 1's clean presentation
            string[] clutterNames = { "TitleLabel", "StudentInfo", "SceneLabel", "BtnTestWin", "BtnTestLose" };
            foreach (var name in clutterNames)
            {
                var go = GameObject.Find(name);
                if (go != null)
                {
                    go.SetActive(false);
                    EditorUtility.SetDirty(go);
                }
            }

            // 3. Setup TimerHUD (Top-Left, Big, Bold, Yellow - exactly like Stage 1)
            TMP_Text timerText = SetupTMPElement(canvasGO, "TimerHUD",
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(30f, -30f), new Vector2(300f, 65f),
                44, FontStyles.Bold, new Color(1f, 0.92f, 0.015f, 1f),
                TextAlignmentOptions.TopLeft, "Time: 30.0s");

            // 4. Setup ScoreHUD (Top-Right, Big, Bold, Cyan - exactly like Stage 1)
            TMP_Text scoreText = SetupTMPElement(canvasGO, "ScoreHUD",
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-30f, -30f), new Vector2(380f, 65f),
                40, FontStyles.Bold, new Color(0f, 1f, 1f, 1f),
                TextAlignmentOptions.TopRight, "Score: 0 / 10,000");

            // 5. Setup GoalHUD (Top-Center, Readable, White - exactly like Stage 1)
            TMP_Text goalText = SetupTMPElement(canvasGO, "GoalHUD",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -25f), new Vector2(650f, 50f),
                22, FontStyles.Bold, Color.white,
                TextAlignmentOptions.Top, "Target: >10,000 pts to Win (Fall/Timeout = Lose)");

            // 6. Setup BtnBack (Bottom-Left, Clean, Prominent - exactly like Stage 1)
            var backGO = GameObject.Find("BtnBack");
            if (backGO == null)
            {
                backGO = new GameObject("BtnBack", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
                backGO.transform.SetParent(canvasGO.transform, false);
            }
            backGO.SetActive(true);
            var backRT = backGO.GetComponent<RectTransform>();
            backRT.anchorMin = new Vector2(0f, 0f);
            backRT.anchorMax = new Vector2(0f, 0f);
            backRT.pivot = new Vector2(0f, 0f);
            backRT.anchoredPosition = new Vector2(30f, 25f);
            backRT.sizeDelta = new Vector2(150f, 48f);

            var backImg = backGO.GetComponent<Image>() ?? backGO.AddComponent<Image>();
            backImg.color = new Color(0.85f, 0.85f, 0.85f, 1f);

            var backBtn = backGO.GetComponent<Button>() ?? backGO.AddComponent<Button>();

            // Setup Text child of BtnBack
            Transform backTextChild = backGO.transform.Find("Text") ?? backGO.transform.Find("Text (TMP)");
            GameObject backTextGO;
            if (backTextChild == null)
            {
                backTextGO = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                backTextGO.transform.SetParent(backGO.transform, false);
            }
            else
            {
                backTextGO = backTextChild.gameObject;
            }
            var btrt = backTextGO.GetComponent<RectTransform>();
            btrt.anchorMin = Vector2.zero;
            btrt.anchorMax = Vector2.one;
            btrt.sizeDelta = Vector2.zero;
            btrt.anchoredPosition = Vector2.zero;

            var btTMP = backTextGO.GetComponent<TextMeshProUGUI>() ?? backTextGO.AddComponent<TextMeshProUGUI>();
            btTMP.text = "Back";
            btTMP.fontSize = 22;
            btTMP.fontStyle = FontStyles.Bold;
            btTMP.color = Color.black;
            btTMP.alignment = TextAlignmentOptions.Center;

            // 7. Setup InteractionPromptPanel (Bottom-Center, Sleek dark box with vibrant text)
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
            panelRT.anchoredPosition = new Vector2(0f, 85f);
            panelRT.sizeDelta = new Vector2(480f, 55f);

            var panelImg = promptPanel.GetComponent<Image>();
            panelImg.color = new Color(0.05f, 0.05f, 0.08f, 0.85f);

            TMP_Text promptText = SetupTMPElement(promptPanel, "InteractionPromptText",
                Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
                Vector2.zero, Vector2.zero,
                26, FontStyles.Bold, new Color(0f, 1f, 1f, 1f),
                TextAlignmentOptions.Center, "[E] Collect Item (+500 pts)");
            promptPanel.SetActive(false);

            // 8. Re-wire Stage2Manager references
            var mgrGO = GameObject.Find("Stage2Manager");
            if (mgrGO != null)
            {
                var s2Mgr = mgrGO.GetComponent<Stage2Manager>();
                if (s2Mgr != null)
                {
                    var so = new SerializedObject(s2Mgr);
                    so.FindProperty("timerText").objectReferenceValue = timerText;
                    so.FindProperty("scoreText").objectReferenceValue = scoreText;
                    so.FindProperty("goalText").objectReferenceValue = goalText;
                    so.FindProperty("interactionPromptText").objectReferenceValue = promptText;
                    so.FindProperty("interactionPromptPanel").objectReferenceValue = promptPanel;
                    so.FindProperty("btnBack").objectReferenceValue = backBtn;
                    so.ApplyModifiedProperties();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[PolishStage2UI] Stage 2 UI successfully redesigned to match Stage 1 perfectly! Large readable fonts, proper anchors, and uncluttered layout applied.");
        }

        private static TMP_Text SetupTMPElement(GameObject parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPos, Vector2 sizeDelta,
            float fontSize, FontStyles fontStyle, Color color,
            TextAlignmentOptions alignment, string text)
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
            go.SetActive(true);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = fontStyle;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.enableWordWrapping = false;
            return tmp;
        }
    }
}
