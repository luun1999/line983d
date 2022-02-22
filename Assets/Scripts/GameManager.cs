using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaleteDataNS;
public enum BallColor
{
    RED,
    GREEN,
    BLUE,
    CYAN,
    MAGENTA,
    YELLOW,
    NO_COLOR
}
public class GameManager : MonoBehaviour
{
    [SerializeField] private int time;
    [SerializeField] [Range(5, 11)] private int size;
    [SerializeField] [Range(2, 5)] private int numberBallAppear;
    [SerializeField] [Range(0.1f, 1.0f)] private float smoothBigger;
    public GameObject white;
    public GameObject black;
    [SerializeField] private GameObject ballPrefab;

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

    private void Start()
    {
        //init game
        //create 3 balls big size and 3 balls small size.
        GameInit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            for (int o = 0; o < 15; o++)
                CreateBall(false);
        }
    }

    public void GameInit()
    {
        GeneratePaleteGrid();
        for (int i = 0; i < numberBallAppear; i++)
        {
            CreateBall();
            CreateBall(false);
        }
    }

    public void CreateBall(bool isSmall = true)
    {
        bool canCreate = false;
        int count = 0;
        int col = 0;
        int row = 0;
        int iColor = Random.Range(0, 6);

        while (!canCreate)
        {
            col = Random.Range(0, size);
            row = Random.Range(0, size);

            if (!listPaleteData[col, row].GetFill() && !listPaleteData[col, row].HaveABall) canCreate = true;
            if (count++ > size * size) return;
        }

        GameObject newBall = GameObject.Instantiate(ballPrefab, new Vector3(
            listPalete[col, row].transform.position.x,
            listPalete[col, row].transform.position.y, 10
        ), Quaternion.identity);

        var controller = newBall.GetComponent<BallController>();
        controller.SetColor((BallColor)iColor);
        controller.SetPos(col, row);
        listPaleteData[col, row].HaveABall = true;
        listPaleteData[col, row].BallColor = (BallColor)iColor;

        if (isSmall)
        {
            newBall.transform.localScale = new Vector3((5.0f / size) / 3.0f, (5.0f / size) / 3.0f, 1);
            newBall.name = "smallBall" + "," + controller.PosCol + "," + controller.PosRow;
            newBall.gameObject.tag = "Small Ball";
        }
        else
        {
            newBall.transform.localScale = new Vector3(5.0f / size, 5.0f / size, 1);
            listPaleteData[col, row].SetFill(true);
            listPaleteData[col, row].BallColor = (BallColor)iColor;
            newBall.name = "ball" + "," + controller.PosCol + "," + controller.PosRow;
            newBall.gameObject.tag = "Big Ball";
        }
    }

    public void OnCompletedTurn()
    {
        GameObject[] listSmallBall = GameObject.FindGameObjectsWithTag("Small Ball");
        for (int i = 0; i < listSmallBall.Length; i++)
        {
            BallController ball = listSmallBall[i].GetComponent<BallController>();
            ball.IsSmall = false;
            listSmallBall[i].gameObject.tag = "Big Ball";
            Debug.Log(ball.PosCol + ", " + ball.PosRow);
            listSmallBall[i].name = "ball" + "," + ball.PosCol + "," + ball.PosRow;

            listPaleteData[ball.PosCol, ball.PosRow].SetFill(true);
            listPaleteData[ball.PosCol, ball.PosRow].BallColor = ball.GetColor();

            StartCoroutine(BallBecomeBigger(listSmallBall[i]));
        }

        for (int i = 0; i < numberBallAppear; i++)
        {
            CreateBall();
        }

        //check condition to get point
    }

    public void ResetClickPosition()
    {
        startPos = new Vector2(999, 999);
        targetPos = new Vector2(999, 999);
        isClickingBall = false;
    }

    public void ResetPaleteData(PaleteData data)
    {
        data.HaveABall = false;
        data.SetFill(false);
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
                    result.transform.localPosition =
                        new Vector3(initPos.x + col * paleteSize + paleteSize / 2, initPos.y - row * paleteSize - paleteSize / 2, 0);
                    listPalete[col, row] = result;
                    listPalete[col, row].GetComponent<PaleteController>().SetPos(col, row);
                    listPaleteData[col, row] = new PaleteData(col, row);
                }
                else
                {
                    var result = GameObject.Instantiate(black,
                        Vector3.zero,
                        Quaternion.identity, playArea.transform);

                    result.name = "Palete" + col + "," + row;
                    result.transform.localPosition =
                        new Vector3(initPos.x + col * paleteSize + paleteSize / 2, initPos.y - row * paleteSize - paleteSize / 2, 0);
                    listPalete[col, row] = result;
                    listPalete[col, row].GetComponent<PaleteController>().SetPos(col, row);
                    listPaleteData[col, row] = new PaleteData(col, row);
                }
            }
        }

        Debug.Log(listPalete[0, 0].transform.localPosition);
    }

    public void ClickAPositon(int posCol, int posRow)
    {
        Debug.Log(listPaleteData[posCol, posRow].BallColor);
        if (listPaleteData[posCol, posRow].GetFill() && !isClickingBall)
        {
            startPos = new Vector2(posCol, posRow);
            isClickingBall = true;
            Debug.Log("Click Start Pos: " + startPos);
        }
        else if (listPaleteData[posCol, posRow].GetFill() && isClickingBall)
        {
            Debug.Log("Filled and isWaiting: " + startPos);
            return;
        }
        else if (!listPaleteData[posCol, posRow].GetFill() && !isClickingBall)
        {
            Debug.Log("Not Fill and not waiting: " + startPos);
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
                GameObject ball = GameObject.Find("ball" + "," + (int)startPos.x + "," + (int)startPos.y);
                Debug.Log(ball);

                // for (int i = 0; i < route.pathRoute.Count; i++)
                // {
                //     //get position of palete
                //     Vector2 nextPos = listPalete[(int)route.pathRoute[i].x, (int)route.pathRoute[i].y].transform.position;
                //     //move to new position
                //     Debug.Log(ball.transform.position + "___" + route.pathRoute[i]);
                //     ball.transform.position = Vector2.Lerp(ball.transform.position, nextPos, 1);
                //     //name ball for new position
                // }
                StartCoroutine(MovingBall(route.pathRoute, ball));

                isClickingBall = false;
                //setFill at StartPos and TargetPosition
                ball.name = "ball" + "," + (int)targetPos.x + "," + (int)targetPos.y;
                listPaleteData[(int)targetPos.x, (int)targetPos.y].SetFill(true);
                listPaleteData[(int)targetPos.x, (int)targetPos.y].BallColor =
                    ball.GetComponent<BallController>().GetColor();

                listPaleteData[(int)startPos.x, (int)startPos.y].BallColor = BallColor.NO_COLOR;
                ResetPaleteData(listPaleteData[(int)startPos.x, (int)startPos.y]);
            }
            OnCompletedTurn();
        }
    }

    private IEnumerator BallBecomeBigger(GameObject ball)
    {
        ball.GetComponent<BallController>();
        while (ball.transform.localScale.x < 5.0f / size)
        {
            ball.transform.localScale =
                Vector2.Lerp(ball.transform.localScale, new Vector3(5.0f / size, 5.0f / size, 1), smoothBigger);
            yield return null;
        }
    }
    private IEnumerator MovingBall(List<Vector2> list, GameObject ball)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //get position of palete
            Vector2 nextPos = listPalete[(int)list[i].x, (int)list[i].y].transform.position;
            //move to new position
            Debug.Log(ball.transform.position + "___" + list[i]);
            ball.transform.position = Vector2.Lerp(ball.transform.position, nextPos, 1);
            //name ball for new position
            yield return null;
        }


        //delete if have small ball in target position.
        GameObject small = GameObject.Find("smallBall" + "," + (int)targetPos.x + "," + (int)targetPos.y);
        if (small)
        {
            GameObject.Destroy(small);
        }
    }

    public ListPath FindPath(PaleteData[,] listData, Vector2 startPos, Vector2 targetPos)
    {
        //init state;
        //float average = Mathf.Sqrt(Mathf.Pow(targetPos.x - startPos.x, 2) + Mathf.Pow(targetPos.y - startPos.y, 2));
        var listMetrics = new NodeMetrics[listData.GetLength(1), listData.GetLength(0)];
        Debug.Log(listMetrics.GetLength(1));

        for (int row = 0; row < listData.GetLength(0); row++)
        {
            for (int col = 0; col < listData.GetLength(1); col++)
            {
                //if listPalete[col, row].isFill 
                NodeMetrics node = new NodeMetrics();
                node.posCol = col;
                node.posRow = row;
                node.metric = Mathf.Sqrt(Mathf.Pow(targetPos.x - (float)col, 2) + Mathf.Pow(targetPos.y - row, 2));
                node.listNode = new List<NodeMetrics>();

                listMetrics[col, row] = node;
            }
        }
        for (int row = 0; row < listData.GetLength(0); row++)
        {
            for (int col = 0; col < listData.GetLength(1); col++)
            {
                AddNode(ref listMetrics[col, row].listNode, listMetrics, col - 1, row);
                AddNode(ref listMetrics[col, row].listNode, listMetrics, col, row - 1);
                AddNode(ref listMetrics[col, row].listNode, listMetrics, col + 1, row);
                AddNode(ref listMetrics[col, row].listNode, listMetrics, col, row + 1);

                Debug.Log("Pos: " + col + "," + row);
                foreach (var item in listMetrics[col, row].listNode)
                {
                    Debug.Log(item.posCol + ", " + item.posRow);
                }
            }
        }

        ListPath listPath = new ListPath();
        listPath.pathRoute = new List<Vector2>();

        return RecurTree(ref listMetrics, listMetrics[(int)startPos.x, (int)startPos.y], targetPos, listPath);
    }

    private ListPath RecurTree(ref NodeMetrics[,] arrMetric, NodeMetrics node, Vector2 target, ListPath listPath)
    {
        // if (node.posCol == target.x && node.posRow == target.y) return;
        //get min node
        arrMetric[node.posCol, node.posRow].SetVisited(true);
        List<NodeMetrics> listNode = new List<NodeMetrics>();
        Debug.Log("node" + node.posCol + "," + node.posRow + ". Have " + node.listNode.Count + "node");
        listNode = SortMetrics(arrMetric, node);

        if (listNode.Count == 0)
        {
            Debug.Log("Nothing to back");
            listPath.SetCorrect(false);
            return listPath;
        }

        foreach (var member in listNode)
        {
            Debug.Log("???" + member.posCol + ", " + member.posRow);
        }

        for (int i = 0; i < listNode.Count; i++)
        {
            if (arrMetric[listNode[i].posCol, listNode[i].posRow].isVisited)
            {
                Debug.Log("CONTINUE " + listNode[i].posCol + ", " + listNode[i].posRow + "ISVISITED");
                continue;
            }
            if (listNode[i].posCol == target.x && listNode[i].posRow == target.y)
            {
                //add to list path 
                Debug.Log("Path:" + listNode[i].posCol + "," + listNode[i].posRow);
                listPath.InsertToPath(target);
                listPath.SetCorrect(true);
                return listPath;
            }

            //todo Problems here
            Debug.Log("F" + listNode[i].posCol + listNode[i].posRow);
            ListPath result = new ListPath();
            result = RecurTree(ref arrMetric, listNode[i], target, listPath);
            if (result.isCorrect)
            {
                Debug.Log("Path:" + listNode[i].posCol + "," + listNode[i].posRow);
                result.InsertToPath(new Vector2(listNode[i].posCol, listNode[i].posRow));
                return result;
            }
            else
            {
                Debug.Log("FALSE: " + listNode[i].posCol + listNode[i].posRow);
                arrMetric[listNode[i].posCol, listNode[i].posRow].SetVisited(true);
            }
        }
        return listPath;
    }

    private List<NodeMetrics> SortMetrics(NodeMetrics[,] arrMetric, NodeMetrics node)
    {
        Debug.Log("Sort: " + node.posCol + "," + node.posRow);
        for (int i = 0; i < node.listNode.Count; i++)
        {
            if (arrMetric[node.listNode[i].posCol, node.listNode[i].posRow].isVisited)
            {
                node.listNode.RemoveAt(i);
            }
        }

        if (node.listNode.Count == 0)
        {
            return node.listNode;
        }

        for (int i = 0; i < node.listNode.Count - 1; i++)
        {
            for (int j = 1; j < node.listNode.Count; j++)
            {
                if (node.listNode[i].metric >= node.listNode[j].metric)
                {
                    var temp = node.listNode[i];
                    node.listNode[i] = node.listNode[j];
                    node.listNode[j] = temp;
                }
            }
        }

        return node.listNode;
    }

    private void AddNode(ref List<NodeMetrics> list, NodeMetrics[,] metricArray, int col, int row) //x = col
    {
        if (col < 0 || col >= listPalete.GetLength(1) || row < 0 || row >= listPalete.GetLength(0)
            || listPaleteData[col, row].GetFill())
        {
            Debug.Log("=====>return" + col + ", " + row);
            return;
        }

        NodeMetrics node = new NodeMetrics();
        node = metricArray[col, row];
        list.Add(node);
    }
}
