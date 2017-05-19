using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	// TODO: Implement features shown in https://www.draw.io/#DEnemyManager

	public static Vector3 RequestHidingSpot()
	{
		if(Registry.Current.Doors != null)
		{
			return Registry.Current.Doors[Random.Range(0, Registry.Current.Doors.Count)].transform.position;
		}
		else
		{
			return Vector3.zero;
		}
	}
}
