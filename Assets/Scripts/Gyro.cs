using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyro : MonoBehaviour
{
    // Start is called before the first frame update
    LineRenderer lr;
    static Quaternion offset = Quaternion.identity;
    public GameObject cube;
    static float gravityMagnitude = 9.81f;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        gravityMagnitude = 1;
        Input.gyro.enabled = true;
    }
    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(0, 0, -q.z, -q.w);
    }

    public void Calibrate()
    {
        offset = Quaternion.Inverse(GyroToUnity(Input.gyro.attitude));
    }

    // Update is called once per frame
    void Update()
    {
        //print(GyroToUnity(Input.gyro.attitude).eulerAngles);
        // cube.transform.rotation = GyroToUnity(Input.gyro.attitude) * offset;
        Physics2D.gravity = Quaternion.Inverse(Quaternion.Euler(0, 0, (GyroToUnity(Input.gyro.attitude) * offset).eulerAngles.z)) * Vector2.down * gravityMagnitude;
        var unitGravity = Physics2D.gravity.normalized;
        lr.SetPosition(0, unitGravity * 0.5f);
        lr.SetPosition(1, -unitGravity * 0.5f);
        print(Physics2D.gravity);
    }
}