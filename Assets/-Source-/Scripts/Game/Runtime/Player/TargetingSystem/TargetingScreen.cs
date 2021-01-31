using UnityEngine;

namespace Game
{
	using Selection;
	
	namespace Player.Targeting
	{

		public sealed partial class TargetingScreen : MonoBehaviour
		{
			#region Fields

			[SerializeField] private float radius = 1;

			private Plane _selectionPlane = new Plane(inNormal: Vector3.up, inPoint: Vector3.zero);

			private Vector3 _center;

			#endregion

			#region Methods

			private void Reset()
			{
				Transform __transform = transform;
				_selectionPlane = new Plane(inNormal: __transform.up, inPoint: __transform.position);
				_center = __transform.position;
			}

			private void Awake()
			{
				Transform __transform = transform;
				_selectionPlane = new Plane(inNormal: __transform.up, inPoint: __transform.position);
				_center = __transform.position;
			}

			private Vector3 MouseRelativeOnPlane
			{
				get
				{
					Ray __ray = SelectionManager.MouseRay;
					
					Debug.DrawRay(__ray.origin, __ray.direction, Color.red);

					return _selectionPlane.Raycast(ray: __ray, enter: out float __distance)
						? __ray.GetPoint(__distance)
						: Vector3.zero;
				}
			}

			/// <summary>
			/// Returns true if in range, false if not.
			/// </summary>
			/// <param name="mouseLocalPosition"></param>
			/// <returns></returns>
			private bool MouseInTargetingScreen(out Vector3 mouseLocalPosition)
			{
				Vector3 __mouseOnPlane = MouseRelativeOnPlane;
				
				mouseLocalPosition = transform.InverseTransformPoint(__mouseOnPlane);

				return (Vector3.Distance(a: Vector3.zero, b: mouseLocalPosition) <= radius);
			}

			#endregion
		}
	}
}