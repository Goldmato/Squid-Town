using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAndCloseDoor : MonoBehaviour
{

	public GameObject door;

	void OpenDoor ()
	{
		door.SetActive (false);
	}

	void CloseDoor ()
	{
		door.SetActive (true);
	}
}
