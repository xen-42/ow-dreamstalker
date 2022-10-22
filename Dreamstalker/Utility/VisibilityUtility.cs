using UnityEngine;

namespace Dreamstalker.Utility;

internal static class VisibilityUtility
{
	public static VisibilityTracker AddVisibilityTracker(Transform transform, float size)
	{
		var visibilityTracker = new GameObject("VisibilityTracker_Sphere");
		visibilityTracker.transform.parent = transform;
		visibilityTracker.transform.localPosition = Vector3.up * 0.5f;

		var sphere = visibilityTracker.AddComponent<SphereShape>();
		sphere.radius = size;

		var tracker = visibilityTracker.AddComponent<ShapeVisibilityTracker>();

		return tracker;
	}
}
