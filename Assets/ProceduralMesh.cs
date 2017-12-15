using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class ProceduralMesh : MonoBehaviour {

	protected static int SetTriUv(Vector2[] uv, int i, Vector2 v0, Vector2 v1, Vector2 v2) {
		uv [i] = v0;
		uv [i+1] = v1;
		uv [i+2] = v2;
		return i + 3;
	}

	protected static int SetTri(int[] triangles, int i, int v0, int v1, int v2) {
		triangles [i] = v0;
		triangles [i+1] = v1;
		triangles [i+2] = v2;
		return i + 3;
	}

	protected static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11) {
		triangles [i] = v00;
		triangles [i+1] = triangles [i+4] = v01;
		triangles [i+2] = triangles [i+3] = v10;
		triangles [i+5] = v11;
		return i + 6;
	}

	protected bool gizmos = false;
	protected Mesh mesh;
	protected Vector3[] vertices;
	protected Vector2[] uv;
	protected Vector4[] tangents;
	protected int[] triangles;

	protected virtual void Awake() {
		Generate ();
	}

	private void Generate () {
		GetComponent<MeshFilter> ().mesh = mesh = new Mesh ();
		mesh.name = MeshName ();
		CreateVertices ();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;
		CreateTriangles ();
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
	}

	protected virtual string MeshName () {
		return "Procedural Mesh";
	}

	protected abstract void CreateVertices ();
	protected abstract void CreateTriangles ();

	private void OnDrawGizmos () {
		if (!gizmos || mesh == null || mesh.vertices == null) return;
		Gizmos.color = Color.black;
		foreach (Vector3 t in mesh.vertices) {
			Gizmos.DrawSphere (t, 0.1f);
		}
	}
}
