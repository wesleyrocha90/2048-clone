﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    GameOver,
    WaitingForMoveToEnd
}

public class GameManager : MonoBehaviour
{
    public GameState State;
    [Range(0, 2f)]
    public float delay;
    private bool moveMade;
    private bool[] lineMoveComplete = new bool[4]{ true, true, true, true };
    public GameObject GameOverPanel;
    public GameObject YouWonText;
    public GameObject GameOverText;
    public Text GameOverScoreText;
    private Tile[,] AllTiles = new Tile[4,4];
    private List<Tile[]> columns = new List<Tile[]>();
    private List<Tile[]> rows = new List<Tile[]>();
    private List<Tile> EmptyTiles = new List<Tile>();

    void Start()
    {
        Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile>();
        foreach (Tile tile in AllTilesOneDim)
        {
            tile.Number = 0;
            AllTiles[tile.indexRow, tile.indexColumn] = tile;
            EmptyTiles.Add(tile);
        }

        columns.Add(new Tile[]{ AllTiles[0, 0], AllTiles[1, 0], AllTiles[2 ,0], AllTiles[3, 0] });
        columns.Add(new Tile[]{ AllTiles[0, 1], AllTiles[1, 1], AllTiles[2 ,1], AllTiles[3, 1] });
        columns.Add(new Tile[]{ AllTiles[0, 2], AllTiles[1, 2], AllTiles[2, 2], AllTiles[3, 2] });
        columns.Add(new Tile[]{ AllTiles[0, 3], AllTiles[1, 3], AllTiles[2, 3], AllTiles[3, 3] });

        rows.Add(new Tile[]{ AllTiles[0, 0], AllTiles[0, 1], AllTiles[0, 2], AllTiles[0, 3] });
        rows.Add(new Tile[]{ AllTiles[1, 0], AllTiles[1, 1], AllTiles[1, 2], AllTiles[1, 3] });
        rows.Add(new Tile[]{ AllTiles[2, 0], AllTiles[2, 1], AllTiles[2, 2], AllTiles[2, 3] });
        rows.Add(new Tile[]{ AllTiles[3, 0], AllTiles[3, 1], AllTiles[3, 2], AllTiles[3, 3] });

        Generate();
        Generate();
    }

    private void YouWon()
    {
        GameOverText.SetActive(false);
        YouWonText.SetActive(true);
        GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
        GameOverPanel.SetActive(true);
    }
    private void GameOver()
    {
        GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
        GameOverPanel.SetActive(true);
    }

    bool CanMove()
    {
        if (EmptyTiles.Count > 0)
            return true;
        else
        {
            // Check columns
            for (int i = 0; i < columns.Count; i++)
                for (int j = 0; j < rows.Count - 1; j++)
                    if (AllTiles[j, i].Number == AllTiles[j + 1, i].Number)
                        return true;
            // Check rows
            for (int i = 0; i < rows.Count; i++)
                for (int j = 0; j < columns.Count - 1; j++)
                    if (AllTiles[i, j].Number == AllTiles[i, j + 1].Number)
                        return true;
        }
        return false;
    }

