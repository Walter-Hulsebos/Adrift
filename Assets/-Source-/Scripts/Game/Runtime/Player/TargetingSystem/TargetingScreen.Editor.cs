using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Player.Targeting
{
	public sealed partial class TargetingScreen
	{
		#if UNITY_EDITOR
		
		[CustomEditor(typeof(TargetingScreen))]
		private sealed class TargetingScreenEditor : Editor
		{
			private TargetingScreen _targetingScreen;
			
			private Transform _transform;

			private void OnSceneGUI()
			{
				_targetingScreen = (TargetingScreen) target;
				_transform = _targetingScreen.transform;
				
				DebugTargetingArea();
				DebugMouseLocalPosition();
				DebugMouseOnPlane();
			}

			private void DebugTargetingArea()
			{
				Vector3 __center = _transform.position;
				Vector3 __normal = _targetingScreen._selectionPlane.normal;
				float   __radius = _targetingScreen.radius;

				Handles.color = new Color(.5f, .5f, 0, .5f);
				Handles.DrawSolidDisc(center: __center, normal: __normal, radius: __radius);
			}

			private void DebugMouseOnPlane()
			{
				if (!Application.isPlaying) return;

				Handles.matrix = _transform.worldToLocalMatrix;
				
				Handles.color = Color.green;

				Vector3 __mousePlanePosition = _targetingScreen.MouseRelativeOnPlane;
				
				Handles.SphereHandleCap(
					0,
					__mousePlanePosition,
					Quaternion.identity, 
					size: 0.25f,
					EventType.Repaint);
			}
			
			private void DebugMouseLocalPosition()
			{
				if (!Application.isPlaying) return;
				
				Handles.matrix = _transform.localToWorldMatrix;
					
				Handles.color = _targetingScreen.MouseLocalPositionTargetingScreen(mouseLocalPosition: out Vector3 __mouseLocalPosition) 
					? Color.red 
					: Color.cyan;
				
				Handles.SphereHandleCap(
					0,
					__mouseLocalPosition,
					Quaternion.identity, 
					size: 0.25f,
					EventType.Repaint);
			}
		}
		
		#endif
	}
}