/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// A physics finger model that will apply force back on the _attachment_.
public class NewtonianFinger : SkeletalFinger {

  public Rigidbody attachment;
  public float filtering = 0.5f;

  void Start() {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null)
        bones[i].rigidbody.maxAngularVelocity = Mathf.Infinity;
    }
  }
  
  protected void SetAnchor(int i) {
    Vector3 bone_center = GetBoneCenter(i);
    ConfigurableJoint bone_joint = bones[i].GetComponent<ConfigurableJoint>();

    bone_joint.connectedBody = attachment;
    Vector3 target_anchor = attachment.transform.InverseTransformPoint(bone_center);
    Vector3 delta_anchor = target_anchor - bone_joint.connectedAnchor;
    bone_joint.connectedAnchor += (1 - filtering) * delta_anchor;
  }

  public override void InitFinger() {
    base.InitFinger();

    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null)
        SetAnchor(i);
    }
  }

  public override void UpdateFinger() {
    for (int i = 0; i < bones.Length; ++i) {
      if (bones[i] != null) {
        SetAnchor(i);

        // Set bone angular velocity.
        Quaternion target_rotation = GetBoneRotation(i);
        Quaternion delta_rotation = target_rotation *
                                    Quaternion.Inverse(bones[i].transform.rotation);
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;
        delta_rotation.ToAngleAxis(out angle, out axis);

        if (angle >= 180) {
          angle = 360 - angle;
          axis  = -axis;
        }

        if (angle != 0)
          bones[i].rigidbody.angularVelocity = (1 - filtering) * angle * axis;
      }
    }
  }
}
