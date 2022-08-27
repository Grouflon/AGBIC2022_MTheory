using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class PuzzleController : MonoBehaviour
{
    public BoardDefinition[] boardDefinitions;
    public BoardController boardPrefab;
    public float boardInterval = 4f;
    public Color backgroundColor;

    public List<BoardController> boards
    {
        get { return m_boards; }
    }

    public void generateBoards()
    {
        BoardController[] previousBoards = GetComponentsInChildren<BoardController>(true);
        foreach (BoardController board in previousBoards)
        {
            if (board == null)
                continue;

            DestroyImmediate(board.gameObject);
        }

        m_dirty = false;
        m_boards = new List<BoardController>();

        if (boardDefinitions == null)
            return;

        for (int i = 0; i < boardDefinitions.Length; ++i)
        {
            BoardDefinition boardDefinition = boardDefinitions[i];
            BoardController board = GameObject.Instantiate(boardPrefab, transform);

            board.setData(boardDefinition, backgroundColor);
            m_boards.Add(board);
        }

        UpdateBoardPositions();
    }

    private List<BoardController> m_boards;
    bool m_dirty = true;

    void Start()
    {
    }

    void Update()
    {
        if (m_dirty)
        {
            generateBoards();
        }
    }

    public void UpdateBoardPositions()
    {
        for (int i = 0; i < m_boards.Count; ++i)
        {
            BoardController board = m_boards[i];
            if (board.grabbed)
                continue;

            board.transform.localPosition = new Vector3(i * boardInterval, 0f, 0f);
        }
    }

    public void SwapBoards(int _a, int _b)
    {
        Assert.IsTrue(_a >= 0 && _a < m_boards.Count);
        Assert.IsTrue(_b >= 0 && _b < m_boards.Count);
        BoardController temp = m_boards[_a];
        m_boards[_a] = m_boards[_b];
        m_boards[_b] = temp;

        UpdateBoardPositions();
    }

    void OnValidate()
    {
        m_dirty = true;
    }

}
