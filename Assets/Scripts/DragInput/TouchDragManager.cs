using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDragManager : MonoBehaviour
{
    [System.Serializable]
    public struct DragOptions
    {
        public float ForceMultiplier;
        public bool DragFromOffsetPoint;
    };
    [Header("Options")]
    public DragOptions Options = new()
    {
        ForceMultiplier = 10,
        DragFromOffsetPoint = true
    };
    Camera cam;
    Dictionary<int, Draggable> currentTouches;
    void Start()
    {
        currentTouches = new();
    }

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main;
        
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            var touchPos = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                print($"Touch began with ID {touch.fingerId}");
                var worldPoint = cam.ScreenToWorldPoint(touchPos);
                var overlappingColliders = Physics2D.OverlapPointAll(worldPoint);
                foreach (var collider in overlappingColliders)
                {
                    var obj = collider.gameObject;
                    print($"Processing object: {obj}");
                    var draggable = obj.GetComponent<Draggable>();
                    if (draggable != null)
                    {
                        currentTouches[touch.fingerId] = draggable;
                        draggable.BeginTouch(Options, touch, worldPoint);
                        break;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                currentTouches[touch.fingerId].ContinueTouch(Options, touch);
            }
            else
            {
                if (!currentTouches.ContainsKey(touch.fingerId)) return;
                print($"Touch ended with ID {touch.fingerId}");
                currentTouches[touch.fingerId].EndTouch(Options, touch);
                currentTouches.Remove(touch.fingerId);
            }
        }
    }
}
