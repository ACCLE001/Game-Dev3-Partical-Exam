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
            string scenePath = "Assets/Scenes/Stage2.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

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

            // 2. Fix HumanoidPlayer in Scene
            var playerGO = GameObject.Find("HumanoidPlayer");
            if (playerGO != null)
            {
                // Disable default robot Geometry mesh
                var geom = playerGO.transform.Find("Geometry");
                if (geom != null)
                {
                    geom.gameObject.SetActive(false);
                    Debug.Log("[FixStage2] Successfully disabled default robot Geometry.");
                }

                // Setup Ch02_nonPBR model and animator
                var ch02 = playerGO.transform.Find("Ch02_nonPBR");
                if (ch02 != null)
                {
                    ch02.gameObject.SetActive(true);
                    var childAnim = ch02.GetComponent<Animator>() ?? ch02.gameObject.AddComponent<Animator>();
                    childAnim.enabled = true;
                    if (animController != null) childAnim.runtimeAnimatorController = animController;
                    if (ch02Avatar != null) childAnim.avatar = ch02Avatar;
                    EditorUtility.SetDirty(childAnim);
                    Debug.Log("[FixStage2] Successfully configured Ch02_nonPBR animator with StarterAssetsThirdPerson and Ch02 Avatar.");
                }

                // On root player, disable root Animator to avoid conflict with child Animator
                var rootAnim = playerGO.GetComponent<Animator>();
                if (rootAnim != null)
                {
                    rootAnim.enabled = false;
                    EditorUtility.SetDirty(rootAnim);
                }

                EditorUtility.SetDirty(playerGO);
            }

            // 3. Fix Camera - Smooth Camera Follow (identical to Stage 1)
            var pfcGO = GameObject.Find("PlayerFollowCamera");
            if (pfcGO != null)
            {
                pfcGO.SetActive(false);
                Debug.Log("[FixStage2] Disabled PlayerFollowCamera.");
            }

            var mainCamGO = GameObject.Find("MainCamera");
            if (mainCamGO != null)
            {
                // Remove broken CinemachineBrain if present
                var brain = mainCamGO.GetComponent("CinemachineBrain");
                if (brain != null)
                {
                    Object.DestroyImmediate(brain);
                }

                // Add or configure SmoothCameraFollow
                var follow = mainCamGO.GetComponent<SmoothCameraFollow>() ?? mainCamGO.AddComponent<SmoothCameraFollow>();
                if (playerGO != null)
                {
                    follow.Target = playerGO.transform;
                }
                follow.Offset = new Vector3(0f, 3.5f, -5.5f);
                follow.SmoothSpeed = 8f;
                follow.FollowTargetRotation = false;

                // Position camera behind player facing forward into level
                Vector3 playerPos = playerGO != null ? playerGO.transform.position : Vector3.zero;
                mainCamGO.transform.position = playerPos + new Vector3(0f, 3.5f, -5.5f);
                mainCamGO.transform.LookAt(playerPos + Vector3.up * 1.2f);

                EditorUtility.SetDirty(mainCamGO);
                Debug.Log("[FixStage2] Configured SmoothCameraFollow on MainCamera behind player.");
            }

            // 4. Also fix the HumanoidPlayer prefab asset directly
            string prefabPath = "Assets/Prefabs/HumanoidPlayer.prefab";
            var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot != null)
            {
                var pGeom = prefabRoot.transform.Find("Geometry");
                if (pGeom != null) pGeom.gameObject.SetActive(false);

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

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();

            Debug.Log("[FixStage2] All fixes completed successfully!");
        }
    }
}
