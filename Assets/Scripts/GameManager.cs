using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public BlockGenerator blockGenerator;
    public BlockPlacementManager blockPlacementManager;

    public int allowedMoves;
    public bool infiniteMoves;
    public int targetScore;

    private int _currentScore;
    private int _currentMoves;

    [System.Serializable]
    public class InitialBlockData
    {
        public int row;
        public int column;
    }

    public List<InitialBlockData> initialBlocks = new List<InitialBlockData>();

    public TMP_Text movesText;
    public TMP_Text targetScoreText;

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        _currentMoves = allowedMoves;
        _currentScore = targetScore;
        StartCoroutine(SetupLevelWithDelay());
        UpdateUI();
    }

    System.Collections.IEnumerator SetupLevelWithDelay()
    {
        while (!gridManager.IsGridReady())
        {
            yield return null;
        }
        
        SetupLevel();
        UpdateUI();
    }

    void SetupLevel()
    {
        foreach (var blockData in initialBlocks)
        {
            PlaceBlock(blockData.row, blockData.column);
        }
    }

    void PlaceBlock(int row, int column)
    {
        var block = blockGenerator.GenerateBlock();
        var blockObject = blockGenerator.InstantiateBlock(block);
        blockObject.transform.position = new Vector3(column, -row, 0);
        gridManager.logicalGrid[row, column] = block;
    }

    public void OnBlockPlaced()
    {
        if (!infiniteMoves)
        {
            _currentMoves--;
            if (_currentMoves <= 0)
            {
                HandleLoseCondition();
                return;
            }
        }

        UpdateUI();
    }

    public void OnSegmentPopped(int segmentsPopped)
    {
        _currentScore -= segmentsPopped;

        if (_currentScore <= 0)
        {
            HandleWinCondition();
        }
        else
        {
            UpdateUI();
        }
    }

    void HandleWinCondition()
    {
        LoadNextLevel();
    }

    void HandleLoseCondition()
    {
        RestartLevel();
    }

    void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateUI()
    {
        movesText.text = infiniteMoves ? "∞" : _currentMoves.ToString();

        targetScoreText.text = _currentScore.ToString();
    }
}
