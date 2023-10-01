using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatCam : MonoBehaviour
{
    private Transform _camTransform;
    // Start is called before the first frame update
    void Start()
    {
        _camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(_camTransform);
        transform.Rotate(new Vector3(0, 180, 0));
    }
}
