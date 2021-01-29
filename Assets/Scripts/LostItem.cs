using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using utility;

public class LostItem : Multiton<LostItem>
{
    [SerializeField]
    public string _debugName = "Lost Item";

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

    private Vector3 _mouseOffset;
    private Vector3 _newPosition;
    private bool _mouseDown = false;

    private static LostItem _currentSelect = null;



    private void Start()
    {
        _newPosition = transform.position;
        Table.Instance.ConstraintMovement(this, true);
        _camera = MainCamera.Instance.GetComponent<Camera>();
    }

    private void Update()
    {
        if (_mouseDown)
        {
            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _newPosition = new Vector3(mousePosition.x + _mouseOffset.x, mousePosition.y + _mouseOffset.y, transform.position.z);

        }

        Table.Instance.ConstraintMovement(this, IsMoving);

        var lerp = Vector3.Lerp(transform.position, _newPosition, 0.05f);
        transform.position = new Vector3(lerp.x, lerp.y, transform.position.z);
    }


    private void OnMouseDown()
    {
        Debug.Log(_debugName);
        _mouseDown = true;
        var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        _mouseOffset = transform.position - mousePosition;
        _currentSelect = this;
    }

    private void OnMouseUp()
    {
        _mouseDown = false;
        _currentSelect = null;
 
    }
}
