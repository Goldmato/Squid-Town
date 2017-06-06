using UnityEngine;

public class DestroySelf : MonoBehaviour {

	public float Timer = 0.51f;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, Timer);
	}
}
