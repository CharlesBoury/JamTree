using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[ExecuteInEditMode]
public class Grow : MonoBehaviour
{
    public GameObject Segment;
    public int recurseCount = 2;
    // Start is called before the first frame update
    void OnEnable()
    {
        GrowTree();
    }
    
    public void GrowTree() 
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

[CustomEditor(typeof(Grow))]
public class GrowEditor: Editor
{
     public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if(GUILayout.Button("Grow!")) {
            Debug.Log("It's alive: " + target.name);
            Grow GrowScript = ((MonoBehaviour)target).gameObject.GetComponent<Grow>();
            GrowScript.GrowTree();
            //Grow.GrowTree();
        }
        
    }
}