using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleController))]
public class PuzzleControllerEditor : Editor
{
    PuzzleController puzzle;

    void OnEnable()
    {
        puzzle = target as PuzzleController;
        //puzzle.generateBoards();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        /*if (GUILayout.Button("Update Puzzle"))
        {
            puzzle.generateBoards();
        }*/
    }
}
