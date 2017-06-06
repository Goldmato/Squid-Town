using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasso : MonoBehaviour {
	public float ThrowSpeed = 1000f;
	public GameObject Rope;
	public GameObject FPSController;
	public Transform CarryPosition;
	public GameObject FirstPersonCharacter;
	public float MaxDistanceX = 10f;
	public float MaxDistanceZ = 10f;
	private bool thrown = false;

	// Use this for initialization
	void Start () {
		Carry ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)){
			if (!thrown /* make sure it doesn't get thrown out further and further */) {
				Throw ();
			} else if (thrown) {
				Carry ();
			}
		}

		// make sure the lasso isn't too far away from the player
		if (thrown) {
			// check the x distance
			float xDistance = gameObject.GetComponent<Transform> ().position.x - FPSController.GetComponent<Transform> ().position.x;
			if (xDistance > MaxDistanceX) {
				gameObject.GetComponent<Transform> ().position = new Vector3(FPSController.GetComponent<Transform> ().position.x + MaxDistanceX, gameObject.GetComponent<Transform> ().position.y, gameObject.GetComponent<Transform> ().position.z);
				// set the velocity to zero
				gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			} else if (xDistance < -MaxDistanceX) {
				gameObject.GetComponent<Transform> ().position = new Vector3(FPSController.GetComponent<Transform> ().position.x - MaxDistanceX, gameObject.GetComponent<Transform> ().position.y, gameObject.GetComponent<Transform> ().position.z);
				// set the velocity to zero
				gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}

			// check the z distance
			float zDistance = gameObject.GetComponent<Transform> ().position.z - FPSController.GetComponent<Transform> ().position.z;
			if (zDistance > MaxDistanceZ) {
				gameObject.GetComponent<Transform> ().position = new Vector3(gameObject.GetComponent<Transform> ().position.x, gameObject.GetComponent<Transform> ().position.y, FPSController.GetComponent<Transform> ().position.z + MaxDistanceZ);
				// set the velocity to zero
				gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			} else if (zDistance < -MaxDistanceZ) {
				gameObject.GetComponent<Transform> ().position = new Vector3(gameObject.GetComponent<Transform> ().position.x, gameObject.GetComponent<Transform> ().position.y, FPSController.GetComponent<Transform> ().position.z - MaxDistanceZ);
				// set the velocity to zero
				gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		}
	}

	public void Carry ()
	{
		// disable the rope
		Rope.SetActive (false);

		// turn off gravity
		gameObject.GetComponent<Rigidbody> ().useGravity = false;

		// attach to player
		gameObject.transform.SetParent (FirstPersonCharacter.GetComponent<Transform> ());

		// set the velocity to zero
		gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);

		// reset the rotation
		gameObject.GetComponent<Rigidbody> ().rotation = FPSController.GetComponent<Transform> ().rotation;

		// zero out the angular velocity
		gameObject.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0, 0, 0);

		// set the position
		gameObject.transform.position = CarryPosition.position;

		thrown = false;
	}

	void Throw () {
		// turn on gravity
		gameObject.GetComponent<Rigidbody> ().useGravity = true;

		// enable the rope
		Rope.SetActive (true);

		// move the lasso
		gameObject.GetComponent <Rigidbody> ().AddRelativeForce (new Vector3(0f, 0f, ThrowSpeed));
//		gameObject.GetComponent<Rigidbody> ().velocity = gameObject.GetComponent<Rigidbody> ().velocity + transform.forward * throwSpeed;

		// detach from player
		gameObject.transform.SetParent (null);

		thrown = true;
	}
}
