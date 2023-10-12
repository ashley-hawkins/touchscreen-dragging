using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    public float ForceMultiplier;
    Camera cam;
    Dictionary<int, GameObject> currentTouches;
    void Start()
    {
        currentTouches = new();
    }

    void DoMove(Touch touch)
    {
        var fingerId = touch.fingerId;
        if (!currentTouches.ContainsKey(fingerId)) return;

        var obj = currentTouches[fingerId];
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        print("DOING MOVE");

        var worldPoint = cam.ScreenToWorldPoint(touch.position);
        var distance = (worldPoint - (Vector3)(rb.position));
        var finalMultiplier = ForceMultiplier;
        if (Vector2.Dot(rb.velocity, distance.normalized) < 0)
        {
            finalMultiplier = 1.2f * finalMultiplier;
        }
        var theForce = Time.deltaTime * ((distance).normalized * finalMultiplier);
        print($"Adding force: {theForce}");
        rb.velocity = theForce;
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
                        print("DOING SOMETHING!!!!");
                        currentTouches[touch.fingerId] = obj;
                        obj.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
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
                currentTouches[touch.fingerId].GetComponent<Rigidbody2D>().gravityScale = 1.0f;
                currentTouches.Remove(touch.fingerId);
            }
        }
    }
}
