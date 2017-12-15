using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexGrid : ProceduralMesh {

	private const float AnimationFps = 10;
	private const float YDeltaMod = 0.5f;
	private const float XDeltaMod = 0.5f * 1.732050808f;
	private Vector2[][] _uvVecs;
	private float[][] _cellHeights;
	private float[][][] _vertexHeights;
	private bool _vertexAnimation;
	private bool _equalTriMode = false;

	public int XSize, YSize;
	public float SideLength;
	public float AnimTime;
	public float ModeTime;

	protected override void Awake () {
		base.Awake ();
		gizmos = true;
	}

	void Update() {
		AnimTime += Time.deltaTime;
		ModeTime += Time.deltaTime;
		if (ModeTime > 1f) {
			ModeTime -= 1;
			_vertexAnimation = !_vertexAnimation;
		}
		if (AnimTime > 1f / AnimationFps) {
			AnimTime -= 1f / AnimationFps;
			Animate();
		}
	}

	protected override void CreateVertices () {
		vertices = new Vector3[XSize * YSize * 6];
		uv = new Vector2[vertices.Length];
		tangents = new Vector4[vertices.Length];
		var tangent = new Vector4(1f, 0f, 0f, -1f);
		for (int i = 0, y = 0; y < YSize; y++) {
			for (var x = 0; x < XSize; x++, i+=6) {
				var offsetX = y % 2 != 0;
				var dX = SideLength * XDeltaMod;
				var vX = x * 2 * dX + (offsetX ? dX : 0);
				var dY = SideLength * YDeltaMod;
				var vY = y * (SideLength + dY);

				var color = Random.Range (0, 4);
				const float height = 0;

				vertices [i] = new Vector3 (vX, height, vY);
				uv [i] = UvColor (color, 0);
				tangents [i] = tangent;
				vertices [i+1] = new Vector3 (vX, height, vY + SideLength);
				uv [i+1] = UvColor (color, 1);
				tangents [i+1] = tangent;
				vertices [i+2] = new Vector3 (vX + dX, height, vY + SideLength + dY);
				uv [i+2] = UvColor (color, 0);
				tangents [i+2] = tangent;
				vertices [i+3] = new Vector3 (vX + 2*dX, height, vY + SideLength);
				uv [i+3] = UvColor (color, 1);
				tangents [i+3] = tangent;
				vertices [i+4] = new Vector3 (vX + 2*dX, height, vY);
				uv [i+4] = UvColor (color, 0);
				tangents [i+4] = tangent;
				vertices [i+5] = new Vector3 (vX + dX, height, vY - dY);
				uv [i+5] = UvColor (color, 1);
				tangents [i+5] = tangent;
			}
		}
//		vertices = new Vector3[((XSize + 1) * 2) * (YSize + 1)];
//		uv = new Vector2[vertices.Length];
//		tangents = new Vector4[vertices.Length];
//		var tangent = new Vector4(1f, 0f, 0f, -1f);
//		for (int i = 0, y = 0; y <= YSize; y++) {
//			for (var x = 0; x <= XSize; x++, i+=2) {
//				var dX = SideLength * XDeltaMod;
//				var vX = x * 2 * dX;
//				var offsetY = y % 2 == 0;
//				var dY = SideLength * YDeltaMod;
//				var vY = y * SideLength;
//
//				var color = Random.Range (0, 4);
//
//				vertices [i] = new Vector3 (vX, 0, offsetY ? vY + dY : vY);
//				uv [i] = UvColor (color, 0);
//				tangents [i] = tangent;
//				vertices [i+1] = new Vector3 (vX + dX, 0, offsetY ? vY : vY + dY);
//				uv [i+1] = UvColor (color, 1);
//				tangents [i+1] = tangent;
//			}
//		}
	}

	protected override void CreateTriangles () {
		var tris = _equalTriMode ? 6 : 4;
		triangles = new int[XSize * YSize * tris * 3];
		for (int i = 0, tI = 0, y = 0; y < YSize; y++) {
			for (var x = 0; x < XSize; x++, i+=6) {
				tI = SetTri (triangles, tI, i, i + 1, i + 2);
				tI = SetTri (triangles, tI, i, i + 2, i + 3);
				tI = SetTri (triangles, tI, i, i + 3, i + 4);
				tI = SetTri (triangles, tI, i, i + 4, i + 5);
			}
		}
//		triangles = new int[XSize * YSize * 12];
//		for (int x = 0, i = 0; x < XSize; x++) {
//			for (var y = 0; y < YSize; y++) {
//				var dRow = (XSize * 2) + 2;
//				var origin = (y * dRow) + (x * 2);
//				if (y % 2 != 0) {
//					origin += 1;
//				}
//
//				i = SetTri (triangles, i, origin, origin + dRow, origin + dRow + 1);
//				i = SetTri (triangles, i, origin, origin + dRow + 1, origin + dRow + 2);
//				i = SetTri (triangles, i, origin, origin + dRow + 2, origin + 2);
//				i = SetTri (triangles, i, origin, origin + 2, origin + 1);
//			}
//		}
	}

	private void PopulateUvVectors() {
		if (_uvVecs != null) return;
		_uvVecs = new Vector2[4][];
		for (var color = 0; color < _uvVecs.Length; color++) {
			_uvVecs [color] = new Vector2[3];
			for (var pos = 0; pos < _uvVecs[color].Length; pos++) {
				_uvVecs [color][pos] = UvColor (color, pos);
//					Debug.Log (string.Format("{0}, {1}: {2}", color, pos, UV_VECS[color][pos]), this);
			}
		}
	}

	private void Animate() {
		if (_cellHeights == null) {
			_cellHeights = new float[XSize][];
			_vertexHeights = new float[XSize][][];
			for (int x = 0, i = 0; x < XSize; x++) {
				_cellHeights[x] = new float[YSize];
				_vertexHeights[x] = new float[YSize][];
				for (var y = 0; y < YSize; y++) {
					_vertexHeights[x][y] = new float[6];
				}
			}
		}
		for (int x = 0, i = 0; x < XSize; x++) {
			for (var y = 0; y < YSize; y++, i += 6) {
				var lastH = _cellHeights[x][y];
				_cellHeights[x][y] = Random.Range(Math.Max(lastH - 0.3f, -1f), Math.Min(lastH + 0.3f, 1f));
//				Debug.Log (string.Format("{0}, {1} ({2}): {3}", x, y, i, _cellHeights[x][y]), this);
				for (var pI = i; pI < i + 6; pI++) {
					_vertexHeights[x][y][pI-i] = Random.Range(Math.Max(lastH - 0.3f, -1f), Math.Min(lastH + 0.3f, 1f));
					vertices[pI].y = _vertexAnimation ? _vertexHeights[x][y][pI-i] : _cellHeights[x][y];
				}
			}
		}
		mesh.vertices = vertices;
	}

	private Vector2 UvColor(int color, int pos) {
		PopulateUvVectors ();
		var x = 0.1f + color % 2 * 0.5f + (pos == 1 ? 0.3f : 0);
		var y = 0.1f + color / 2f * 0.5f + (pos == 2 ? 0.3f : 0);
		return new Vector2 (x, y);
	}
}
