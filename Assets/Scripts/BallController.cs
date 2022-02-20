using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private int posCol;
    private int posRow;
    [SerializeField] private bool isSmall;
    public int PosCol { get => posCol; set => posCol = value; }
    public int PosRow { get => posRow; set => posRow = value; }
    public bool IsSmall { get => isSmall; set => isSmall = value; }

    public void SetPos(int posCol, int posRow)
    {
        this.posCol = posCol;
        this.posRow = posRow;
    }

    // Start is called before the first frame update
    void Start()
    {
        posCol = 0;
        posRow = 0;
        isSmall = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
