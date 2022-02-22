using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaleteDataNS;

public class BallController : MonoBehaviour
{
    private int posCol;
    private int posRow;
    private SpriteRenderer sprite;
    public Animator anim;
    public bool isClick = false;
    [SerializeField] private bool isSmall;
    [SerializeField] private BallColor color;
    public int PosCol { get => posCol; set => posCol = value; }
    public int PosRow { get => posRow; set => posRow = value; }
    public bool IsSmall { get => isSmall; set => isSmall = value; }

    public void SetPos(int posCol, int posRow)
    {
        this.posCol = posCol;
        this.posRow = posRow;
    }

    public void SetColor(BallColor c)
    {
        color = c;
    }
    public BallColor GetColor()
    {
        return color;
    }

    public Color GetRGBColor()
    {
        switch (color)
        {
            case BallColor.RED:
                return Color.red;
            case BallColor.GREEN:
                return Color.green;
            case BallColor.BLUE:
                return Color.blue;
            case BallColor.CYAN:
                return Color.cyan;
            case BallColor.MAGENTA:
                return Color.magenta;
            case BallColor.YELLOW:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    public void UpdateColor()
    {
        switch (color)
        {
            case BallColor.RED:
                sprite.color = Color.red;
                break;
            case BallColor.GREEN:
                sprite.color = Color.green;
                break;
            case BallColor.BLUE:
                sprite.color = Color.blue;
                break;
            case BallColor.CYAN:
                sprite.color = Color.cyan;
                break;
            case BallColor.MAGENTA:
                sprite.color = Color.magenta;
                break;
            case BallColor.YELLOW:
                sprite.color = Color.yellow;
                break;
            default:
                sprite.color = Color.white;
                break;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        posCol = 0;
        posRow = 0;
        isSmall = true;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isClick", isClick);
    }
}
