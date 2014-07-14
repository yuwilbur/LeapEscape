using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Interact;

public class HoldingCallbacks : MonoBehaviour {

  private GameObject lastHovered = null;

  void Start () {
    UnityUtil.Scene.OnHoldingHoverOver += new Scene.HoldingNotification(OnHoldingHovers);
    UnityUtil.Scene.OnHoldingStarts += new Scene.HoldingNotification(OnHoldingStarts);
    UnityUtil.Scene.OnHoldingUpdates += new Scene.HoldingNotification(OnHoldingUpdates);
    UnityUtil.Scene.OnHoldingEnds += new Scene.HoldingNotification(OnHoldingEnds);
  }
  
  void Update () {
  }

  public void OnHoldingHovers(Holding holding) {
    Body body = holding.Body;
    GameObject gameObject = null;
    if (body != null && body.IsValid())
      gameObject = UnityUtil.BodyMapper.FirstOrDefault(x => x.Value.BodyId.ptr == body.BodyId.ptr).Key;
    if (lastHovered != gameObject)
    {
      if (lastHovered) {
        Material material = Resources.Load("LeapInteract/Materials/Free") as Material;
        lastHovered.renderer.material = material;
      }
      if (gameObject) {
        Material material = Resources.Load("LeapInteract/Materials/Hover") as Material;
        gameObject.renderer.material = material;
      }
      lastHovered = gameObject;
    }
  }

  public void OnHoldingStarts(Holding holding) {
    Body body = holding.Body;
    GameObject gameObject = null;
    if (body != null && body.IsValid())
      gameObject = UnityUtil.BodyMapper.FirstOrDefault(x => x.Value.BodyId.ptr == body.BodyId.ptr).Key;
    //Debug.Log("holding started " + gameObject);
    if (gameObject) {
      Material material = Resources.Load("LeapInteract/Materials/Held") as Material;
      gameObject.renderer.material = material;
    }
  }

  public void OnHoldingUpdates(Holding holding) {
  }

  public void OnHoldingEnds(Holding holding) {
    Body body = holding.Body;
    GameObject gameObject = null;
    if (body != null && body.IsValid())
      gameObject = UnityUtil.BodyMapper.FirstOrDefault(x => x.Value.BodyId.ptr == body.BodyId.ptr).Key;
    if (gameObject) {
      Material material = Resources.Load("LeapInteract/Materials/Free") as Material;
      gameObject.renderer.material = material;
    }
  }
}
