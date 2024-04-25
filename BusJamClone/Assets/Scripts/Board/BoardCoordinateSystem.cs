using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BoardCoordinateSystem : IInitializable, IDisposable
{
    #region Zenject

    private SceneReferenceHolder _sceneReferenceHolder;
    private BoardSettings _boardSettings;
    private GridSlot.Factory _gridSlotFactory;

    [Inject]
    private void Construct(SceneReferenceHolder sceneReferenceHolder,
        BoardSettings boardSettings,
        GridSlot.Factory gridSlotFactory)
    {
        _sceneReferenceHolder = sceneReferenceHolder;
        _boardSettings = boardSettings;
        _gridSlotFactory = gridSlotFactory;
    }

    #endregion

    private GridSlot[,] _gridSlots;

    private readonly List<Vector2Int> _neighbourIncrementList = new List<Vector2Int>()
    {
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1)
    };

    public void Initialize()
    {
    }

    public void SpawnGridSlots(Level currentLevel)
    {
        if (_gridSlots != null)
        {
            for (int i = 0; i <= _gridSlots.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _gridSlots.GetUpperBound(1); j++)
                {
                    _gridSlots[i, j].GoToPool();
                }
            }
        }

        _gridSlots = new GridSlot[currentLevel.RowCount, currentLevel.ColumnCount];

        for (int i = 0; i < currentLevel.RowCount; i++)
        {
            for (int j = 0; j < currentLevel.ColumnCount; j++)
            {
                var gridSlotData = currentLevel.GridSlotDatas[i * currentLevel.ColumnCount + j];
                var gridSlot = CreateGridSlot();
                gridSlot.SetGridSlot(gridSlotData);
                gridSlot.transform.parent = _sceneReferenceHolder.BoardHolder;
                gridSlot.transform.localPosition =
                    new Vector3(j * _boardSettings.GridSlotDistance - currentLevel.ColumnCount / 2f, 0f,
                        -i * _boardSettings.GridSlotDistance);
                gridSlot.SetCoordinates(i, j);
                gridSlot.transform.name = $"Slot ({i},{j})";
                _gridSlots[i, j] = gridSlot;
                gridSlot.SetAvailability(true);
            }
        }
    }

    public void SpawnGridSlotsBySave(GridSlotSaveData gridSlotSaveData)
    {
        if (_gridSlots != null)
        {
            for (int i = 0; i <= _gridSlots.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _gridSlots.GetUpperBound(1); j++)
                {
                    _gridSlots[i, j].GoToPool();
                }
            }
        }

        _gridSlots = new GridSlot[gridSlotSaveData.RowCount, gridSlotSaveData.ColumnCount];

        for (int i = 0; i < gridSlotSaveData.RowCount; i++)
        {
            for (int j = 0; j < gridSlotSaveData.ColumnCount; j++)
            {
                var gridSlotData = gridSlotSaveData.GridSlotDatas[i * gridSlotSaveData.ColumnCount + j];
                var gridSlot = CreateGridSlot();
                gridSlot.SetGridSlot(gridSlotData);
                gridSlot.transform.parent = _sceneReferenceHolder.BoardHolder;
                gridSlot.transform.localPosition =
                    new Vector3(j * _boardSettings.GridSlotDistance - gridSlotSaveData.ColumnCount / 2f, 0f,
                        -i * _boardSettings.GridSlotDistance);
                gridSlot.SetCoordinates(i, j);
                gridSlot.transform.name = $"Slot ({i},{j})";
                _gridSlots[i, j] = gridSlot;
                gridSlot.SetAvailability(true);
            }
        }
    }

    private GridSlot CreateGridSlot()
    {
        return _gridSlotFactory.Create();
    }

    public GridSlotSaveData GetGridSlotSaveData()
    {
        GridSlotSaveData saveData = new GridSlotSaveData();
        saveData.RowCount = _gridSlots.GetUpperBound(0) + 1;
        saveData.ColumnCount = _gridSlots.GetUpperBound(1) + 1;
        saveData.GridSlotDatas = new List<GridSlotData>();

        for (int i = 0; i <= _gridSlots.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= _gridSlots.GetUpperBound(1); j++)
            {
                saveData.GridSlotDatas.Add(_gridSlots[i, j].GetSaveData());
            }
        }

        return saveData;
    }

    public void UpdateGridSlotAvailabilities()
    {
        for (int i = 0; i <= _gridSlots.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= _gridSlots.GetUpperBound(1); j++)
            {
                _gridSlots[i, j].SetAvailability(CheckForWayOut(i, j));
            }
        }
    }

    private bool CheckForWayOut(int row, int column)
    {
        if (row == 0) return true;

        List<GridSlot> visited = new List<GridSlot>();
        List<GridSlot> notVisited = new List<GridSlot>();

        int currentRow;
        int currentColumn;

        notVisited.Add(_gridSlots[row, column]);

        while (notVisited.Count > 0)
        {
            if (notVisited[0].RowIndex == 0) return true;

            for (int i = 0; i < _neighbourIncrementList.Count; i++)
            {
                currentRow = notVisited[0].RowIndex + _neighbourIncrementList[i].x;
                currentColumn = notVisited[0].ColumnIndex + _neighbourIncrementList[i].y;
                if (!IsValidCoordinate(currentRow, currentColumn)) continue;

                if (_gridSlots[currentRow, currentColumn].GetGridSlotState() != GridSlotState.Empty) continue;

                if (visited.Contains(_gridSlots[currentRow, currentColumn]) ||
                    notVisited.Contains(_gridSlots[currentRow, currentColumn])) continue;

                notVisited.Add(_gridSlots[currentRow, currentColumn]);
            }


            visited.Add(notVisited[0]);
            notVisited.RemoveAt(0);
        }

        return false;
    }

    private bool IsValidCoordinate(int row, int column)
    {
        return row >= 0 && column >= 0 && _gridSlots.GetUpperBound(0) >= row && _gridSlots.GetUpperBound(1) >= column;
    }

    public void Dispose()
    {
    }
}