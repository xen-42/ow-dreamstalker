using UnityEngine;

namespace Dreamstalker.Utility;

internal static class DSMath
{
	public static bool VectorApproxEquals(Vector3 a, Vector3 b, float epsilon = 0.001f) =>
		OWMath.ApproxEquals(a.x, b.x, epsilon) && OWMath.ApproxEquals(a.y, b.y, epsilon) && OWMath.ApproxEquals(a.z, b.z, epsilon);
}
