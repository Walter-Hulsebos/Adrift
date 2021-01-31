using System;
using CGTK.Utilities.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
	namespace Player.Targeting
	{
		[ExecuteAlways]
		public sealed partial class TargetingScreen : Singleton<TargetingScreen>
		{
			#region Fields

			[SerializeField] private float radius = 1;

			private Plane _selectionPlane = new Plane(inNormal: Vector3.up, inPoint: Vector3.zero);

			private Vector3 _center;

			#endregion

			#region Methods

			private void Reset() => UpdatePlane();

			private void Awake() => UpdatePlane();

			private void OnValidate() => UpdatePlane();

			private void UpdatePlane()
			{
				Transform __transform = transform;
				Vector3 __position = __transform.position;
				_selectionPlane = new Plane(inNormal: __transform.up, inPoint: __position);
				_center = __position;
			}
			

			private Vector3 MouseRelativeOnPlane
			{
				get
				{
					//Ray __ray = SelectionManager.MouseRay;

					Ray __ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					
					Debug.DrawRay(__ray.origin, __ray.direction, Color.red);

					return _selectionPlane.Raycast(ray: __ray, enter: out float __distance)
						? __ray.GetPoint(__distance)
						: Vector3.zero;
				}
			}

			public Vector3 localPositionTargetingScreen;
			
			/// <summary>
			/// Returns true if in range, false if not.
			/// </summary>
			/// <param name="mouseLocalPosition"></param>
			/// <returns> True when in targeting screen, false when out. </returns>
			private bool MouseLocalPositionTargetingScreen (out Vector3 mouseLocalPosition)
			{
				Vector3 __mouseOnPlane = MouseRelativeOnPlane;
				
				mouseLocalPosition = transform.InverseTransformPoint(__mouseOnPlane);

				localPositionTargetingScreen = new Vector3(mouseLocalPosition.x, 0, mouseLocalPosition.z);

				return (Vector3.Distance(a: Vector3.zero, b: mouseLocalPosition) <= radius);
			}

			/// <summary>
			/// 2D range between x [0 - 1], y [0 - 1]
			/// </summary>
			/// <returns> True when in targeting screen, false when out. </returns>
			private bool MousePercentages(out Vector2 mousePercentages)
			{
				bool __isMouseOnTargetingScreen = MouseLocalPositionTargetingScreen(out Vector3 __mouseLocalPosition);
				
				float __diameter = (radius / 2);

				mousePercentages.x = (__mouseLocalPosition.x / __diameter).Clamp01();
				mousePercentages.y = (__mouseLocalPosition.z / __diameter).Clamp01();

				CurrentPercentages = mousePercentages;

				return __isMouseOnTargetingScreen;
			}

			public Vector2 CurrentPercentages { get; private set; }
			
			private bool MouseInOtherScene(in Camera otherSceneCamera, out Vector3 worldPositionOtherScene)
			{
				bool __isMouseOnTargetingScreen = MousePercentages(out Vector2 __mousePercentages);

				worldPositionOtherScene = otherSceneCamera.ViewportToWorldPoint(position: 
					new Vector3(
						__mousePercentages.x, 
						__mousePercentages.y, 
						otherSceneCamera.nearClipPlane));

				return __isMouseOnTargetingScreen;
			}

			public IActor GetClosestActorToMouse(in Camera otherSceneCamera, in IActor[] actors)
			{
				bool __isMouseOnTargetingScreen = MouseInOtherScene(otherSceneCamera, out Vector3 __worldPositionOtherScene);

				return __isMouseOnTargetingScreen 
					? GetClosestActor(__worldPositionOtherScene, actors) 
					: null;
			}

			private static IActor GetClosestActor(in Vector2 referencePosition, in IActor[] actors)
			{
				float  __closestDistance = Mathf.Infinity;
				IActor __closestActor = null;

				foreach (IActor __actor in actors)
				{
					float __distance = Vector2.Distance(__actor.ActorPosition, referencePosition);
					
					if (__distance >= __closestDistance) continue;
					
					__closestDistance = __distance;
					__closestActor = __actor;
				}
				
				return __closestActor;
			}


			[SerializeField] private GuidReference otherSceneCamera;

			[SerializeField] private Transform otherSceneDebugObject;
			
			private void Update()
			{
				UpdatePlane();
				
				if (otherSceneCamera == null) return;
				
				if (MouseInOtherScene(otherSceneCamera.gameObject.GetComponent<Camera>(),
					out Vector3 __worldPositionOtherScene))
				{
					//otherSceneDebugObject.position = __worldPositionOtherScene;
				}

			}

			#endregion
		}
	}
}