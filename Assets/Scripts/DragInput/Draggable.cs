using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    bool active;
    int fingerId;
    Vector3 offset;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        active = false;
    }

    public void BeginTouch(TouchDragManager.DragOptions opts, Touch touch, Vector3 worldPos)
    {
        active = true;
        fingerId = touch.fingerId;
        offset = Quaternion.Inverse(transform.rotation) * (worldPos - transform.position);
        DoMove(touch, opts);
    }
    public void ContinueTouch(TouchDragManager.DragOptions opts, Touch touch)
    {
        DoMove(touch, opts);
    }
    public void EndTouch(TouchDragManager.DragOptions opts, Touch touch)
    {
        //var (gravityScale, _, obj) = currentTouches[touch.fingerId];
        //obj.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
        active = false;
    }
    void DoMove(Touch touch, TouchDragManager.DragOptions opts)
    {
        Camera cam = Camera.main;
        var fingerId = touch.fingerId;

        var realOffset = transform.rotation * offset;
        if (rb == null) return;

        var worldPoint = cam.ScreenToWorldPoint(touch.position) - (opts.DragFromOffsetPoint ? realOffset : Vector3.zero);
        var distance = (worldPoint - (Vector3)(rb.position));
        var theForce = ((distance) * opts.ForceMultiplier);
        print($"Adding force: {theForce}");
        if (theForce.magnitude > 0.1f)
        {
            if (opts.DragFromOffsetPoint)
            {
                rb.AddForceAtPosition(((Vector2)theForce - rb.velocity) * rb.mass, transform.position + realOffset, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(((Vector2)theForce - rb.velocity) * rb.mass, ForceMode2D.Impulse);
            }
        }
    }
}
