using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaleteDataNS;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int time;
    [SerializeField] private int size;
    public GameObject white;
    public GameObject black;

    private bool isClickingBall;
    private GameObject playArea;
    private float paleteSize;
    private GameObject[,] listPalete;
    private PaleteData[,] listPaleteData;
    private Vector2 startPos, targetPos;
    private ListPath route; //listPath
    private struct NodeMetrics
    {
        public int posCol;
        public int posRow;
        public bool isVisited;
        public float metric;
        public List<NodeMetrics> listNode;

        public void SetPos(int col, int row)
        {
            posCol = col;
            posRow = row;
        }

        public void SetVisited(bool isV)
        {
            isVisited = isV;
        }
    }

    [HideInInspector]
    public struct ListPath
    {
        public List<Vector2> pathRoute;
        public bool isCorrect;

        public void SetCorrect(bool isTrue)
        {
            isCorrect = isTrue;
        }
        public void InsertToPath(Vector2 newPath)
        {
            if (pathRoute.Count == 0)
            {
                pathRoute = new List<Vector2>();
            }
            pathRoute.Insert(0, newPath);
        }
    }

    private void Awake()
    {
        playArea = GameObject.FindGameObjectWithTag("PlayArea");
        paleteSize = 768 / (float)size;
        listPalete = new GameObject[size, size];
        listPaleteData = new PaleteData[size, size];
        isClickingBall = false;
        ResetClickPosition();
    }

    private void Update()
    {
    }

    public void ResetClickPosition()
    {
        startPos = new Vector2(999, 999);
        targetPos = new Vector2(999, 999);
        isClickingBall = false;
    }

    public void GeneratePaleteGrid()
    {
        Vector3 initPos = new Vector3(0, 0, 0);
        white.GetComponent<RectTransform>().sizeDelta = new Vector2(paleteSize, paleteSize);
        black.GetComponent<RectTransform>().sizeDelta = new Vector2(paleteSize, paleteSize);

        for (int col = 0; col < size; col++)
        {
            for (int row = 0; row < size; row++)
            {
                if ((col % 2 == 0 && row % 2 == 0) || (col % 2 != 0 && row % 2 != 0))
                {
                    var result = GameObject.Instantiate(white,
                        Vector3.zero,
                        Quaternion.identity, playArea.transform);

                    result.name = "Palete" + col + "," + row;
                    result.transform.localPosition = new Vector3(initPos.x + col * paleteSize, initPos.y - row * paleteSize, 0);
                    listPalete[col, row] = result;
                    listPaleteData[col, row] = new PaleteData(col, row);
                }
                else
                {
                    var result = GameObject.Instantiate(black,
                        Vector3.zero,
                        Quaternion.identity, playArea.transform);

                    result.name = "Palete" + col + "," + row;
                    result.transform.localPosition = new Vector3(initPos.x + col * paleteSize, initPos.y - row * paleteSize, 0);
                    listPalete[col, row] = result;
                    listPaleteData[col, row] = new PaleteData(col, row);
                }
            }
        }

        Debug.Log(listPalete[0, 0].transform.localPosition);
    }

    public void ClickAPositon(int posCol, int posRow)
    {
        if (listPaleteData[posCol, posRow].GetFill() && !isClickingBall)
        {
            startPos = new Vector2(posCol, posRow);
            isClickingBall = true;
        }
        else if (listPaleteData[posCol, posRow].GetFill() && isClickingBall)
        {
            return;
        }
        else if (!listPaleteData[posCol, posRow].GetFill() && !isClickingBall)
        {
            return;
        }
        else if (!listPaleteData[posCol, posRow].GetFill() && isClickingBall)
        {
            targetPos = new Vector2(posCol, posRow);
            isClickingBall = false;
            Debug.Log(startPos + "<>" + targetPos);

            route = FindPath(listPaleteData, startPos, targetPos);
            if (!route.isCorrect)
            {
                return;
            }
            else
            {
                isClickingBall = false;
                //setFill at StartPos and TargetPosition
            }
            //ResetClickPosition();
        }
    }

    public ListPath FindPath(PaleteData[,] listData, Vector2 startPos, Vector2 targetPos)
    {
        //init state;
        float average = Mathf.Sqrt(Mathf.Pow(targetPos.x - startPos.x, 2) + Mathf.Pow(targetPos.y - startPos.y, 2));
        var listMetrics = new NodeMetrics[listData.GetLength(1), listData.GetLength(0)];

        for (int row = 0; row < listData.GetLength(0); row++)
        {
            for (int col = 0; col < listData.GetLength(1); col++)
            {
                //if listPalete[col, row].isFill 
                NodeMetrics node = new NodeMetrics();
                node.posCol = col;
                node.posRow = row;
                node.metric = Mathf.Sqrt(Mathf.Pow(targetPos.x - col, 2) + Mathf.Pow(targetPos.y - row, 2));
                node.listNode = new List<NodeMetrics>();

                NodeMetrics nodeTop = new NodeMetrics();
                nodeTop.SetPos(row - 1, col);
                AddNode(ref node.listNode, nodeTop);

                NodeMetrics nodeLeft = new NodeMetrics();
                nodeTop.SetPos(row, col - 1);
                AddNode(ref node.listNode, nodeLeft);

                NodeMetrics nodeBottom = new NodeMetrics();
                nodeTop.SetPos(row + 1, col);
                AddNode(ref node.listNode, nodeBottom);

                NodeMetrics nodeRight = new NodeMetrics();
                nodeTop.SetPos(row, col + 1);
                AddNode(ref node.listNode, nodeRight);

                listMetrics[col, row] = node;
            }
        }


        return RecurTree(listMetrics[(int)startPos.x, (int)startPos.y], targetPos);
    }

    private ListPath RecurTree(NodeMetrics node, Vector2 target, ListPath listPath = new ListPath())
    {
        // if (node.posCol == target.x && node.posRow == target.y) return;
        //get min node
        List<NodeMetrics> listNode = new List<NodeMetrics>();
        listNode = SortMetrics(node.listNode);
        Debug.Log(listNode);

        for (int i = 0; i < listNode.Count; i++)
        {
            if (listNode[i].isVisited) continue;
            if (listNode[i].posCol == target.x && listNode[i].posRow == target.y)
            {
                //add to list path 
                listPath.InsertToPath(target);
                listPath.SetCorrect(true);
                return listPath;
            }

            ListPath result = new ListPath();
            result = RecurTree(listNode[i], target, listPath);
            if (result.isCorrect)
            {
                Debug.Log("Path:" + listNode[i].posCol + "," + listNode[i].posRow);
                result.InsertToPath(new Vector2(listNode[i].posCol, listNode[i].posRow));
                return result;
            }
            else
            {
                listNode[i].SetVisited(true);
            }
        }
        return listPath;
    }

    private List<NodeMetrics> SortMetrics(List<NodeMetrics> listNode)
    {
        for (int i = 0; i < listNode.Count - 1; i++)
        {
            for (int j = 1; j < listNode.Count; j++)
            {
                if (listNode[i].metric >= listNode[j].metric)
                {
                    var temp = listNode[i];
                    listNode[i] = listNode[j];
                    listNode[j] = temp;
                }
            }
        }

        return listNode;
    }

    private void AddNode(ref List<NodeMetrics> list, NodeMetrics node) //x = col
    {
        if (node.posCol < 0 || node.posCol >= listPalete.GetLength(1) || node.posRow < 0 || node.posRow >= listPalete.GetLength(0)
            || listPaleteData[node.posCol, node.posRow].GetFill())
        {
            return;
        }

        list.Add(node);
    }
}
