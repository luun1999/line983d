using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaleteController : MonoBehaviour
{
    private int posCol;
    private int posRow;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetPos(int col, int row)
    {
        this.posCol = col;
        this.posRow = row;
    }
    public void OnClick()
    {
        gameManager.ClickAPositon(this.posCol, this.posRow);
    }
}
