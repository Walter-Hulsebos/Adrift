using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonGames.Utilities.CGTK
{
    #if UNITY_EDITOR

    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using UnityEditor;
    using UnityEditor.VersionControl;

    /// <summary> Various BuildSettings interactions </summary>
    public static class BuildSceneUtilities
    {
        // time in seconds that we have to wait before we query again when IsReadOnly() is called.
        private const float _MIN_CHECK_WAIT = 3;

        private static float _lastTimeChecked;
        private static bool _cachedReadonlyVal = true;

        /// <summary>
        /// A small container for tracking scene data BuildSettings
        /// </summary>
        public struct BuildScene
        {
            public int BuildIndex;
            public GUID AssetGuid;
            public string AssetPath;
            public EditorBuildSettingsScene Scene;
        }

        /// <summary>
        /// Check if the build settings asset is readonly.
        /// Caches value and only queries state a max of every 'minCheckWait' seconds.
        /// </summary>
        public static bool IsReadOnly
        {
            get
            {
                float __curTime = Time.realtimeSinceStartup;
                float __timeSinceLastCheck = __curTime - _lastTimeChecked;

                if(!(__timeSinceLastCheck > _MIN_CHECK_WAIT))
                {
                    return _cachedReadonlyVal;
                }

                _lastTimeChecked = __curTime;
                _cachedReadonlyVal = QueryBuildSettingsStatus();

                return _cachedReadonlyVal;
            }
        }

        /// <summary>
        /// A blocking call to the Version Control system to see if the build settings asset is readonly.
        /// Use BuildSettingsIsReadOnly for version that caches the value for better responsivenes.
        /// </summary>
        private static bool QueryBuildSettingsStatus()
        {
            // If no version control provider, assume not readonly
            if(UnityEditor.VersionControl.Provider.enabled == false)
            {
                return false;
            }

            // If we cannot checkout, then assume we are not readonly
            if(UnityEditor.VersionControl.Provider.hasCheckoutSupport == false)
            {
                return false;
            }

            //// If offline (and are using a version control provider that requires checkout) we cannot edit.
            //if (UnityEditor.VersionControl.Provider.onlineState == UnityEditor.VersionControl.OnlineState.Offline)
            //    return true;

            // Try to get status for file
            Task __status =
                UnityEditor.VersionControl.Provider.Status(asset: "ProjectSettings/EditorBuildSettings.asset", false);

            __status.Wait();

            // If no status listed we can edit
            if(__status.assetList == null || __status.assetList.Count != 1)
            {
                return true;
            }

            // If is checked out, we can edit
            return !__status.assetList[0].IsState(state: UnityEditor.VersionControl.Asset.States.CheckedOutLocal);
        }

        /// <summary>
        /// For a given Scene Asset object reference, extract its build settings data, including buildIndex.
        /// </summary>
        public static BuildScene GetBuildScene(Object sceneObject)
        {
            BuildScene __entry = new BuildScene()
            {
                BuildIndex = -1,
                AssetGuid = new GUID(hexRepresentation: string.Empty)
            };

            if(sceneObject as SceneAsset == null)
                return __entry;

            __entry.AssetPath = AssetDatabase.GetAssetPath(assetObject: sceneObject);
            __entry.AssetGuid = new GUID(hexRepresentation: AssetDatabase.AssetPathToGUID(path: __entry.AssetPath));

            for(int __index = 0; __index < EditorBuildSettings.scenes.Length; ++__index)
            {
                if(!__entry.AssetGuid.Equals(obj: EditorBuildSettings.scenes[__index].guid))
                {
                    continue;
                }

                __entry.Scene = EditorBuildSettings.scenes[__index];
                __entry.BuildIndex = __index;
                return __entry;
            }

            return __entry;
        }

        /// <summary>
        /// Enable/Disable a given scene in the buildSettings
        /// </summary>
        public static void SetBuildSceneState(in BuildScene buildScene, in bool enabled)
        {
            bool __modified = false;
            EditorBuildSettingsScene[] __scenesToModify = EditorBuildSettings.scenes;
            foreach(EditorBuildSettingsScene __curScene in __scenesToModify)
            {
                if(!__curScene.guid.Equals(buildScene.AssetGuid)) continue;

                __curScene.enabled = enabled;
                __modified = true;
                break;
            }

            if(__modified)
            {
                EditorBuildSettings.scenes = __scenesToModify;
            }
        }

        /// <summary>
        /// Display Dialog to add a scene to build settings
        /// </summary>
        public static void AddBuildScene(in BuildScene buildScene, in bool force = false, bool enabled = true)
        {
            if(force == false)
            {
                int __selection = EditorUtility.DisplayDialogComplex(
                    title: "Add Scene To Build",
                    message: $"You are about to add scene: \n<b>{buildScene.AssetPath}</b> \nTo the Build Settings.",    
                    ok: "Add as Enabled",       // option 0
                    cancel: "Add as Disabled",      // option 1
                    alt: "Cancel (do nothing)"); // option 2

                switch(__selection)
                {
                    case 0: // enabled
                        enabled = true;
                        break;
                    case 1: // disabled
                        enabled = false;
                        break;
                    default:
                        return;
                }
            }

            EditorBuildSettingsScene __newScene = new EditorBuildSettingsScene(guid: buildScene.AssetGuid, enabled: enabled);
            List<EditorBuildSettingsScene> __tempScenes = EditorBuildSettings.scenes.ToList();
            __tempScenes.Add(item: __newScene);
            EditorBuildSettings.scenes = __tempScenes.ToArray();
        }

        /// <summary>
        /// Display Dialog to remove a scene from build settings (or just disable it)
        /// </summary>
        public static void RemoveBuildScene(BuildScene buildScene, bool force = false)
        {
            bool __onlyDisable = false;
            if(force == false)
            {
                int __selection = -1;

                const string __TITLE = "Remove Scene From Build";
                string __details =
                    "You are about to remove the following scene from build settings:\n" +
                    $"{buildScene.AssetPath}\n" +
                    $"buildIndex: {buildScene.BuildIndex}" +
                    "\n\nThis will modify build settings, but the scene asset will remain untouched.";

                const string __CONFIRM = "Remove From Build";
                const string __ALT = "Just Disable";
                const string __CANCEL = "Cancel (do nothing)";

                if(buildScene.Scene.enabled)
                {
                    __details += "\n\nIf you want, you can also just disable it instead.";
                    __selection = EditorUtility.DisplayDialogComplex(title: __TITLE, message: __details, ok: __CONFIRM, cancel: __ALT, alt: __CANCEL);
                }
                else
                {
                    __selection = EditorUtility.DisplayDialog(title: __TITLE, message: __details, ok: __CONFIRM, cancel: __CANCEL) ? 0 : 2;
                }

                switch(__selection)
                {
                    case 0: // remove
                        break;
                    case 1: // disable
                        __onlyDisable = true;
                        break;
                    default:
                        return;
                }
            }

            // User chose to not remove, only disable the scene
            if(__onlyDisable)
            {
                SetBuildSceneState(buildScene: buildScene, false);
            }
            // User chose to fully remove the scene from build settings
            else
            {
                List<EditorBuildSettingsScene> __tempScenes = EditorBuildSettings.scenes.ToList();
                __tempScenes.RemoveAll(match: scene => scene.guid.Equals(obj: buildScene.AssetGuid));
                EditorBuildSettings.scenes = __tempScenes.ToArray();
            }
        }

        /// <summary>
        /// Open the default Unity Build Settings window
        /// </summary>
        public static void OpenBuildSettings()
        {
            EditorWindow.GetWindow(t: typeof(BuildPlayerWindow));
        }
    }
    #endif
}