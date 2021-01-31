namespace CommonGames.Utilities.CGTK
{
    using System;
    using Sirenix.Utilities;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    
    using Object = UnityEngine.Object;
    
    /// <summary>
    /// A wrapper that provides the means to safely serialize Scene Asset References.
    /// </summary>
    [Serializable]
    public sealed class SceneReference : ISerializationCallbackReceiver
    {
        #region Variables

        #if UNITY_EDITOR
        [SerializeField] private Object sceneAsset;
        
        private bool IsValidSceneAsset => sceneAsset is SceneAsset;
        #endif
        
        ///<remarks> This should only ever be set during serialization/deserialization! </remarks> 
        [SerializeField] private string scenePath = string.Empty;

        ///<summary> Use this when you want to actually have the scene path. </summary>
        public string ScenePath
        {
            // At runtime we rely on the stored path value which we assume was serialized correctly at build time.
            get
            {
                return
                    #if UNITY_EDITOR
                    GetScenePathFromAsset;
                    #else
                    scenePath;
                    #endif
            }
            set
            {
                scenePath = value;
                #if UNITY_EDITOR
                sceneAsset = GetSceneAssetFromPath;
                #endif
            }
        }

        #if UNITY_EDITOR

        public Object Scene
        {
            get => sceneAsset;
            set => sceneAsset = value;
        }
        
        private SceneAsset GetSceneAssetFromPath
            => string.IsNullOrEmpty(scenePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath: scenePath);

        private string GetScenePathFromAsset
            => (sceneAsset == null) ? string.Empty : AssetDatabase.GetAssetPath(assetObject: sceneAsset);
        
        #endif
        
        #endregion

        #region Operators

        //public static implicit operator int(SceneReference sceneReference) => sceneReference.ScenePath;
        
        public static implicit operator string(in SceneReference sceneReference) 
            => sceneReference.ScenePath;
        
        #if UNITY_EDITOR
        
        public static implicit operator SceneAsset(in SceneReference sceneReference) 
            => sceneReference.GetSceneAssetFromPath;

        public static implicit operator SceneReference(in SceneAsset sceneAsset) 
            => new SceneReference() { Scene = sceneAsset };

        #endif

        #region Comparison

        public static bool operator ==(in Scene scene, in SceneReference sceneReference)
            => sceneReference?.Equals(scene) ?? false;
        public static bool operator !=(in Scene scene, in SceneReference sceneReference)
            => !sceneReference?.Equals(scene) ?? true;

        public static bool operator ==(in SceneReference sceneReference1, in SceneReference sceneReference2)
        {
            if(sceneReference1 == null && sceneReference2 == null)
            {
                return true;
            }
            
            return sceneReference1?.Equals(sceneReference2) ?? false;   
        }
        public static bool operator !=(in SceneReference sceneReference1, in SceneReference sceneReference2)
            => !sceneReference1?.Equals(sceneReference2) ?? true;
        
        public static bool operator ==(in string name, in SceneReference sceneReference)
            => sceneReference?.Equals(name) ?? false;
        public static bool operator !=(in string name, in SceneReference sceneReference)
            => !sceneReference?.Equals(name) ?? true;

        public override bool Equals(object obj) => base.Equals(obj);

        public bool Equals(in Scene scene)
            => this.GetSceneName.Equals(scene.name);

        public bool Equals(in SceneReference sceneReference)
            => sceneReference != null && this.GetSceneName.Equals(sceneReference.GetSceneName);

        public bool Equals(in string name)
            => !name.IsNullOrWhitespace() && GetSceneName.Equals(name);

        public string GetSceneName
        {
            get
            {
                #if UNITY_EDITOR
                
                return (sceneAsset == null)
                    ? "NULL"
                    : sceneAsset.name;
                
                #else
                
                return System.IO.Path.GetFileNameWithoutExtension(scenePath);
                
                #endif
            }
        }
        
        #endregion

        #endregion

        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
            HandleBeforeSerialize();
            #endif
        }
        public void OnAfterDeserialize()
        {
            #if UNITY_EDITOR
            // We can't access AssetDataBase during serialization, so call event to handle later.
            EditorApplication.update += HandleAfterDeserialize;
            #endif
        }

        #if UNITY_EDITOR
        private void HandleBeforeSerialize()
        {
            //Asset is invalid but has a Path to try and recover from.
            if (IsValidSceneAsset == false && string.IsNullOrEmpty(scenePath) == false)
            {
                sceneAsset = GetSceneAssetFromPath;

                //No asset found, path was invalid. Make sure we don't carry over the invalid path.
                if (sceneAsset == null){ scenePath = string.Empty; }

                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
            else
            {
                scenePath = GetScenePathFromAsset;
            }
        }
        private void HandleAfterDeserialize()
        {
            EditorApplication.update -= HandleAfterDeserialize;
            
            if (IsValidSceneAsset) return; 
                
            //Asset is invalid but has a Path to try and recover from.
            if (string.IsNullOrEmpty(value: scenePath)) return;

            sceneAsset = GetSceneAssetFromPath;
            
            //No asset found, path was invalid. Make sure we don't carry over the invalid path.
            if(sceneAsset == null)
            {
                scenePath = string.Empty;
            }

            if(Application.isPlaying == false)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
        #endif
    }
}