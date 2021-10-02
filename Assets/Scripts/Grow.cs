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
    float minLength = 0.2f;
    float minRadius = 0.1f;

    [Header("Random Settings")]
    public int seed = 42;
    public float rotationRangeZ = 20f;
    public float rotationRangeY = 20f;
    public float initRadius = 1f;
    public float scaleRadius = 0.8f;
    public float initLength = 1f;
    public float scaleLength = 0.9f;
    [Range(0f, 1f)] public float chanceOfBourgeon = 0.9f;

    int numberOfBranches;
    
    public class Branch 
    {
        public Transform t;
        public float radius;
        public float length;
        public float weight;

        public Branch(Transform ct, float cradius, float clength, float cweight) {
            t = ct;
            radius = cradius;
            length = clength;
            weight = cweight;
        }
    }

    void OnEnable()
    {
        GrowTree();
    }

    public void GrowTree() {
    	DestroyChildren();

        numberOfBranches = 0;

        // create trunk at 000
        GameObject trunk = Instantiate(Segment);
        Deform(trunk, initRadius, initLength);
        trunk.transform.parent = transform;
        Branch trunkBranch = new Branch(trunk.transform, initRadius, initLength, 1.0f);
        numberOfBranches ++;

        // create branches
        Random.InitState(seed);
        RecurseGrow(recurseCount-1, trunkBranch);
        Debug.Log("weight of trunk: " + trunkBranch.weight + " Tons");
        Debug.Log("numberOfBranches: " + numberOfBranches);
    }

    float RecurseGrow(int remainingCount, Branch b) {
        float weightSum = 0.0f;
        if(remainingCount > 0) {
            remainingCount--;
            Branch newBranch = GrowBranch(b);

            if (Random.value < chanceOfBourgeon)
                weightSum += RecurseGrow(remainingCount, newBranch);
            if (Random.value < chanceOfBourgeon * 0.6667f)
                weightSum += RecurseGrow(remainingCount, newBranch);
            if (Random.value < chanceOfBourgeon * 0.1f)
                weightSum += RecurseGrow(remainingCount, newBranch);
            b.weight += weightSum;
            return b.weight;
        } else {
            return 0.0f;
        }
        
    }

    Branch GrowBranch(Branch b) {
        GameObject o = Instantiate(Segment);

        float radius = b.radius * scaleRadius;
        float length = b.length * scaleLength;
        float weight = 0.2f*0.2f*radius*radius * 2 * 3.14f * length;
        if(radius < minRadius) {radius = minRadius;}
        if(length < minLength) {length = minLength;}

        Deform(o, radius, length);
        o.transform.parent = b.t;
        o.transform.localPosition = new Vector3(0, b.length, 0);
        o.transform.localEulerAngles = new Vector3(
            0, 
            rotationRangeY* (Random.value - 0.5f), 
            rotationRangeZ* (Random.value - 0.5f));

        numberOfBranches++;
        
        return new Branch(o.transform, radius, length, weight);
    }


    void DestroyChildren() 
    {
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

	void Deform(GameObject o, float radius, float length) {

		Mesh originalMesh = o.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] originalVertices = originalMesh.vertices;
		Vector3[] displacedVertices = new Vector3[originalVertices.Length];
		
		for (int i = 0; i < originalVertices.Length; i++) {
			displacedVertices[i] = CalculateDeformation(originalVertices[i], radius, radius * scaleRadius, length);
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


	Vector3 CalculateDeformation(Vector3 position, float radiusMax, float radiusMin, float length) {
		float scale = (1.0f-position.y) * radiusMax + position.y * radiusMin;
		return new Vector3(scale * position.x,  length * position.y, scale * position.z);
	}
}

[CustomEditor(typeof(Grow))]
public class GrowEditor: Editor
{
     public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if(GUILayout.Button("Grow!")) {
            Grow GrowScript = ((MonoBehaviour)target).gameObject.GetComponent<Grow>();
            GrowScript.GrowTree();
        }
        if(GUILayout.Button("Grow random!")) {
            Grow GrowScript = ((MonoBehaviour)target).gameObject.GetComponent<Grow>();
            GrowScript.seed = Random.Range(0, 100000);
            GrowScript.GrowTree();
        }
    }
}