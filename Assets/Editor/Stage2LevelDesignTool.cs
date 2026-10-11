using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using System.IO;

namespace ThanethHamburut.EditorTools
{
    public class Stage2LevelDesignTool
    {
        private const string Stage2ScenePath = "Assets/Scenes/Stage2.unity";
        private const string PrefabsFolder = "Assets/Prefabs/Items";
        private const string MaterialsFolder = "Assets/Materials";
        private const string PlayerPrefabPath = "Assets/Prefabs/HumanoidPlayer.prefab";
        private const string PlayerFollowCameraPrefabPath = "Assets/Starter Assets/Runtime/ThirdPersonController/Prefabs/PlayerFollowCamera.prefab";
        private const string MainCameraPrefabPath = "Assets/Starter Assets/Runtime/ThirdPersonController/Prefabs/MainCamera.prefab";

        [InitializeOnLoadMethod]
        private static void AutoRunOnCompile()
        {
            EditorApplication.delayCall += () =>
            {
                if (!SessionState.GetBool("Stage2LevelDesign_Q7_Done", false))
                {
                    SessionState.SetBool("Stage2LevelDesign_Q7_Done", true);
                    BuildStage2Level();
                }
            };
        }

        [MenuItem("Tools/ThanethHamburut/Build Stage 2 Level (Q7)")]
        public static void BuildStage2Level()
        {
            var scene = EditorSceneManager.OpenScene(Stage2ScenePath);

            // Clean up temporary TestGround from previous day if it exists
            GameObject oldGround = GameObject.Find("TestGround");
            if (oldGround != null)
            {
                Object.DestroyImmediate(oldGround);
            }

            // Remove previous ProBuilder level root if re-running
            GameObject oldLevel = GameObject.Find("Level_ProBuilder_Stage2");
            if (oldLevel != null)
            {
                Object.DestroyImmediate(oldLevel);
            }

            // Remove old Items root if re-running
            GameObject oldItems = GameObject.Find("Items_Prefabs_Stage2");
            if (oldItems != null)
            {
                Object.DestroyImmediate(oldItems);
            }

            // 1. Materials
            Material matPlatform = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Mat_Platform.mat");
            Material matBridge = AssetDatabase.LoadAssetAtPath<Material>($"{MaterialsFolder}/Mat_NarrowBridge.mat");

            // 2. Build ProBuilder Level (Q.7: Size >= 50 square meters)
            // Total area here is over 700 sq.m.!
            GameObject levelRoot = new GameObject("Level_ProBuilder_Stage2");

            // Central Plaza (12m x 12m = 144 sq.m.)
            CreatePBPlatform("PB_CenterPlaza", new Vector3(0f, -0.5f, 0f), new Vector3(12f, 1f, 12f), matPlatform, levelRoot.transform);

            // North Pathway & North Platform
            CreatePBPlatform("PB_NorthPathway", new Vector3(0f, -0.5f, 11f), new Vector3(4f, 1f, 10f), matBridge, levelRoot.transform);
            CreatePBPlatform("PB_NorthPlatform", new Vector3(0f, -0.5f, 21f), new Vector3(10f, 1f, 10f), matPlatform, levelRoot.transform);

            // South Pathway & South Platform
            CreatePBPlatform("PB_SouthPathway", new Vector3(0f, -0.5f, -11f), new Vector3(4f, 1f, 10f), matBridge, levelRoot.transform);
            CreatePBPlatform("PB_SouthPlatform", new Vector3(0f, -0.5f, -21f), new Vector3(10f, 1f, 10f), matPlatform, levelRoot.transform);

            // East Bridge & East Platform
            CreatePBPlatform("PB_EastBridge", new Vector3(11f, -0.5f, 0f), new Vector3(10f, 1f, 4f), matBridge, levelRoot.transform);
            CreatePBPlatform("PB_EastPlatform", new Vector3(21f, -0.5f, 0f), new Vector3(10f, 1f, 10f), matPlatform, levelRoot.transform);

            // West Bridge & West Platform
            CreatePBPlatform("PB_WestBridge", new Vector3(-11f, -0.5f, 0f), new Vector3(10f, 1f, 4f), matBridge, levelRoot.transform);
            CreatePBPlatform("PB_WestPlatform", new Vector3(-21f, -0.5f, 0f), new Vector3(10f, 1f, 10f), matPlatform, levelRoot.transform);

            // Fall Zone (Underneath the level at Y = -8)
            GameObject fallZoneGO = GameObject.Find("FallZone_Stage2");
            if (fallZoneGO == null)
            {
                fallZoneGO = new GameObject("FallZone_Stage2");
                fallZoneGO.transform.position = new Vector3(0f, -8f, 0f);
                BoxCollider fallCol = fallZoneGO.AddComponent<BoxCollider>();
                fallCol.size = new Vector3(200f, 4f, 200f);
                fallCol.isTrigger = true;
            }

            // 3. Ensure Humanoid Player exists and is positioned at spawn
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
                if (playerPrefab != null)
                {
                    player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                    player.name = "HumanoidPlayer";
                }
            }
            if (player != null)
            {
                player.transform.position = new Vector3(0f, 0.05f, 0f);
            }

