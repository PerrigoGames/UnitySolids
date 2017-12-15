using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGrid : ProceduralMesh {

	public int xSize, ySize;

	protected override void Awake() {
		base.Awake ();
		gizmos = true;
	}

	protected override string MeshName () {
		return "Procedural Grid";
	}

	protected override void CreateVertices () {
		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		uv = new Vector2[vertices.Length];
		tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		for (int i = 0, y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices [i] = new Vector3 (x, Random.Range (0f, 1f), y);
				uv [i] = new Vector2 ((float)x / xSize, (float)y / ySize);
				Debug.Log (string.Format("{0}, {1}: {2}", x, y, uv[i]), this);
				tangents [i] = tangent;
			}
		}
	}

	protected override void CreateTriangles () {
		triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles [ti] = vi;
				triangles [ti + 2] = triangles [ti + 3] = vi + 1;
				triangles [ti + 1] = triangles [ti + 4] = vi + xSize + 1;
				triangles [ti + 5] = vi + xSize + 2;
			}
		}
	}
}
