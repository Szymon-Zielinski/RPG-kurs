using System;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Camera _mainCamera;
    private float _lastCameraPositionX;
    private float _cameraHalfWidth;
    
    [SerializeField] private ParallaxLayer[] backgroundLayers;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _cameraHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        CalculateBackgroundWirth();
    }

    private void FixedUpdate()
    {
        float cameraPositionX = _mainCamera.transform.position.x;
        float distanceToMove = cameraPositionX - _lastCameraPositionX;
        _lastCameraPositionX = cameraPositionX;

        float cameraLeftEdge = cameraPositionX - _cameraHalfWidth;
        float cameraRightEdge = cameraPositionX + _cameraHalfWidth;

        foreach (ParallaxLayer layer in backgroundLayers)
        {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    private void CalculateBackgroundWirth()
    {
        foreach (ParallaxLayer layer in backgroundLayers)
            layer.CalcualteImageWidth();
    }
}
