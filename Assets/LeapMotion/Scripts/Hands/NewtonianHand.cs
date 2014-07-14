/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// A physics hand model that will apply force back on the _attachment_.
public class NewtonianHand : SkeletalHand {

  public Rigidbody attachment;
  public float filtering = 0.5f;

  void Start() {
    palm.rigidbody.maxAngularVelocity = Mathf.Infinity;
    IgnoreCollisionsWithSelf();
  }

  protected void SetAnchor() {
    if (palm != null) {
      Vector3 palm_center = GetPalmCenter();
      ConfigurableJoint palm_joint = palm.GetComponent<ConfigurableJoint>();

      palm_joint.connectedBody = attachment;
      Vector3 target_anchor = attachment.transform.InverseTransformPoint(palm_center);
      Vector3 delta_anchor = target_anchor - palm_joint.connectedAnchor;
      palm_joint.connectedAnchor += (1 - filtering) * delta_anchor;
    }
  }

  public override void InitHand() {
    if (palm != null) {
      SetAnchor();
      palm.transform.position = GetPalmCenter();
      palm.transform.rotation = GetPalmRotation();
    }

    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null)
        fingers[i].InitFinger();
    }
  }

  public override void UpdateHand() {
    if (palm != null) {
      SetAnchor();

      // Set palm angular velocity.
      Quaternion delta_rotation = GetPalmRotation() *
                                  Quaternion.Inverse(palm.transform.rotation);
      float angle = 0.0f;
      Vector3 axis = Vector3.zero;
      delta_rotation.ToAngleAxis(out angle, out axis);

      if (angle >= 180) {
        angle = 360 - angle;
        axis = -axis;
      }
      if (angle != 0)
        palm.rigidbody.angularVelocity = (1 - filtering) * angle * axis;
    }

    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null)
        fingers[i].UpdateFinger();
    }
  }
}
