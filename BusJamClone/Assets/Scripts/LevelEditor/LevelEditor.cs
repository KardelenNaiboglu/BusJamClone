using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class LevelEditor : MonoBehaviour
{
    public TMP_InputField TimerInputField;
    public TMP_InputField RowCountInputField;
    public TMP_InputField ColumnCountInputField;
    public TMP_Dropdown ExistingLevelsDropDown;
    public TMP_Dropdown ExistingPassengersDropDown;
    public TMP_Dropdown GridSlotStatusDropDown;

    public Transform GridSlotParent;
    public EditorPassenger prefabPassenger;
    public EditorGridSlot prefabEditorGridSlot;
    public BoardSettings BoardSettings;
    public PassengerSettings PassengerSettings;

    public TMP_Text EditModeSwitch;

    private List<EditorGridSlot> _editorGridSlots = new();
    private Level currentLevel;
    private LevelEditMode _levelEditMode;
    private bool _editingExistingLevel = false;

    private void OnEnable()
    {
        UpdateDropdownValues();
    }

    #region Button

    public void OnButtonClickGenerateLevel()
    {
        if (!int.TryParse(TimerInputField.text, out var timer)) return;
        if (!int.TryParse(RowCountInputField.text, out var rowCount)) return;
        if (!int.TryParse(ColumnCountInputField.text, out var columnCount)) return;
        ClearObjects();
        _editingExistingLevel = false;
        currentLevel = ScriptableObject.CreateInstance<Level>();
        currentLevel.RowCount = rowCount;
        currentLevel.ColumnCount = columnCount;
        currentLevel.TimeInSeconds = timer;
        currentLevel.GridSlotDatas = new List<GridSlotData>();
        currentLevel.BusDatas = new List<BusData>();

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                var gridSlot = Instantiate(prefabEditorGridSlot, GridSlotParent);
                gridSlot.transform.localPosition =
                    new Vector3(j * BoardSettings.GridSlotDistance - columnCount / 2, 0f,
                        -i * BoardSettings.GridSlotDistance);
                gridSlot.SetCoordinates(i, j);
                _editorGridSlots.Add(gridSlot);
            }
        }
    }

    public void OnButtonClickSaveLevel()
    {
        if (_editingExistingLevel)
        {
            if (selectedLevel == null) return;
            selectedLevel.GridSlotDatas = new();
            foreach (var editorGridSlot in _editorGridSlots)
            {
                selectedLevel.GridSlotDatas.Add(editorGridSlot.GetGridSlotData());
            }

            EditorUtility.SetDirty(selectedLevel);
            AssetDatabase.SaveAssets();
        }
        else
        {
            if (currentLevel == null) return;

            currentLevel.GridSlotDatas = new();
            foreach (var editorGridSlot in _editorGridSlots)
            {
                currentLevel.GridSlotDatas.Add(editorGridSlot.GetGridSlotData());
            }

            var allLevels = Resources.LoadAll<Level>("Levels");
            AssetDatabase.CreateAsset(currentLevel, $"Assets/Resources/Levels/Level {allLevels.Length + 1}.asset");
        }


        AssetDatabase.Refresh();
        selectedLevel = null;
        currentLevel = null;
        UpdateDropdownValues();
    }

    private Level selectedLevel;

    public void OnButtonClickShowSelectedLevel()
    {
        ClearObjects();
        _editingExistingLevel = true;
        var allLevels = Resources.LoadAll<Level>("Levels");
        selectedLevel = allLevels[ExistingLevelsDropDown.value];
        EditorUtility.SetDirty(selectedLevel);
        CreateExistingLevel();
    }

    public void OnButtonClickEditMode()
    {
        _levelEditMode = _levelEditMode == LevelEditMode.Grid ? LevelEditMode.Passenger : LevelEditMode.Grid;
        EditModeSwitch.text = _levelEditMode == LevelEditMode.Grid ? "Edit Mode Grid" : "Edit Mode Passenger";
    }

    #endregion

    private void CreateExistingLevel()
    {
        for (int i = 0; i < selectedLevel.RowCount; i++)
        {
            for (int j = 0; j < selectedLevel.ColumnCount; j++)
            {
                var gridSlot = Instantiate(prefabEditorGridSlot, GridSlotParent);
                gridSlot.transform.localPosition =
                    new Vector3(j * BoardSettings.GridSlotDistance - selectedLevel.ColumnCount / 2, 0f,
                        -i * BoardSettings.GridSlotDistance);
                gridSlot.SetCoordinates(i, j);
                gridSlot.SetGridSlotState(selectedLevel.GridSlotDatas[i * selectedLevel.ColumnCount + j].GridSlotState);
                if (selectedLevel.GridSlotDatas[i * selectedLevel.ColumnCount + j].GridSlotState ==
                    GridSlotState.Occupied)
                {
                    gridSlot.SetPassenger(CreatePassenger((int)selectedLevel
                        .GridSlotDatas[i * selectedLevel.ColumnCount + j].PassengerType));
                }

                _editorGridSlots.Add(gridSlot);
            }
        }
    }

    private void UpdateDropdownValues()
    {
        var allLevels = Resources.LoadAll<Level>("Levels");
        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < allLevels.Length; i++)
        {
            var optionData = new TMP_Dropdown.OptionData();
            optionData.text = allLevels[i].name;
            optionDatas.Add(optionData);
        }

        ExistingPassengersDropDown.ClearOptions();
        ExistingLevelsDropDown.AddOptions(optionDatas);
        optionDatas.Clear();

        for (int i = 0; i < PassengerSettings.PassengerDatas.Count; i++)
        {
            var optionData = new TMP_Dropdown.OptionData();
            optionData.text = Enum.GetName(typeof(PassengerType), PassengerSettings.PassengerDatas[i].PassengerType);
            optionDatas.Add(optionData);
        }

        ExistingPassengersDropDown.ClearOptions();
        ExistingPassengersDropDown.AddOptions(optionDatas);
        optionDatas.Clear();

        for (int i = 0; i < 3; i++)
        {
            var optionData = new TMP_Dropdown.OptionData();
            optionData.text = Enum.GetName(typeof(GridSlotState), i);
            optionDatas.Add(optionData);
        }

        GridSlotStatusDropDown.ClearOptions();
        GridSlotStatusDropDown.AddOptions(optionDatas);
    }

    private void ClearObjects()
    {
        foreach (var gridSlot in _editorGridSlots)
        {
            Destroy(gridSlot.gameObject);
        }

        _editorGridSlots.Clear();
    }

    public void OnClickOnGridSlot(EditorGridSlot gridSlot)
    {
        switch (_levelEditMode)
        {
            case LevelEditMode.Grid:
                gridSlot.SetGridSlotState((GridSlotState)GridSlotStatusDropDown.value);
                break;

            case LevelEditMode.Passenger:
                gridSlot.SetPassenger(CreatePassenger(ExistingPassengersDropDown.value));
                break;

            default: break;
        }
    }

    private EditorPassenger CreatePassenger(int value)
    {
        var createdPassenger = Instantiate(prefabPassenger);

        createdPassenger.SetPassengerTypeAndColor(PassengerSettings.GetPassengerDataByType((PassengerType)value));
        return createdPassenger;
    }
}

public enum LevelEditMode
{
    Grid,
    Passenger
}

#endif