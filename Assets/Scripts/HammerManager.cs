using TMPro;
using UnityEngine;

public class HammerManager : MonoBehaviour
{
    public GridManager gridManager;
    public PoppingSystem poppingSystem;
    public int hammerCount = 3;
    public GameObject hammerButton;
    public TMP_Text hammerCountText;

    private bool _isHammerModeActive;

    void Start()
    {
        UpdateHammerButtonText();
    }

    void Update()
    {
        if (_isHammerModeActive && Input.GetMouseButtonDown(0))
        {
            HandleHammerClick();
        }
    }

    public void ToggleHammerMode()
    {
        _isHammerModeActive = !_isHammerModeActive;
    }

    public bool IsHammerModeActive()
    {
        return _isHammerModeActive;
    }

    private void HandleHammerClick()
    {
        if (hammerCount <= 0)
        {
            Debug.Log("No hammers left!");
            _isHammerModeActive = false;
            return;
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedBlock = hit.collider.gameObject;

            // Find the block in the grid
            for (int x = 0; x < gridManager.logicalGridSize; x++)
            {
                for (int y = 0; y < gridManager.logicalGridSize; y++)
                {
                    if (gridManager.logicalGrid[y, x]?.blockObject == clickedBlock)
                    {
                        poppingSystem.RemoveBlockFromGrid(gridManager.logicalGrid[y,x]);
                        poppingSystem.CheckAndPopSegments(y,x);
                        hammerCount--;
                        UpdateHammerButtonText();
                        _isHammerModeActive = false;
                        return;
                    }
                }
            }
        }
        else
        {
            _isHammerModeActive = false;
        }
    }

    private void UpdateHammerButtonText()
    {
        if (hammerButton != null)
        {
            hammerCountText.text = $"Hammers: {hammerCount}";
        }
    }
}
