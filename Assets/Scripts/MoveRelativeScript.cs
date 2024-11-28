using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveRelativeScript : MonoBehaviour
{
    public List<GameObject> objects;
    public List<Vector3> offsets;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject targetObject = objects[i];
            Vector3 targetOffset = offsets[i];
            targetObject.transform.position = transform.TransformPoint(targetOffset);
        }
    }
}
