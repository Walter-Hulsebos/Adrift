//Inspired by Nate Tessman's Multi-Scene

using System;
using System.Collections;
using System.Collections.Generic;
using CommonGames.Utilities.CGTK;
using UnityEngine;
using UnityEngine.SceneManagement;
    
#if ODIN_INSPECTOR

using Sirenix.OdinInspector;
    
using ScriptableObject = Sirenix.OdinInspector.SerializedScriptableObject;
    
#endif

//using CommonGames.Utilities.CGTK;
//using CommonGames.Utilities.Extensions;

//TODO: -Walter- Ensure that there's only ONCE entry in the list of per scene. (No duplicates).
public class MultiScene : ScriptableObject
{
	#region Custom Types

	[Serializable]
	public class SceneInfo
	{
		[HideLabel]
		[PropertySpace(spaceBefore: 13, spaceAfter: 13f)]
		[HorizontalGroup(@group: "Split", 8f, LabelWidth = 8f)]
		[BoxGroup(@group: "Split/Left", showLabel: false)]
		public bool active = false;
		
		[HorizontalGroup(@group: "Split", 0.2f, LabelWidth = 110)]
		[BoxGroup(@group: "Split/Mid", showLabel: false)]
		[PropertySpace(spaceBefore: 0, spaceAfter: 26f)]
		//[ValueDropdown(nameof(LoadOptions))]
		public bool loadScene = true;
		
		[BoxGroup(@group: "Split/Right", showLabel: false)]
		public SceneReference sceneReference = null;

		public SceneInfo(SceneReference sceneReference, bool active = default, bool loadScene = default)
		{
			this.sceneReference = sceneReference;
			this.active = active;
			this.loadScene = loadScene;
		}

		//public static implicit operator SceneReference(SceneInfo sceneInfo) => sceneInfo.asset;
		
		public static implicit operator SceneReference(SceneInfo sceneInfo)
			=> sceneInfo.sceneReference;
	}

	private static IEnumerable LoadOptions = new ValueDropdownList<bool>()
	{
		{ "Loaded", true },
		{ "Not Loaded", false },
		{ "Load", false },
	};
	
	#endregion

	#region Variables

	[HideInInspector]
	public SceneReference activeScene;
	[HideInInspector]
	public int activeSceneIndex = -1;
	
	//public SceneInfo[] sceneAssets;
	[ListDrawerSettings(CustomAddFunction = nameof(CustomAddFunction))]
	public List<SceneInfo> sceneAssets = new List<SceneInfo>();

	#endregion

	#region Methods

	private SceneInfo CustomAddFunction()
		=> new SceneInfo(sceneReference: null, active: false, loadScene: true);

	#if UNITY_EDITOR
	
	private List<SceneInfo> _sceneAssetsLastFrame = new List<SceneInfo>();
	
	#endif

	private void OnValidate()
	{
		bool __foundNewActiveScene = false;
		
		for(int __index = 0; __index < sceneAssets.Count; __index++)
		{
			SceneInfo __scene = sceneAssets[__index];

			#region Active Scene Check

			if(__foundNewActiveScene) continue;
			if(!__scene.active || __index == activeSceneIndex) continue;

			activeSceneIndex = __index;
			activeScene = __scene;
            
			__DeactivateOtherScenes();
			__foundNewActiveScene = true;
			//return;

			#endregion
		}

		void __DeactivateOtherScenes()
		{
			for(int __index = 0; __index < sceneAssets.Count; __index++)
			{
				SceneInfo __scene = sceneAssets[__index];

				if(__index != activeSceneIndex)
				{
					__scene.active = false;
				}
			}
		}
	}

	public void Load()
	{
		//sceneAssets.For(sceneInfo => SceneManager.LoadScene(sceneInfo.asset, LoadSceneMode.Additive));

		for (int __index = 0; __index < sceneAssets.Count; __index++)
		{
			SceneInfo sceneInfo = sceneAssets[__index];
			
			SceneManager.LoadSceneAsync(sceneInfo.sceneReference, mode: (__index == 0) ? LoadSceneMode.Single : LoadSceneMode.Additive);
		}
	}
	
	#endregion
}