using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
 [RequireComponent (typeof (MeshFilter))]
public class Grow : MonoBehaviour
{
    public GameObject Segment;
    public int recurseCount = 2;
    public float initRadius = 1f;
    public float scaleRadius = 0.8f;

    [Header("Random Settings")]
    public int seed = 42;
    public float rotationRangeZ = 20f;
    public float rotationRangeY = 20f;
    
    void OnEnable()
    {
        GrowTree();
    }

    public void GrowTree() {
    	DestroyChildren();

        // create trunk at 000
        GameObject trunk = Instantiate(Segment);
        trunk.transform.parent = transform;

        // create branches
        Random.InitState(seed);
        RecurseGrow(recurseCount-1, trunk.transform, initRadius);
    }

    void RecurseGrow(int remainingCount, Transform t, float radius) {
        if(remainingCount > 0) {
            remainingCount--;
            radius = radius * scaleRadius;
            Debug.Log(remainingCount);

            Transform o = GrowBranch(t, radius);
            RecurseGrow(remainingCount, o, radius);
        } 
    }

    Transform GrowBranch(Transform t, float radius) {
        //GameObject o = PrefabUtility.InstantiatePrefab(Segment) as GameObject;
        GameObject o = Instantiate(Segment);
        Deform(o, radius);
        o.transform.parent = t;
        o.transform.localPosition = new Vector3(0, 1f, 0);
        o.transform.localEulerAngles = new Vector3(
            0, 
            rotationRangeY* (Random.value - 0.5f), 
            rotationRangeZ* (Random.value - 0.5f));

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

	void Deform(GameObject o, float radius) {

		Mesh originalMesh = o.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] originalVertices = originalMesh.vertices;
		Vector3[] displacedVertices = new Vector3[originalVertices.Length];
		
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = CalculateDeformation(originalVertices[i], radius, radius * scaleRadius);
		}
		
        Mesh deformedMesh = new Mesh();
        deformedMesh.vertices = displacedVertices;
        deformedMesh.triangles = originalMesh.triangles;
        deformedMesh.uv = originalMesh.uv;
        deformedMesh.RecalculateNormals();
        o.GetComponent<MeshFilter>().mesh = deformedMesh;
		
	}

    // Useful for later, not used yet
	Vector2 PolarFromPosition(Vector3 position) {
		float theta = Vector2.Angle(new Vector2(1.0f, 0.0f), new Vector2(position.x, position.z)) * Mathf.PI / 180.0f;
		return new Vector2(theta, position.y);
	}


	Vector3 CalculateDeformation(Vector3 position, float radiusMax, float radiusMin) {
		float scale = (1.0f-position.y) * radiusMax + position.y * radiusMin;
		return new Vector3(scale * position.x,  position.y, scale * position.z);
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