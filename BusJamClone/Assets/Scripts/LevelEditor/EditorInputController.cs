using UnityEngine;

#if UNITY_EDITOR

public class EditorInputController : MonoBehaviour
{
    public LevelEditor LevelEditor;
    private Camera _mainCamera;
    private int _layerMaskGridSlot;

    private void OnEnable()
    {
        _mainCamera = Camera.main;
        _layerMaskGridSlot = 1 << LayerMask.NameToLayer("GridSlot");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out var hit, 200,
                    _layerMaskGridSlot))
            {
                if (hit.collider.TryGetComponent(out EditorGridSlot gridSlot))
                {
                    LevelEditor.OnClickOnGridSlot(gridSlot);
                }
            }
        }
    }
}
#endif