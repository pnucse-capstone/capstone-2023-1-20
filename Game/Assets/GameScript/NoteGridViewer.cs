using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class NoteGridViewer : MonoBehaviour
{
    [SerializeField] Color colorGrid;
    [SerializeField] Color colorNode;
    [SerializeField] Color colorPale;
    [SerializeField]GameObject vertical;
    [SerializeField] GameObject horizontal;
    [SerializeField] GameObject node_number;
    [SerializeField] GameObject LineNumberLabel;
    List<GameObject> VerticalLines;
    List<GameObject> HorizontalLines;
    List<GameObject> labels;
    [SerializeField] Refresher refresh;
    List<GameObject> Nodes;
    public void Zoom(float multiplier)
    {
        NoteEditor.zoom *= multiplier;
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
        Generate();
    }
    void Init()
    {
        VerticalLines = new List<GameObject>();
        HorizontalLines = new List<GameObject>();
        labels = new List<GameObject>();
        Nodes = new List<GameObject>();
        for(int i=0;i<=Game.lineCount; i++)
        {
            HorizontalLines.Add(Instantiate(horizontal));
        }
        for (int i = 0; i < 64; i++)
        {
            VerticalLines.Add(Instantiate(vertical));
        }
        for (int i = 0; i < Game.lineCount; i++)
        {
            var obj = Instantiate(LineNumberLabel);
            obj.transform.position = transform.position + new Vector3(-1, -1F*i*6/Game.lineCount);
            obj.GetComponent<TextMeshPro>().text = (i + 1).ToString();
            labels.Add(obj);
        }
        for (int i = 0; i < 64; i++)
        {
            Nodes.Add(Instantiate(node_number));
        }
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                Zoom(Mathf.Pow(2,1/8F));
                refresh.Refresh();
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                Zoom(Mathf.Pow(0.5F, 1 / 8F));
                refresh.Refresh();
            }
        }
    }
    public void Refresh()
    {
        Generate();
    }

    public void Generate()
    {
        GenerateVertical();

        GenerateHorizontal();
        GenerateNodes();
    }
    void ResizeVertical(int cnt)
    {
        while (VerticalLines.Count < cnt)
        {
            var temp = Instantiate(vertical);
            VerticalLines.Add(temp);
        }
    }
    void ResizeNodes(int cnt)
    {
        while (Nodes.Count < cnt)
        {
            var temp = Instantiate(node_number);
            Nodes.Add(temp);
        }
    }
    private void GenerateVertical()
    {
        List<SnapGrid.Grid> snaps = NoteEditor.snaps.GetGrids();
        ResizeVertical(snaps.Count + 1);
        foreach (var i in VerticalLines)
        {
            i.SetActive(false);
        }
        for(int i=0;i<snaps.Count;i++)
        {
            var temp = VerticalLines[i];
            temp.GetComponent<SpriteRenderer>().sortingOrder = 1;

            temp.transform.position = transform.position + Vector3.right * (float)snaps[i].time * NoteEditor.zoom;
            temp.GetComponent<SpriteRenderer>().color = colorNode;
            switch (snaps[i].GridType)
            {
                case SnapGrid.GridType.Node:
                    temp.transform.localScale = new Vector3(0.06F, 6, 1);
                    temp.GetComponent<SpriteRenderer>().color = colorNode;
                    
                    break;
                case SnapGrid.GridType.Thick:
                    temp.transform.localScale = new Vector3(0.02F, 6, 1);
                    temp.GetComponent<SpriteRenderer>().color = colorGrid;
                    break;

                case SnapGrid.GridType.Thin:
                    temp.transform.localScale = new Vector3(0.02F, 6, 1);
                    temp.GetComponent<SpriteRenderer>().color = colorPale;
                    break;
            }
            temp.SetActive(true);
        }
    }
    private void GenerateNodes()
    {
        List<SnapGrid.Grid> snaps = NoteEditor.snaps.GetGrids().Where(x=>x.GridType == SnapGrid.GridType.Node).ToList();

        ResizeNodes(snaps.Count+ 1);
        foreach (var i in Nodes)
        {
            i.SetActive(false);
        }
        for (int i = 0; i < snaps.Count; i++)
        {
            var temp = Nodes[i];
            temp.GetComponent<TextMesh>().text = (i + 1) + "";
            temp.SetActive(true);
            temp.transform.position = transform.position + Vector3.right * (float)snaps[i].time * NoteEditor.zoom;
        }
    }
    private void GenerateHorizontal()
    {
        for (int i = 0; i <= Game.lineCount; i++)
        {
            var temp = HorizontalLines[i];
            temp.GetComponent<SpriteRenderer>().color = colorPale;
            temp.GetComponent<SpriteRenderer>().sortingOrder = 0;

            //위치 적용
            temp.transform.position = transform.position + Vector3.down * i * 6 / Game.lineCount;

            // 가로 길이를 적용
            temp.transform.localScale = new Vector3(MusicPlayer.expandedlength*NoteEditor.zoom, 0.02F,1);
        }
    }
}
