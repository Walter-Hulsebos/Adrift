#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;
using UnityEditorInternal;

using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

using static MultiScene;

[CustomEditor(typeof(MultiScene))]
public class MultiSceneEditor : OdinEditor
{
	[MenuItem(itemName: "Assets/Create/Multi-Scene", false, 201)]
	private static void CreateMultiScene()
	{
		MultiScene __multi = CreateInstance<MultiScene>();
		__multi.name = "New Multi-Scene";

		UnityEngine.Object __parent = Selection.activeObject;

		string __directory = "Assets";
		if(__parent != null)
		{
			__directory = AssetDatabase.GetAssetPath(__parent.GetInstanceID());
			if(!Directory.Exists(__directory))
			{
				__directory = Path.GetDirectoryName(__directory);
			}
		}

		ProjectWindowUtil.CreateAsset(__multi, $"{__directory}/{__multi.name}.asset");
	}

	[MenuItem(itemName: "Edit/Multi-Scene From Open Scenes %#&s", false, 0)]
	private static void CreateMultiSceneFromOpenScenes()
	{
		MultiScene __newMultiScene = CreateInstance<MultiScene>();
		__newMultiScene.name = "New Multi-Scene";

		Scene __activeScene = SceneManager.GetActiveScene();
		int __sceneCount = SceneManager.sceneCount;

		for(int __i = 0; __i < __sceneCount; __i++)
		{
			Scene __scene = SceneManager.GetSceneAt(__i);

			SceneAsset __sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(__scene.path);
			
			if(__activeScene == __scene)
			{
				__newMultiScene.activeScene = __sceneAsset;
			}

			__newMultiScene.sceneAssets.Add(new SceneInfo(sceneReference: __sceneAsset, loadScene: __scene.isLoaded));
		}

		string __directory = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
		bool __isDirectory = Directory.Exists(__directory);
		
		if(!__isDirectory)
		{
			__directory = Path.GetDirectoryName(__directory);
		}

		ProjectWindowUtil.CreateAsset(asset: __newMultiScene, pathName: $"{__directory}/{__newMultiScene.name}.asset");
	}

	[OnOpenAsset(1)]
	private static bool OnOpenAsset(int id, int line)
	{
		UnityEngine.Object __obj = EditorUtility.InstanceIDToObject(instanceID: id);
		
		if(__obj is MultiScene __scene)
		{
			OpenMultiScene(__scene, Event.current.alt);
			return true;
		}

		if(!(__obj is SceneAsset)) return false;
		
		if(!Event.current.alt) return false;
		
		EditorSceneManager.OpenScene(scenePath: AssetDatabase.GetAssetPath(instanceID: __obj.GetInstanceID()), mode: OpenSceneMode.Additive);
		
		return true;
	}

	private static void OpenMultiScene(MultiScene obj, bool additive)
	{
		Scene __activeScene = default;
		
		if(additive || EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
		{
			List<string> __firstUnloadedScenes = new List<string>();
			bool __inFirstUnloadedScenes = true;
			Scene __firstLoadedScene = default;
			
			foreach(MultiScene.SceneInfo __info in obj.sceneAssets)
			{
				if(__info.sceneReference == null) { continue; }
				
				string __path = AssetDatabase.GetAssetPath(((SceneAsset)__info.sceneReference).GetInstanceID());
				OpenSceneMode __mode = OpenSceneMode.Single;
				bool __isActiveScene =(SceneAsset)__info.sceneReference == obj.activeScene;

				bool __exitedFirstUnloadedScenes = false;
				if(__inFirstUnloadedScenes)
				{
					if(!__isActiveScene && !__info.loadScene)
					{
						__firstUnloadedScenes.Add(__path);
						continue;
					}

					__inFirstUnloadedScenes = false;
					__exitedFirstUnloadedScenes = true;
				}

				if((!__exitedFirstUnloadedScenes) || additive)
				{
					__mode = ((!additive && __isActiveScene) || __info.loadScene)
						? OpenSceneMode.Additive
						: OpenSceneMode.AdditiveWithoutLoading; 
				}

				Scene __scene = EditorSceneManager.OpenScene(__path, __mode);

				if(__isActiveScene) __activeScene = __scene;
				if(__exitedFirstUnloadedScenes) __firstLoadedScene = __scene;
			}

			foreach(string __path in __firstUnloadedScenes)
			{
				Scene __scene = EditorSceneManager.OpenScene(__path, OpenSceneMode.AdditiveWithoutLoading);
				
				if(__firstLoadedScene.IsValid())
				{
					EditorSceneManager.MoveSceneBefore(__scene, __firstLoadedScene);
				}
			}
			
		}
		if(!additive && __activeScene.IsValid())
		{
			SceneManager.SetActiveScene(__activeScene);
		}
	}

	private static Scene SceneAssetToScene(UnityEngine.Object asset)
	{
		return SceneManager.GetSceneByPath(AssetDatabase.GetAssetPath(asset));
	}
	
	private MultiScene _targetMultiScene;

	protected override void OnEnable()
	{
		base.OnEnable();
		
		_targetMultiScene = (MultiScene)base.target;
	}
}

#endif