    public void NewGameButtonHandler()
    {
        // Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    bool MakeOneMoveDownIndex(Tile[] LineOfTiles)
    {
        for (int i = 0; i < LineOfTiles.Length - 1; i++)
        {
            // MOVE BLOCK
            if (LineOfTiles[i].Number == 0 && LineOfTiles[i + 1].Number != 0)
            {
                LineOfTiles[i].Number = LineOfTiles[i + 1].Number;
                LineOfTiles[i + 1].Number = 0;
                return true;
            }
            // MERGED BLOCK
            if (LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i + 1].Number &&
                LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i + 1].mergedThisTurn == false)
                {
                    LineOfTiles[i].Number *= 2;
                    LineOfTiles[i + 1].Number = 0;
                    LineOfTiles[i].mergedThisTurn = true;
                    LineOfTiles[i].PlayMergeAnimation();
                    ScoreTracker.Instance.Score += LineOfTiles[i].Number;
                    if (LineOfTiles[i].Number == 2048)
                        YouWon();
                    return true;
                }
        }
        return false;
    }

    bool MakeOneMoveUpIndex(Tile[] LineOfTiles)
    {
        for (int i = LineOfTiles.Length - 1; i > 0; i--)
        {
            // MOVE BLOCK
            if (LineOfTiles[i].Number == 0 && LineOfTiles[i - 1].Number != 0)
            {
                LineOfTiles[i].Number = LineOfTiles[i - 1].Number;
                LineOfTiles[i - 1].Number = 0;
                return true;
            }
            // MERGED BLOCK
            if (LineOfTiles[i].Number != 0 && LineOfTiles[i].Number == LineOfTiles[i - 1].Number &&
                LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i - 1].mergedThisTurn == false)
                {
                    LineOfTiles[i].Number *= 2;
                    LineOfTiles[i - 1].Number = 0;
                    LineOfTiles[i].mergedThisTurn = true;
                    LineOfTiles[i].PlayMergeAnimation();
                    ScoreTracker.Instance.Score += LineOfTiles[i].Number;
                    if (LineOfTiles[i].Number == 2048)
                        YouWon();
                    return true;
                }
        }
        return false;
    }

    void Generate()
    {
        if (EmptyTiles.Count > 0)
        {
            int indexForNewNumber = Random.Range(0, EmptyTiles.Count);
            int randomNum = Random.Range(0, 10);
            if (randomNum == 0)
                EmptyTiles[indexForNewNumber].Number = 4;
            else
                EmptyTiles[indexForNewNumber].Number = 2;

            EmptyTiles[indexForNewNumber].PlayAppearAnimation();
            EmptyTiles.RemoveAt(indexForNewNumber);
        }
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Generate();
        }
    }*/

    private void ResetMergedFlags()
    {
        foreach (Tile tile in AllTiles)
            tile.mergedThisTurn = false;
    }

    private void UpdateEmptyTiles()
    {
        EmptyTiles.Clear();
        foreach (Tile tile in AllTiles)
        {
            if (tile.Number == 0)
                EmptyTiles.Add(tile);
        }
    }

    public void Move(MoveDirection moveDirection)
    {
        bool moveMade = false;
        ResetMergedFlags();
        if (delay > 0)
            StartCoroutine(MoveCoroutine(moveDirection));

        for (int i = 0; i < rows.Count; i++)
        {
            switch(moveDirection)
            {
            case MoveDirection.Down:
                while(MakeOneMoveUpIndex(columns[i])) {
                    moveMade = true;
                }
                break;
            case MoveDirection.Left:
                while(MakeOneMoveDownIndex(rows[i])) {
                    moveMade = true;
                }
                break;
            case MoveDirection.Right:
                while(MakeOneMoveUpIndex(rows[i])) {
                    moveMade = true;
                }
                break;
            case MoveDirection.Up:
                while(MakeOneMoveDownIndex(columns[i])) {
                    moveMade = true;
                }
                break;
            }
        }

        if (moveMade)
        {
            UpdateEmptyTiles();
            Generate();

            if(!CanMove())
                GameOver();
        }
    }

    IEnumerator MoveOneLineUpIndexCoroutine(Tile[] line, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveUpIndex(line))
        {
            moveMade = true;
            yield return new WaitForSeconds(delay);
        }
        lineMoveComplete[index] = true;
    }

    IEnumerator MoveOneLineDownIndexCoroutine(Tile[] line, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveDownIndex(line))
        {
            moveMade = true;
            yield return new WaitForSeconds(delay);
        }
        lineMoveComplete[index] = true;
    }

    IEnumerator MoveCoroutine(MoveDirection moveDirection)
    {
        State = GameState.WaitingForMoveToEnd;
        switch (moveDirection)
        {
            case MoveDirection.Down:
                for (int i = 0; i < columns.Count; i++)
                    StartCoroutine(MoveOneLineUpIndexCoroutine(columns[i], i));
                break;
            case MoveDirection.Left:
                for (int i = 0; i < rows.Count; i++)
                    StartCoroutine(MoveOneLineDownIndexCoroutine(rows[i], i));
                break;
            case MoveDirection.Right:
                for (int i = 0; i < rows.Count; i++)
                    StartCoroutine(MoveOneLineUpIndexCoroutine(rows[i], i));
                break;
            case MoveDirection.Up:
                for (int i = 0; i < columns.Count; i++)
                    StartCoroutine(MoveOneLineDownIndexCoroutine(columns[i], i));
                break;
        }

        while (!(lineMoveComplete[0] && lineMoveComplete[1] && lineMoveComplete[2] && lineMoveComplete[3]))
            yield return null;

        if (moveMade)
        {
            UpdateEmptyTiles();
            Generate();
            if (!CanMove())
                GameOver();
        }

        State = GameState.Playing;
        StopAllCoroutines();
    }
}
