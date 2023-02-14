using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private List<Image> _healthbarOrbs = new List<Image>();
    [SerializeField] private GameObject _container;
    [SerializeField] private Image _orbPrefab;
    [SerializeField] private bool _alwaysFaceCamera = true;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        for (int i = 0; i < _unit.MaxHP; i++)
        {
            var orb = Instantiate(_orbPrefab, _container.transform);
            _healthbarOrbs.Add(orb);
        }
    }

    void Update()
    {
        UpdateHealthBar();
        if (_alwaysFaceCamera)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(transform.position.x, 0, transform.position.z) - new Vector3(_cam.transform.position.x, _cam.transform.position.y, _cam.transform.position.z));
        }
    }
    public void UpdateHealthBar()
    {
        for (int i = 0; i < _unit.MaxHP; i++)
        {
            if (i >= _unit.HP)
            {
                _healthbarOrbs[i].gameObject.SetActive(false);
            }
        }
    }

}
