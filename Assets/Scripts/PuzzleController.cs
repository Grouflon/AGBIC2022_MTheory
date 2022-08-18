using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PuzzleController : MonoBehaviour
{
    public BoardDefinition[] boards;
    public BoardController boardPrefab;
    public float boardInterval = 4f;
    public Color backgroundColor;

    public void generateBoards()
    {
        BoardController[] previousBoards = GetComponentsInChildren<BoardController>(true);
        foreach (BoardController board in previousBoards)
        {
            if (board == null)
                continue;

            DestroyImmediate(board.gameObject);
        }

        m_boards = new List<BoardController>();
        for (int i = 0; i < boards.Length; ++i)
        {
            BoardDefinition boardDefinition = boards[i];
            BoardController board = GameObject.Instantiate(boardPrefab, transform);
            board.transform.localPosition = new Vector3(i * boardInterval, 0f, 0f);

            board.setData(boardDefinition, backgroundColor);

            m_boards.Add(board);
        }

        m_dirty = false;
    }

    private List<BoardController> m_boards;
    bool m_dirty = true;

    void Start()
    {
        generateBoards();
    }

    void Update()
    {
        if (m_dirty)
        {
            generateBoards();
        }
    }

    void OnValidate()
    {
        m_dirty = true;
    }

}
