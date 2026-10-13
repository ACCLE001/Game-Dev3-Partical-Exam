using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;
using StarterAssets;

namespace ThanethHamburut.EditorTools
{
    public class Stage2PlayerSetupTool
    {
        private const string Ch02ModelPath = "Assets/PracticalExamResources/Character Models/Ch02_nonPBR.fbx";
        private const string PlayerArmaturePrefabPath = "Assets/Starter Assets/Runtime/ThirdPersonController/Prefabs/PlayerArmature.prefab";
        private const string PlayerFollowCameraPrefabPath = "Assets/Starter Assets/Runtime/ThirdPersonController/Prefabs/PlayerFollowCamera.prefab";
        private const string MainCameraPrefabPath = "Assets/Starter Assets/Runtime/ThirdPersonController/Prefabs/MainCamera.prefab";
        private const string TargetPlayerPrefabPath = "Assets/Prefabs/HumanoidPlayer.prefab";
        private const string Stage2ScenePath = "Assets/Scenes/Stage2.unity";

        // Manual menu item only - do not auto-run on Editor startup
        [MenuItem("Tools/ThanethHamburut/Configure Stage 2 Player (Q6)")]
        public static void ConfigureStage2Player()
        {
            EnsureDirectories();
            ConfigureModelImporter();
            GameObject playerPrefab = CreateOrUpdateHumanoidPlayerPrefab();
            SetupStage2Scene(playerPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Stage2PlayerSetupTool] Successfully configured Stage 2 Humanoid Player with Starter Assets, Audio, and Interaction Detector (Q.6)!");
        }

        private static void EnsureDirectories()
        {
            if (!Directory.Exists("Assets/Prefabs"))
            {
                Directory.CreateDirectory("Assets/Prefabs");
            }
        }

        private static void ConfigureModelImporter()
        {
            ModelImporter importer = AssetImporter.GetAtPath(Ch02ModelPath) as ModelImporter;
            if (importer != null)
            {
                bool needReimport = false;
                if (importer.animationType != ModelImporterAnimationType.Human)
                {
                    importer.animationType = ModelImporterAnimationType.Human;
                    importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
                    needReimport = true;
                }

                if (needReimport)
                {
                    importer.SaveAndReimport();
                    Debug.Log("[Stage2PlayerSetupTool] Configured Ch02_nonPBR as Humanoid with Avatar!");
                }
            }
        }

        private static GameObject CreateOrUpdateHumanoidPlayerPrefab()
        {
            // Load base StarterAssets PlayerArmature
            GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerArmaturePrefabPath);
            if (basePrefab == null)
            {
                Debug.LogError($"[Stage2PlayerSetupTool] Could not find base PlayerArmature at {PlayerArmaturePrefabPath}");
                return null;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            instance.name = "HumanoidPlayer";
            instance.tag = "Player";

            // Hide the default robot mesh
            Transform defaultMesh = instance.transform.Find("Armature_Mesh");
            if (defaultMesh != null)
            {
                defaultMesh.gameObject.SetActive(false);
            }

            // Instantiate Ch02 character model inside player
            GameObject ch02Model = AssetDatabase.LoadAssetAtPath<GameObject>(Ch02ModelPath);
            if (ch02Model != null)
            {
                Transform existingCh02 = instance.transform.Find("Ch02_nonPBR");
                if (existingCh02 != null)
                {
                    Object.DestroyImmediate(existingCh02.gameObject);
                }

                GameObject ch02Instance = (GameObject)PrefabUtility.InstantiatePrefab(ch02Model);
                ch02Instance.name = "Ch02_nonPBR";
                ch02Instance.transform.SetParent(instance.transform, false);
                ch02Instance.transform.localPosition = Vector3.zero;
                ch02Instance.transform.localRotation = Quaternion.identity;

                // Disable child Animator so root drives motion
                Animator ch02Anim = ch02Instance.GetComponent<Animator>();
                if (ch02Anim != null)
                {
                    ch02Anim.enabled = false;
                }
            }

            // Assign Ch02 Avatar to root Animator if available
            Animator rootAnim = instance.GetComponent<Animator>();
            if (rootAnim != null)
            {
                Avatar ch02Avatar = null;
                Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(Ch02ModelPath);
                foreach (var asset in subAssets)
                {
                    if (asset is Avatar av && av.isHuman)
                    {
                        ch02Avatar = av;
                        break;
                    }
                }

                if (ch02Avatar != null)
                {
                    rootAnim.avatar = ch02Avatar;
                }
            }

            // Configure ThirdPersonController audio clips
            ThirdPersonController tpc = instance.GetComponent<ThirdPersonController>();
            if (tpc != null)
            {
                AudioClip[] footsteps = new AudioClip[10];
                for (int i = 1; i <= 10; i++)
                {
                    string path = $"Assets/Starter Assets/Runtime/ThirdPersonController/Character/Sfx/Player_Footstep_{i:D2}.wav";
                    footsteps[i - 1] = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                }
                tpc.FootstepAudioClips = footsteps;
                tpc.LandingAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Starter Assets/Runtime/ThirdPersonController/Character/Sfx/Player_Land.wav");
                tpc.FootstepAudioVolume = 0.6f;
            }

            // Add Interaction Detector for Q.6
            PlayerInteractionDetector detector = instance.GetComponent<PlayerInteractionDetector>();
            if (detector == null)
            {
                detector = instance.AddComponent<PlayerInteractionDetector>();
            }

            // Save as HumanoidPlayer prefab
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, TargetPlayerPrefabPath);
            Object.DestroyImmediate(instance);

            return savedPrefab;
        }

        private static void SetupStage2Scene(GameObject playerPrefab)
        {
            var scene = EditorSceneManager.OpenScene(Stage2ScenePath);

            // 1. Directional Light
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

            // 2. Test Ground Platform (temporary testing ground for Q.6; Q.7 ProBuilder level will be done separately)
            Transform existingGround = GameObject.Find("TestGround")?.transform;
            if (existingGround == null)
            {
                GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ground.name = "TestGround";
                ground.transform.position = new Vector3(0f, -0.5f, 0f);
                ground.transform.localScale = new Vector3(20f, 1f, 20f);

                Material matGround = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Platform.mat");
                if (matGround != null)
                {
                    ground.GetComponent<MeshRenderer>().sharedMaterial = matGround;
                }
            }

            // 3. Spawn Humanoid Player
            GameObject existingPlayer = GameObject.FindWithTag("Player");
            if (existingPlayer != null)
            {
                Object.DestroyImmediate(existingPlayer);
            }

            GameObject spawnedPlayer = null;
            if (playerPrefab != null)
            {
                spawnedPlayer = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                spawnedPlayer.name = "HumanoidPlayer";
                spawnedPlayer.transform.position = new Vector3(0f, 0f, 0f);
            }

            // 4. Follow Camera & Main Camera Setup
            Transform cameraRoot = spawnedPlayer != null ? spawnedPlayer.transform.Find("PlayerCameraRoot") : null;

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
            else
            {
                var brain = mainCam.GetComponent<Unity.Cinemachine.CinemachineBrain>();
                if (brain == null)
                {
                    mainCam.gameObject.AddComponent<Unity.Cinemachine.CinemachineBrain>();
                }
            }

            // PlayerFollowCamera
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

            // 5. Stage 2 Audio Setup (Plays BGM)
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

            EditorSceneManager.SaveScene(scene);
        }
    }
}
