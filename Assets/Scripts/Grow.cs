using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Grow : MonoBehaviour
{
    public GameObject Segment;
    public int recurseCount = 2;
    
    void OnEnable()
    {
        GrowTree();
    }

    void GrowTree() {
    	DestroyChildren();

        // create trunk at 000
        GameObject trunk = PrefabUtility.InstantiatePrefab(Segment) as GameObject;
        trunk.transform.parent = transform;

        // create branches
        RecurseGrow(recurseCount-1, trunk.transform);
    }

    void RecurseGrow(int remainingCount, Transform t) {
        if(remainingCount > 0) {
            remainingCount--;
            Debug.Log(remainingCount);

            Transform o = GrowBranch(t);
            RecurseGrow(remainingCount, o);
        } 
    }

    Transform GrowBranch(Transform t) {
        GameObject o = PrefabUtility.InstantiatePrefab(Segment) as GameObject;
        o.transform.parent = t;
        o.transform.localPosition = new Vector3(0, 1f, 0);

        return o.transform;
    }


    void DestroyChildren() 
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
