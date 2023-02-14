using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float _rotationInput, _horizontalInput, _verticalInput, _scrollInput;
    [SerializeField] private float _rotationSpeed = 100;
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _width = 8;
    [SerializeField] private float _height = 8;
    [SerializeField] private List<Vector2> _zoomLevels;
    [SerializeField] private Camera _camera;
    private int _currentZoom = 3;

    public bool InMenu = true;
    void Update()
    {
        if (InMenu)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * _rotationSpeed * 0.005f);
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ResetCameraPosition();
            }
            else
            {
                //Vector2 gridDimensions = GridManager.Instance.GetGridDimensions();
                _rotationInput = Input.GetAxis("CameraRotation");
                transform.Rotate(Vector3.up, Time.deltaTime * _rotationInput * _rotationSpeed);

                if (transform.position.x < -_width / 2 + 0.5f)
                {
                    transform.position = new Vector3(-_width / 2 + 0.5f, transform.position.y, transform.position.z);
                }

                if (transform.position.x > _width / 2 - 0.5f)
                {
                    transform.position = new Vector3(_width / 2 - 0.5f, transform.position.y, transform.position.z);
                }
                _horizontalInput = Input.GetAxis("Horizontal");
                transform.Translate(transform.right * Time.deltaTime * _horizontalInput * _movementSpeed, Space.World);

                if (transform.position.z < -_height / 2 + 0.5f)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, -_height / 2 + 0.5f);
                }

                if (transform.position.z > _height / 2 - 0.5f)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, _height / 2 - 0.5f);
                }
                _verticalInput = Input.GetAxis("Vertical");
                transform.Translate(transform.forward * Time.deltaTime * _verticalInput * _movementSpeed, Space.World);

                _scrollInput = Input.GetAxis("Mouse ScrollWheel");
                if (_scrollInput != 0)
                {
                    SetCameraZoom((int)(_scrollInput * -10));
                }
            }
        }
    }

    public void ResetCameraPosition()
    {
        //Vector2 gridDimensions = GridManager.Instance.GetGridDimensions();
        transform.position = new Vector3(0, 0, 0);//new Vector3((float)gridDimensions.x / 2 - 0.5f, 0, (float)gridDimensions.y / 2 - 0.5f);
        transform.rotation = Quaternion.identity;
    }

    private void SetCameraZoom(int zoomDirection)
    {
        int _zoomLevelsCount = _zoomLevels.Count;
        _currentZoom = (0 <= _currentZoom + zoomDirection && _currentZoom + zoomDirection < _zoomLevelsCount) ? _currentZoom + zoomDirection : _currentZoom;
        Vector2 zoom = _zoomLevels[_currentZoom];
        _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, zoom.x, zoom.y);
    }
}
