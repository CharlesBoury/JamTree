using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Grow : MonoBehaviour
{
    public GameObject Segment;
    public int recurseCount = 2;
    // Start is called before the first frame update
    void OnEnable()
    {
        DestroyChildren();
        RecurseGrow(recurseCount, transform);
    }

    void Update()
    {
        DestroyChildren();
        RecurseGrow(recurseCount, transform);
    }

    void DestroyChildren() 
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    void RecurseGrow(int remainingCount, Transform t) {
        if(remainingCount > 0) {
            remainingCount--;
            Debug.Log(remainingCount);
            GameObject o = Instantiate(Segment, t);
            RecurseGrow(remainingCount, o.transform);
        } 
    }
}
