using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCube : ProceduralMesh {

	public int xSize, ySize, zSize;

	protected override void Awake() {
		base.Awake ();
		gizmos = true;
	}

	protected override string MeshName () {
		return "Procedural Cube";
	}

	protected override void CreateVertices() {
		int cornerVertices = 8;
		int edgeVertices = (xSize + ySize + zSize - 3) * 4;
		int faceVertices = (
			(xSize - 1) * (ySize - 1) +
			(xSize - 1) * (zSize - 1) +
			(ySize - 1) * (zSize - 1)) * 2;
		vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
		int v = 0;
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				vertices [v++] = new Vector3 (x, y, 0);
			}
			for (int z = 1; z <= zSize; z++) {
				vertices [v++] = new Vector3 (xSize, y, z);
			}
			for (int x = xSize - 1; x >= 0; x--) {
				vertices [v++] = new Vector3 (x, y, zSize);
			}
			for (int z = zSize - 1; z > 0; z--) {
				vertices [v++] = new Vector3 (0, y, z);
			}
		}
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				vertices [v++] = new Vector3 (x, ySize, z);
			}
		}
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				vertices [v++] = new Vector3 (x, 0, z);
			}
		}
		mesh.vertices = vertices;
	}

	protected override void CreateTriangles() {
		int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
		int[] triangles = new int[quads * 6];
		int ring = (xSize + zSize) * 2;
		int t = 0, v = 0;

		for (int q = 0; q < ring - 1; q++, v++) {
			t = SetQuad (triangles, t, v, v + 1, v + ring, v + ring + 1);
		}
		t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);

		mesh.triangles = triangles;
	}
}
