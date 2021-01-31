using UnityEngine;

namespace Utilities.MultiScenes
{
    public sealed class PreloadSceneLoader : MonoBehaviour
    {
        #if ODIN_INSPECTOR && UNITY_EDITOR
        [Sirenix.OdinInspector.RequiredAttribute]
        [Sirenix.OdinInspector.InlineEditorAttribute]
        #endif
        [SerializeField] private MultiScene multiScene = null;
    
        private void Start()
        {
            multiScene.Load();
        
            //MultiSceneLoader.LoadMultiScene(multiScene: MultiScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }
}
