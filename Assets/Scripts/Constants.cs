using UnityEngine;
using System.Collections;

public static class Constants
{
	public static float		TimerWin = 30f;
	public static int		WinGameBlue = 0;
	public static int		WinGameRed = 0;
	public static Color		ColorBlue = new Color(22f / 255f, 132f / 255f, 216f / 255f);
	public static Color		ColorRed = new Color(251f / 255f, 39f / 255f, 46f / 255f);

	private static float	dist = 380f;
	private static float	height = -340f + 130f;

	public static Vector3[]	BottomPosition = new [] { new Vector3(0f, height, dist), new Vector3(dist, height, 0f), new Vector3(0f, height, -dist), new Vector3(-dist, height, 0f) };
	public static Vector3[]	CornerPosition = new [] { new Vector3(dist, height, dist), new Vector3(dist, height, -dist), new Vector3(-dist, height, -dist), new Vector3(-dist, height, dist) };


	public static float		CameraOrthographicSize = 370f;
	public static float		CameraOrthographicSizeStart = CameraOrthographicSize * 2.5f;
	public static float		DistCollision = 100f;
	public static float		MinOpacity = 0.4f;
}