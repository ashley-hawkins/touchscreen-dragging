using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrag : MonoBehaviour
{
    public float ForceMultiplier = 10;
    public bool DragFromOffsetPoint = true;
    Camera cam;
    Dictionary<int, (float, Vector3, GameObject)> currentTouches;
    void Start()
    {
        currentTouches = new();
    }

    void DoMove(Touch touch)
    {
        var fingerId = touch.fingerId;
        if (!currentTouches.ContainsKey(fingerId)) return;

        var (_, offset, obj) = currentTouches[fingerId];
        offset = obj.transform.rotation * offset;
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        var worldPoint = cam.ScreenToWorldPoint(touch.position) - offset;
        var distance = (worldPoint - (Vector3)(rb.position));
        var theForce = ((distance) * ForceMultiplier);
        print($"Adding force: {theForce}");
        if (theForce.magnitude > 0.1f)
        {
            rb.AddForceAtPosition(((Vector2)theForce - rb.velocity) * rb.mass, obj.transform.position + offset, ForceMode2D.Impulse);
        }
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
                    if (obj.CompareTag("draggable"))
                    {
                        var rb = obj.GetComponent<Rigidbody2D>();
                        currentTouches[touch.fingerId] = (rb.gravityScale, Quaternion.Inverse(obj.transform.rotation) * (worldPoint - obj.transform.position), obj);
                        //rb.gravityScale = 0.0f;
                        DoMove(touch);
                        break;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                DoMove(touch);
            }
            else
            {
                print($"Touch ended with ID {touch.fingerId}");
                if (!currentTouches.ContainsKey(touch.fingerId)) return;
                var (gravityScale, _, obj) = currentTouches[touch.fingerId];
                //obj.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
                currentTouches.Remove(touch.fingerId);
            }
        }
    }
}
