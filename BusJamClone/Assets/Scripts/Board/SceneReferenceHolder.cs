using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneReferenceHolder : MonoBehaviour
{
    #region Zenject

    private SaveController _saveController;

    [Inject]
    private void Construct(SaveController saveController)
    {
        _saveController = saveController;
    }

    #endregion

    #region TransformReferences

    public Transform BoardHolder;
    public Transform BusHolder;
    public Transform QueueHolder;

    #endregion


    private void OnApplicationQuit()
    {
        _saveController.SaveCurrentGame();
    }
}