using System.Collections.Generic;
using UnityEngine;

public class NoteLineRenderer : MonoBehaviour
{
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> indexes = new List<int>();
    [SerializeField]
    bool isOutline;
    public int sortingOrder
    {
        get => GetComponent<MeshRenderer>().sortingOrder;
        set => GetComponent<MeshRenderer>().sortingOrder = value;
    }
    MeshFilter filter;
    public Material material => GetComponent<MeshRenderer>().material;
    Mesh meshBack;
    void Start()
    {
        GetComponent<MeshRenderer>().sortingLayerName = "Line";
        if (isOutline)
        {
        }
        else
        {
            material.SetTexture("_MainTex", StreamingImageAssets.GetLineImage().texture);
        }
        filter = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        meshBack = new Mesh();
        filter.mesh = mesh;
    }
    void DrawClear()
    {
        vertices.Clear();
        indexes.Clear();
        uvs.Clear();
    }
    public float Normalize(float value, float a, float b)
    {
        return (value - a) / (b - a);
    }
    public void RenderLinePath(LinePath.Point[] path, float thick, Note note)
    {
        DrawClear();
        transform.rotation = Quaternion.identity;

        Vector3 prev_n = Vector3.zero; // 이새끼가 문제임
        var degree = note.Rotation().eulerAngles.z;//디그리라고 가정

        Quaternion rotation = Quaternion.Euler(0, 0, degree);
        Matrix4x4 matrix = Matrix4x4.Rotate(rotation);
        prev_n = Vector3.zero;

        for (int i = 1; i < path.Length; i++)
        {
            Vector3 pos1 = new Vector3(path[i - 1].x, path[i - 1].y);
            Vector3 pos2 = new Vector3(path[i].x, path[i].y);
            Vector3 dir = (pos2 - pos1).normalized;
            Vector3 n = new Vector3(dir.y, -dir.x);
            if (n != Vector3.zero)
            {
                vertices.Add(pos1 - n * thick * 0.5F - transform.position);
                vertices.Add(pos1 + n * thick * 0.5F - transform.position);
                vertices.Add(pos2 - n * thick * 0.5F - transform.position);
                vertices.Add(pos2 + n * thick * 0.5F - transform.position);

                var index = vertices.Count - 4;
                indexes.Add(index);
                indexes.Add(index + 1);
                indexes.Add(index + 3);
                indexes.Add(index + 3);
                indexes.Add(index + 2);
                indexes.Add(index);

                float start_t = path[0].t;
                float end_t = path[path.Length - 1].t;
                float t1 = Normalize(path[i - 1].t, start_t, end_t);
                float t2 = Normalize(path[i].t, start_t, end_t);
                uvs.Add(new Vector2(t1, 1));
                uvs.Add(new Vector2(t1, 0));
                uvs.Add(new Vector2(t2, 1));
                uvs.Add(new Vector2(t2, 0));
            }
            //            Debug.Log((JsonConvert.SerializeObject(vertices.Select(x => (x: x.x, y: x.y))), JsonConvert.SerializeObject(indexes.Select(x => (x))), JsonConvert.SerializeObject(uvs.Select(x => (x.x, x.y)))));
        }
        Flush();
    }
    void Flush()
    {
        //        Debug.Assert(vertices.Count == uvs.Count);
        meshBack.triangles = null;
        meshBack.vertices = null;
        meshBack.uv = null;
        meshBack.SetVertices(vertices);
        meshBack.triangles = indexes.ToArray();
        meshBack.uv = uvs.ToArray();
        meshBack.RecalculateBounds();
        var temp = filter.mesh;
        filter.mesh = meshBack;
        meshBack = temp;
    }

    public void SetVisible(bool v)
    {
        GetComponent<Renderer>().enabled = v;
    }
}
