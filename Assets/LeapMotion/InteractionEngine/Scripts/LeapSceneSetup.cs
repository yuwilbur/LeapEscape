using UnityEngine;
using System.Collections;
using Leap.Interact;


/// <summary>
/// Configuration & initialization scripts to setup Leap Interaction & reflect the Unity scene in it.
/// </summary>
public class LeapSceneSetup : MonoBehaviour {

  /// <summary>
  /// Defines the relative scale of the Unity and Leap worlds. Leap unity is equivalent to one millimeter. 
  /// This variable specifiers the length of the Unity unit in millimeters.
  /// </summary>
  public float clientUnitLengthInMillimeters = 50f;

  /// <summary>
  /// Specifies where the Leap device is located in relation to the reference object, or other object used as a reference for
  /// positioning the Leap world. This is specifierd in Leap unity (e.g. in real-world millimeters).
  /// By default, set this to [0,0,0] when using HandController as the reference object. Set it to [0, -200, 200] when using the camera.
  /// </summary>
  public Vector3 OffsetCameraToLeap = new Vector3(0f, -200f, 200f);

  /// <summary>
  /// The refernce object that the leap device is 'attached' to. As this object moves & reorients, the Leap Interaction hands will move along.
  /// </summary>
  public Transform ReferenceObject;

  /// <summary>
  /// Shows the exact hand-proxy shapes used by Leap Interaction. You can also use those shapes to interact/collide/push regular rigidbodies in the scene.
  /// </summary>
  public bool ShowInteractionHands = false;

  [HideInInspector]
  public bool ShowVisualizer = false; // disabled flag
  public bool EnableInteractionCollisions = false;

  public bool AllowPinchWithAnyFinger = false;
  public bool DisableHoldingOnPointingIndexFinger = false;

  static public int LayerForHands = 30;
  static public int LayerForHeldObjects = 31;
  static public int LayerForReleasedObjects = 0; // Default

  private bool initialized = false;

  public static void EnsuerInstanceInitialized() {
    // find instance.
    LeapSceneSetup leap = GameObject.FindObjectOfType<LeapSceneSetup>() as LeapSceneSetup;
    leap.EnsureInitialized();
  }

  public void EnsureInitialized()
  {
    if (!initialized) {
      Initialize();
      initialized = true;
    }
  }

  void Awake() {
    EnsureInitialized();
  }

  /// <summary>
  /// Performs initialization of Leap Interaction.
  /// </summary>
  public void Initialize () {

    m_scene = new Scene ();
    Scene.ClientUnitLengthInMillimeters = clientUnitLengthInMillimeters;
    PositionCamera();

    m_scene.RunCollisionDetection = EnableInteractionCollisions;
    m_scene.ResolveCollisions = true; //EnableInteractionCollisions;
    m_scene.AlwaysRunCollisionForHandsVsHeldObjects = true;
    
    m_scene.HandDistanceMultiplier = 1.0f;
    m_scene.DestroyClustersWhileNotHolding = true;
    
    // Don't create update the hand from the interact dll
    m_scene.UseBodyCallbacks = ShowInteractionHands;
    m_scene.UseHoldingCallbacks = true;
    
    // Start internal visualizer
    if (ShowVisualizer) { m_scene.OpenVisualDebgger (); }

    m_scene.AllowPinchWithAnyFinger = AllowPinchWithAnyFinger;
    m_scene.DisableHoldingOnPointingIndexFinger = DisableHoldingOnPointingIndexFinger;

    m_unityUtil = new UnityUtil (m_scene);
    m_unityUtil.InitLeap ();

    UnityUtil.LayerForHands = LayerForHands;
    UnityUtil.LayerForHeldObjects = LayerForHeldObjects;
    UnityUtil.LayerForReleasedObjects = LayerForReleasedObjects;

    // Optional: Initializatio of holding callbacks.
    //m_scene.OnHoldingHoverOver += OnHoldingHovers;
    //m_scene.OnHoldingStarts += OnHoldingStarts;
    //m_scene.OnHoldingUpdates += OnHoldingUpdates;
    //m_scene.OnHoldingEnds += OnHoldingEnds;
	}
	
	/// <summary>
  /// Updates Leap device position based on the ReferenceObject. Updates Leap Intearction.
  /// </summary>
	void FixedUpdate () {
    PositionCamera();
    m_unityUtil.StepLeap (Time.deltaTime);
	}

  public void PositionCamera() {
    Transform referenceObject = ReferenceObject;
    if(referenceObject == null) {
      HandController tmp = GameObject.FindObjectOfType<HandController>(); 
      if (tmp) referenceObject = tmp.transform;
    }
    if(referenceObject == null) {
      GameObject tmp = GameObject.Find("Main Camera"); 
      if (tmp) referenceObject = tmp.transform;
    }
    if(referenceObject == null) {
      return; 
    }

    LeapTransform referenceTransform = new LeapTransform(referenceObject.position, referenceObject.rotation);
    referenceTransform.Position = referenceObject.position;
    referenceTransform.Rotation = referenceObject.rotation;

    Vector3 offsetCameraToLeap = OffsetCameraToLeap * 1.0f/clientUnitLengthInMillimeters;
    offsetCameraToLeap = referenceObject.rotation * offsetCameraToLeap;
    
    referenceTransform.Position = referenceObject.position + offsetCameraToLeap;
    
    Scene.LeapOriginInClient = referenceTransform;
  }

	Leap.Interact.Scene m_scene;
	Leap.Interact.UnityUtil m_unityUtil;
}
