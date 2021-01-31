using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using utility;

public class LostItem : Multiton<LostItem>
{
    [SerializeField]
    public string _debugName = "Lost Item";

    [SerializeField]
    private float _lerpFactorSelected;

    private Camera _camera;

    public bool IsMoving
    {
        get
        {
            var offset = _newPosition - transform.position;
            var offset2 = new Vector2(offset.x, offset.y);
            return offset2.magnitude > 0.01f;
        }
    }


    public bool IsSelected
    {
        get => _mouseDown;
    }


    public LostItem CurrentSelect
    {
        get => _currentSelect;
    }


    public Vector3 NewPosition
    {
        get => _newPosition;
        set
        {
            _newPosition = value;
        }
    }


    public float LerpFactor
    {
        get => _lerpFactor;
        set
        {
            _lerpFactor = value;
        }
    }

    private Vector3 _mouseOffset;
    private Vector3 _newPosition;
    private Vector3 _velocity;
    private bool _mouseDown = false;

    private static LostItem _currentSelect = null;


    private float _lerpFactor;


    private void Start()
    {
        if (_newPosition == Vector3.zero) { _newPosition = transform.position; }
        Table.Instance.ConstraintMovement(this, true);
        _camera = MainCamera.Instance.GetComponent<Camera>();

        _lerpFactor = _lerpFactorSelected;
    }

    private void Update()
    {
        Debug.Log("pos: " + _newPosition);
        if (_mouseDown)
        {
            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _newPosition = new Vector3(mousePosition.x + _mouseOffset.x, mousePosition.y + _mouseOffset.y, transform.position.z);
            _velocity = Vector3.zero;
        }

        Table.Instance.ConstraintMovement(this, true);

        var lerp = Vector3.Lerp(transform.position, _newPosition, _lerpFactor);
        if (!_mouseDown)
        {
           // if (_velocity == Vector3.zero)
               // _newPosition = (lerp - transform.position) * 3;
        }
        transform.position = new Vector3(lerp.x, lerp.y, transform.position.z);
    }


    private void OnMouseDown()
    {
        Debug.Log(_debugName);
        _mouseDown = true;
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        _mouseOffset = transform.position - mousePosition;
        _lerpFactor = _lerpFactorSelected;

        _currentSelect = this;
    }

    private void OnMouseUp()
    {
        _mouseDown = false;
        _currentSelect = null;
        _newPosition += (Vector3.Lerp(transform.position, _newPosition, 0.05f) - transform.position) * 15;
    }


    private void OnDestroy()
    {
        ItemSpawner.Instance.PoolItem(this);
    }
}
