using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAreaController : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        gameManager.GeneratePaleteGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
