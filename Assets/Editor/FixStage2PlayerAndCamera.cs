using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ThanethHamburut.EditorTools
{
    public static class FixStage2PlayerAndCamera
    {
        [MenuItem("Tools/Thaneth Hamburut/Fix Stage 2 Player And Camera")]
        public static void FixAll()
        {
            // 1. Load Animation Controller and Avatar
            string controllerPath = "Assets/Starter Assets/Runtime/ThirdPersonController/Character/Animations/StarterAssetsThirdPerson.controller";
            var animController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);

            string fbxPath = "Assets/PracticalExamResources/Character Models/Ch02_nonPBR.fbx";
            Avatar ch02Avatar = null;
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            foreach (var a in subAssets)
            {
                if (a is Avatar av)
                {
                    ch02Avatar = av;
                    break;
                }
            }

            // 2. First fix the HumanoidPlayer prefab asset directly
            string prefabPath = "Assets/Prefabs/HumanoidPlayer.prefab";
            var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot != null)
            {
                var pGeom = prefabRoot.transform.Find("Geometry");
                if (pGeom != null) pGeom.gameObject.SetActive(false);

                foreach (var smr in prefabRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (smr.gameObject.name == "Armature_Mesh")
                    {
                        smr.gameObject.SetActive(false);
                    }
                }

                var pCh02 = prefabRoot.transform.Find("Ch02_nonPBR");
                if (pCh02 != null)
                {
                    pCh02.gameObject.SetActive(true);
                    var pChildAnim = pCh02.GetComponent<Animator>() ?? pCh02.gameObject.AddComponent<Animator>();
                    pChildAnim.enabled = true;
                    if (animController != null) pChildAnim.runtimeAnimatorController = animController;
                    if (ch02Avatar != null) pChildAnim.avatar = ch02Avatar;
                }

                var pRootAnim = prefabRoot.GetComponent<Animator>();
                if (pRootAnim != null) pRootAnim.enabled = false;

                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                PrefabUtility.UnloadPrefabContents(prefabRoot);
                Debug.Log("[FixStage2] Saved changes to HumanoidPlayer.prefab.");
            }

            // 3. Open Stage2 scene and fix player, camera, items, and UI
            string scenePath = "Assets/Scenes/Stage2.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            var playerGO = GameObject.Find("HumanoidPlayer");
            if (playerGO != null)
            {
                var geom = playerGO.transform.Find("Geometry");
                if (geom != null)
                {
                    geom.gameObject.SetActive(false);
                }

                foreach (var smr in playerGO.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    if (smr.gameObject.name == "Armature_Mesh")
                    {
                        smr.gameObject.SetActive(false);
                    }
                }

                var ch02 = playerGO.transform.Find("Ch02_nonPBR");
                if (ch02 != null)
                {
                    ch02.gameObject.SetActive(true);
                    var childAnim = ch02.GetComponent<Animator>() ?? ch02.gameObject.AddComponent<Animator>();
                    childAnim.enabled = true;
                    if (animController != null) childAnim.runtimeAnimatorController = animController;
                    if (ch02Avatar != null) childAnim.avatar = ch02Avatar;
                    EditorUtility.SetDirty(childAnim);
                }

                var rootAnim = playerGO.GetComponent<Animator>();
                if (rootAnim != null)
                {
                    rootAnim.enabled = false;
                    EditorUtility.SetDirty(rootAnim);
                }

                if (!playerGO.CompareTag("Player"))
                {
                    playerGO.tag = "Player";
                }

                var detector = playerGO.GetComponent<PlayerInteractionDetector>() ?? playerGO.AddComponent<PlayerInteractionDetector>();
                detector.enabled = true;

                EditorUtility.SetDirty(playerGO);
                Debug.Log("[FixStage2] Fixed HumanoidPlayer in Stage2 scene (robot hidden, Ch02 animator active).");
            }

            // 4. Destroy or disable ALL Cinemachine cameras in Stage2
            var allVCams = Object.FindObjectsByType<Unity.Cinemachine.CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var vc in allVCams)
            {
                if (vc != null && vc.gameObject != null)
                {
                    Object.DestroyImmediate(vc.gameObject);
                }
            }

            // Also find any leftover objects named PlayerFollowCamera
            GameObject leftoverPFC;
            while ((leftoverPFC = GameObject.Find("PlayerFollowCamera")) != null)
            {
                Object.DestroyImmediate(leftoverPFC);
            }

            // 5. Fix MainCamera - Unpack Prefab so CinemachineBrain is permanently removed!
            var mainCamGO = GameObject.Find("MainCamera");
            if (mainCamGO != null)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(mainCamGO))
                {
                    PrefabUtility.UnpackPrefabInstance(mainCamGO, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                }

                var brains = mainCamGO.GetComponents<Unity.Cinemachine.CinemachineBrain>();
                foreach (var b in brains)
                {
                    Object.DestroyImmediate(b);
                }

                var follow = mainCamGO.GetComponent<SmoothCameraFollow>() ?? mainCamGO.AddComponent<SmoothCameraFollow>();
                if (playerGO != null)
                {
                    follow.Target = playerGO.transform;
                }
                follow.Offset = new Vector3(0f, 3.5f, -5.5f);
                follow.SmoothSpeed = 8f;
                follow.FollowTargetRotation = false;

                Vector3 playerPos = playerGO != null ? playerGO.transform.position : Vector3.zero;
                mainCamGO.transform.position = playerPos + new Vector3(0f, 3.5f, -5.5f);
                mainCamGO.transform.LookAt(playerPos + Vector3.up * 1.2f);

                EditorUtility.SetDirty(follow);
                EditorUtility.SetDirty(mainCamGO);
                Debug.Log("[FixStage2] Unpacked MainCamera, destroyed CinemachineBrain, and wired SmoothCameraFollow to HumanoidPlayer.");
            }

            // 6. Ensure all GameItems in Stage2 require interaction (E key)
            var allItems = Object.FindObjectsByType<GameItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var item in allItems)
            {
                item.RequireInteraction = true;
                EditorUtility.SetDirty(item);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            // 7. Re-apply UI Polish to ensure Stage 2 UI is large and clean
            PolishStage2UI.PolishUI();

            AssetDatabase.SaveAssets();
            Debug.Log("[FixStage2] All Stage 2 fixes and UI polish saved permanently!");
        }
    }
}
