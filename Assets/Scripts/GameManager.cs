using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour
{
    public Transform cameraCrane;
    public RotateGizmoController rotateGizmoControllerPrefab;

    void Start()
    {
        m_puzzle = FindObjectOfType<PuzzleController>();
        m_puzzle.generateBoards();
        m_baseBoardBounds = new Bounds[m_puzzle.boards.Count];
        for (int i = 0; i < m_puzzle.boards.Count; ++i)
        {
            m_baseBoardBounds[i] = m_puzzle.boards[i].renderer.bounds;
        }

        m_rotateGizmo = GameObject.Instantiate(rotateGizmoControllerPrefab, Vector3.zero, Quaternion.identity);
        m_rotateGizmo.gameObject.SetActive(false);
        m_rotateGizmo.leftArrowClicked += OnLeftArrowClicked;
        m_rotateGizmo.rightArrowClicked += OnRightArrowClicked;

        SetupCamera();
    }

    void Update()
    {
        //SetupCamera();

        /*Color c = new Color(0, 0, 1, 0.5f);
        foreach (Bounds bounds in m_baseBoardBounds)
        {
            Vector3 boundsMin = bounds.min;
            Vector3 boundsMax = bounds.max;
            float y = boundsMin.y + boundsMax.y * .5f;

            Vector3 min = new Vector3(boundsMax.x, y, boundsMin.z);
            Vector3 max = new Vector3(boundsMin.x, y, boundsMax.z);

            Debug.DrawLine(min, min + Vector3.up * 5f, c, 0f, false);
            Debug.DrawLine(max, max + Vector3.up * 5f, c, 0f, false);
        }*/

        // INPUT
        if (Input.GetMouseButtonUp(0))
        {
            if (m_grabbedBoardIndex >= 0)
            {
                BoardController grabbedBoard = m_puzzle.boards[m_grabbedBoardIndex];
                grabbedBoard.grabbed = false;
                m_grabbedBoardIndex = -1;
                m_puzzle.UpdateBoardPositions();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Assert.IsTrue(m_grabbedBoardIndex < 0);
            

            RaycastHit hit = new RaycastHit();
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = LayerMask.GetMask("Board");
            if (Physics.Raycast(ray, out hit, 10000f, mask))
            {
                BoardController board = hit.collider.gameObject.GetComponent<BoardController>();
                board.grabbed = true;
                m_grabbedBoardIndex = GetBoardIndex(board);
                Assert.IsTrue(m_grabbedBoardIndex >= 0);
            }
        }

        // FIND HOVERED COLUMN
        m_currentHoveredColumn = -1;
        for (int i = 0; i < m_baseBoardBounds.Length; ++i)
        {
            float xMin, xMax;
            GetBoardXScreenBounds(m_baseBoardBounds[i], out xMin, out xMax);

            if (Input.mousePosition.x >= xMin && Input.mousePosition.x <= xMax)
            {
                m_currentHoveredColumn = i;
                break;
            }
        }

        // BOARD DRAG
        if (m_grabbedBoardIndex >= 0)
        {
            // DRAG POSITION
            Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(new Vector3(0f, 0f, -1f), Vector3.zero);
            float enter = 0f;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 position = m_puzzle.transform.position;
                position.x = hitPoint.x;
                m_puzzle.boards[m_grabbedBoardIndex].transform.position = position;
            }

            // BOARD SWAP
            if (m_currentHoveredColumn >= 0 && m_currentHoveredColumn != m_grabbedBoardIndex)
            {
                m_puzzle.SwapBoards(m_grabbedBoardIndex, m_currentHoveredColumn);
                m_grabbedBoardIndex = m_currentHoveredColumn;
            }
        }

        // ROTATION GIZMO
        if (m_grabbedBoardIndex < 0 && m_currentHoveredColumn >= 0)
        {
            m_rotateGizmo.gameObject.SetActive(true);
            m_rotateGizmo.transform.position = m_puzzle.boards[m_currentHoveredColumn].transform.position;
        }
        else
        {
            m_rotateGizmo.gameObject.SetActive(false);
        }
    }

    void GetBoardXScreenBounds(Bounds _bounds, out float _min, out float _max)
    {
        Vector3 boundsMin = _bounds.min;
        Vector3 boundsMax = _bounds.max;
        float y = boundsMin.y + boundsMax.y * .5f;

        Vector3 min = new Vector3(boundsMax.x, y, boundsMin.z);
        Vector3 max = new Vector3(boundsMin.x, y, boundsMax.z);

        Vector2 minScreen = m_camera.WorldToScreenPoint(min);
        Vector2 maxScreen = m_camera.WorldToScreenPoint(max);

        _min = Mathf.Min(minScreen.x, maxScreen.x);
        _max = Mathf.Max(minScreen.x, maxScreen.x);
    }

    int GetBoardIndex(BoardController _board)
    {
        for (int i = 0; i < m_puzzle.boards.Count; ++i)
        {
            if (_board == m_puzzle.boards[i])
                return i;
        }
        return -1;
    }

    void SetupCamera()
    {
        m_camera = Camera.main;
        Vector3 firstBoardPosition = m_puzzle.boards[0].transform.position;
        Vector3 lastBoardPosition = m_puzzle.boards[m_puzzle.boards.Count - 1].transform.position;
        Vector3 cameraReferencePoint = (firstBoardPosition + lastBoardPosition) * .5f;
        cameraCrane.transform.position = cameraReferencePoint;
    }

    void OnLeftArrowClicked()
    {
        if (m_currentHoveredColumn < 0)
            return;

        m_puzzle.boards[m_currentHoveredColumn].rotate(1);
    }

    void OnRightArrowClicked()
    {
        if (m_currentHoveredColumn < 0)
            return;

        m_puzzle.boards[m_currentHoveredColumn].rotate(-1);
    }

    Camera m_camera;
    PuzzleController m_puzzle;
    int m_grabbedBoardIndex = -1;
    int m_currentHoveredColumn = -1;
    Bounds[] m_baseBoardBounds;
    RotateGizmoController m_rotateGizmo;
}
