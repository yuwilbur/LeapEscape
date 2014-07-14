using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

  private float default_speed_;

	// Use this for initialization
	void Start () {
    default_speed_ = 20.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
  
    if (Input.GetKey(KeyCode.W)) {
      MoveForward(default_speed_);
    }
    if (Input.GetKey(KeyCode.S)) {
      MoveBackward(default_speed_);
    }
    if (Input.GetKey(KeyCode.D)) {
      TurnRight(default_speed_ / 6);
    }
    if (Input.GetKey(KeyCode.A)) {
      TurnLeft(default_speed_ / 6);
    }
    if (Input.GetKey(KeyCode.Q)) {
      StrifeLeft(default_speed_ / 2);
    }
    if (Input.GetKey(KeyCode.E)) {
      StrifeRight(default_speed_ / 2);
    }
	}
  
  public Vector3 ForwardVector() {
    return transform.forward;
  }
  
  public void MoveForward(float speed) {
    rigidbody.velocity = transform.forward * speed;
  }
  
  public void MoveBackward(float speed) {
    rigidbody.velocity = -transform.forward * speed;
  }
  
  public void TurnRight(float speed) {
    rigidbody.angularVelocity = new Vector3(0, speed, 0);
  }
  
  public void TurnLeft(float speed) {
    rigidbody.angularVelocity = new Vector3(0, -speed, 0);
  }
  
  public void StrifeRight(float speed) {
    rigidbody.velocity = Vector3.Cross(new Vector3(0,1,0), transform.forward) * speed;
  }
  
  public void StrifeLeft(float speed) {
    rigidbody.velocity = -Vector3.Cross(new Vector3(0,1,0), transform.forward) * speed;
  }
}