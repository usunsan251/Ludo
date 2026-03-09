using UnityEngine;
using Unity.Cinemachine;

public class VirtualCameraSelector : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] _virtualCameraList;
    [SerializeField] private int _unselectedPriority = 0;
    [SerializeField] private int _selectedPriority = 10;
    public int _currentCamera;
    public static VirtualCameraSelector Instance;

    private void Awake()
    {
        Instance = this;
        if(_virtualCameraList == null || _virtualCameraList.Length <= 0)
        {
            return;
        }

        for(var i = 0; i < _virtualCameraList.Length; i++)
        {
            _virtualCameraList[i].Priority = (i == _currentCamera ? _selectedPriority : _unselectedPriority);
        }
    }

    public void SetCurrentCamera(int player)
    {
        _currentCamera = player;
        var vCamFirst = _virtualCameraList[_currentCamera];
        vCamFirst.Priority = _selectedPriority;
    }


    public void VcamChange()
    {
        if (_virtualCameraList == null || _virtualCameraList.Length <= 0)
        {
            return;
        }
        
        var vCamPrev = _virtualCameraList[_currentCamera];
        vCamPrev.Priority = _unselectedPriority;
        if(++_currentCamera >= _virtualCameraList.Length)
            {
                _currentCamera = 0;
            }

        var vCamCurrent = _virtualCameraList[_currentCamera];
        vCamCurrent.Priority = _selectedPriority;
        
    }
}
