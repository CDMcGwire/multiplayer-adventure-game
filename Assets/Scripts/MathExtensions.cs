using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Ext {
	public static Vector3 RoundVector (this Vector3 v) {
		return new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
	}

	/// <summary>
	/// Turns a Unity Vector3 into a 3 dimensional int vector struct. Swaps the values of the y and z axes.
	/// </summary>
	/// <param name="v"></param>
	/// <returns>IntVec3</returns>
	public static IntVec3 ToInt3(this Vector3 v) {
		return new IntVec3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z), Mathf.RoundToInt(v.y));
	}

	/// <summary>
	/// Turns a Unity Vector3 into a 3 dimensional int vector struct. Swaps the values of the y and z axes. Allows redefining of z value.
	/// </summary>
	/// <param name="v"></param>
	/// <returns>IntVec3</returns>
	public static IntVec3 ToInt3(this Vector3 v, int z) {
		return new IntVec3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z), z);
	}

	/// <summary>
	/// Turns a Unity Vector3 into a 2 dimesnional int vector struct. Z axis of the Vector3 becomes the IntVec2's Y axis.
	/// </summary>
	/// <param name="v"></param>
	/// <returns></returns>
	public static IntVec2 ToInt2(this Vector3 v) {
		return new IntVec2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.z));
	}
}

public struct IntVec3 {
	public int x;
	public int y;
	public int z;

	public IntVec3(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public int DistanceTo(IntVec3 from, IntVec3 to) {
		var xDist = Mathf.Abs(to.x - from.x);
		var yDist = Mathf.Abs(to.y - from.y);
		var zDist = Mathf.Abs(to.z - from.z);
		return xDist + yDist + zDist;
	}
}

public struct IntVec2 {
	public int x;
	public int y;

	public IntVec2(int x, int y) {
		this.x = x;
		this.y = y;
	}
}

//////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////////////////

public static class MathExt {
	public static float ToNearestMutliple(float value, float multiple) {
		return Mathf.Round(value / multiple) * multiple;
	}
}