            // 4. Ensure Cameras and tracking target
            Transform cameraRoot = player != null ? player.transform.Find("PlayerCameraRoot") : null;
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject mainCamPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MainCameraPrefabPath);
                if (mainCamPrefab != null)
                {
                    GameObject camObj = (GameObject)PrefabUtility.InstantiatePrefab(mainCamPrefab);
                    camObj.name = "MainCamera";
                }
            }

            var vcam = Object.FindAnyObjectByType<Unity.Cinemachine.CinemachineCamera>();
            if (vcam == null)
            {
                GameObject vcamPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerFollowCameraPrefabPath);
                if (vcamPrefab != null)
                {
                    GameObject vcamObj = (GameObject)PrefabUtility.InstantiatePrefab(vcamPrefab);
                    vcamObj.name = "PlayerFollowCamera";
                    vcam = vcamObj.GetComponent<Unity.Cinemachine.CinemachineCamera>();
                }
            }
            if (vcam != null && cameraRoot != null)
            {
                vcam.Target.TrackingTarget = cameraRoot;
            }

            // 5. Ensure Directional Light
            Light dirLight = Object.FindAnyObjectByType<Light>();
            if (dirLight == null)
            {
                GameObject lightGO = new GameObject("Directional Light");
                Light light = lightGO.AddComponent<Light>();
                light.type = LightType.Directional;
                light.intensity = 1.2f;
                light.color = Color.white;
                lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // 6. Ensure Audio Setup
            GameObject audioSetupGO = GameObject.Find("Stage2AudioSetup");
            if (audioSetupGO == null)
            {
                audioSetupGO = new GameObject("Stage2AudioSetup");
                Stage2AudioSetup audioSetup = audioSetupGO.AddComponent<Stage2AudioSetup>();
                AudioClip stage2Clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/PracticalExamResources/Sounds/BGM/Tiny_Blocks.wav");
                SerializedObject soAudio = new SerializedObject(audioSetup);
                soAudio.FindProperty("stage2BGM").objectReferenceValue = stage2Clip;
                soAudio.ApplyModifiedProperties();
            }

            // 7. Place Item Prefabs (Q.7 requirement: Spread items across map sufficient for winning condition >10,000 pts)
            GameObject itemsRoot = new GameObject("Items_Prefabs_Stage2");
            GameObject prefabScoreBonus = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_ScoreBonus.prefab");
            GameObject prefabScorePenalty = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_ScorePenalty.prefab");
            GameObject prefabTimeBonus = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_TimeBonus.prefab");
            GameObject prefabTimePenalty = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabsFolder}/Item_TimePenalty.prefab");

            // 29 Score Coins (+500 each = 14,500 total points possible > 10,000 pts)
            Vector3[] scorePositions = new Vector3[]
            {
                // Central Plaza (5)
                new Vector3(0f, 0.8f, 3f), new Vector3(3f, 0.8f, 0f), new Vector3(-3f, 0.8f, 0f),
                new Vector3(2.5f, 0.8f, 2.5f), new Vector3(-2.5f, 0.8f, 2.5f),

                // North Pathway & Platform (1 + 5 = 6)
                new Vector3(0f, 0.8f, 11f),
                new Vector3(0f, 0.8f, 18f), new Vector3(2.5f, 0.8f, 21f), new Vector3(-2.5f, 0.8f, 21f),
                new Vector3(0f, 0.8f, 24f), new Vector3(2.5f, 0.8f, 24f),

                // South Pathway & Platform (1 + 5 = 6)
                new Vector3(0f, 0.8f, -11f),
                new Vector3(0f, 0.8f, -18f), new Vector3(2.5f, 0.8f, -21f), new Vector3(-2.5f, 0.8f, -21f),
                new Vector3(0f, 0.8f, -24f), new Vector3(-2.5f, 0.8f, -24f),

                // East Bridge & Platform (1 + 5 = 6)
                new Vector3(11f, 0.8f, 0f),
                new Vector3(18f, 0.8f, 0f), new Vector3(21f, 0.8f, 2.5f), new Vector3(21f, 0.8f, -2.5f),
                new Vector3(24f, 0.8f, 0f), new Vector3(24f, 0.8f, 2.5f),

                // West Bridge & Platform (1 + 5 = 6)
                new Vector3(-11f, 0.8f, 0f),
                new Vector3(-18f, 0.8f, 0f), new Vector3(-21f, 0.8f, 2.5f), new Vector3(-21f, 0.8f, -2.5f),
                new Vector3(-24f, 0.8f, 0f), new Vector3(-24f, 0.8f, -2.5f)
            };

            foreach (var pos in scorePositions)
            {
                SpawnItem(prefabScoreBonus, pos, itemsRoot.transform);
            }

            // Time Bonus Items (+3s)
            Vector3[] timeBonusPositions = new Vector3[]
            {
                new Vector3(0f, 0.8f, 21f),   // North Platform center
                new Vector3(0f, 0.8f, -21f),  // South Platform center
                new Vector3(21f, 0.8f, 0f),   // East Platform center
                new Vector3(-21f, 0.8f, 0f)   // West Platform center
            };
            foreach (var pos in timeBonusPositions)
            {
                SpawnItem(prefabTimeBonus, pos, itemsRoot.transform);
            }

            // Obstacles & Penalties
            SpawnItem(prefabTimePenalty, new Vector3(-2.5f, 0.8f, 24f), itemsRoot.transform);
            SpawnItem(prefabTimePenalty, new Vector3(24f, 0.8f, -2.5f), itemsRoot.transform);
            SpawnItem(prefabScorePenalty, new Vector3(2.5f, 0.8f, -24f), itemsRoot.transform);
            SpawnItem(prefabScorePenalty, new Vector3(-24f, 0.8f, 2.5f), itemsRoot.transform);

            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Stage2LevelDesignTool] Successfully created ProBuilder Level for Stage 2 (Area > 700 sq.m. >= 50 sq.m. required) with 29 Score Coins, Time Bonuses, and Obstacles (Q.7)!");
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
    }
}
