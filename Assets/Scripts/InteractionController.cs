using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

[Serializable]
public class ARObjectPlacedEvent : UnityEvent<InteractionController, GameObject> { }
public class InteractionController : ARBaseGestureInteractable
{
    
    [SerializeField]
        [Tooltip("A GameObject to place when a raycast from a user touch hits a plane.")]
        GameObject m_PlacementPrefab;

        /// <summary>
        /// A <see cref="GameObject"/> to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject placementPrefab
        {
            get => m_PlacementPrefab;
            set => m_PlacementPrefab = value;
        }

        [SerializeField] private GameObject crosshair;
        [SerializeField, Tooltip("Called when the this interactable places a new GameObject in the world.")]
        ARObjectPlacedEvent m_OnObjectPlaced = new ARObjectPlacedEvent();

        /// <summary>
        /// The event that is called when the this interactable places a new <see cref="GameObject"/> in the world.
        /// </summary>
        public ARObjectPlacedEvent onObjectPlaced
        {
            get => m_OnObjectPlaced;
            set => m_OnObjectPlaced = value;
        }

        static readonly List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        static GameObject s_TrackablesObject;

        private Pose pose;
        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Returns <see langword="false"/> otherwise.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            // Allow for test planes
            if (gesture.TargetObject == null || gesture.TargetObject.layer == 9) // TODO Placement gesture layer check should be configurable
                return true;

            return false;
        }

        bool IsPointerOverUI(TapGesture touch)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        private void FixedUpdate()
        {
            if(crosshair.activeSelf)   
                CrosshairCalculation();
        }

        void CrosshairCalculation()
        {
            Vector3 origin = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
            //Ray ray = arCam.ScreenPointToRay(origin);

            if (GestureTransformationUtility.Raycast(origin, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                pose = s_Hits[0].pose;
                crosshair.transform.position = pose.position;
                crosshair.transform.eulerAngles = new Vector3(90,0,0);
            }
        }

        public void onFinishPlacement()
        {
            crosshair.SetActive(false);
        }
        
        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            
            if (gesture.WasCancelled || !crosshair.activeSelf)
                return;

            // If gesture is targeting an existing object we are done.
            // Allow for test planes
            if (gesture.TargetObject != null && gesture.TargetObject.layer != 9) // TODO Placement gesture layer check should be configurable
                return;

            if (IsPointerOverUI(gesture))
                return;
            if (GestureTransformationUtility.Raycast(gesture.startPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hit = s_Hits[0];
                if (Vector3.Dot(Camera.main.transform.position - hit.pose.position,
                                 hit.pose.rotation * Vector3.up) < 0)
                             return;
                GameObject placementObject = Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation);
                var anchorObject = new GameObject("PlacementAnchor");
                anchorObject.transform.position = hit.pose.position;
                anchorObject.transform.rotation = hit.pose.rotation;
                placementObject.transform.parent = anchorObject.transform;
                // if (s_TrackablesObject == null)
                //     s_TrackablesObject = GameObject.Find("Trackables");
                // if (s_TrackablesObject != null)
                //     anchorObject.transform.parent = s_TrackablesObject.transform;
                m_OnObjectPlaced?.Invoke(this, placementObject);
            }
            // Raycast against the location the player touched to search for planes.
            // if (GestureTransformationUtility.Raycast(gesture.startPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            // {
            //     var hit = s_Hits[0];
            //
            //     // Use hit pose and camera pose to check if hittest is from the
            //     // back of the plane, if it is, no need to create the anchor.
            //     if (Vector3.Dot(Camera.main.transform.position - hit.pose.position,
            //             hit.pose.rotation * Vector3.up) < 0)
            //         return;
            //
            //     // Instantiate placement prefab at the hit pose.
            //     var placementObject = Instantiate(placementPrefab, hit.pose.position, hit.pose.rotation);
            //
            // Create anchor to track reference point and set it as the parent of placementObject.
            // TODO This should update with a reference point for better tracking.
            // var anchorObject = new GameObject("PlacementAnchor");
            // anchorObject.transform.position = hit.pose.position;
            // anchorObject.transform.rotation = hit.pose.rotation;
            // placementObject.transform.parent = anchorObject.transform;
            
            // Find trackables object in scene and use that as parent
            // if (s_TrackablesObject == null)
            //     s_TrackablesObject = GameObject.Find("Trackables");
            // if (s_TrackablesObject != null)
            //     anchorObject.transform.parent = s_TrackablesObject.transform;
            //
            //     m_OnObjectPlaced?.Invoke(this, placementObject);
            // }
        }
    }