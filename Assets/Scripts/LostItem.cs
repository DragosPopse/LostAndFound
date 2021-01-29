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


    private List<GameObject> _objectsOnTop = new List<GameObject>();

    private void Start()
    {
        _newPosition = transform.position;
        _camera = MainCamera.Instance.GetComponent<Camera>();
    }

    private void Update()
    {
        if (_mouseDown)
        {
            var mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _newPosition = new Vector3(mousePosition.x + _mouseOffset.x, mousePosition.y + _mouseOffset.y, transform.position.z);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (var obj in _objectsOnTop)
                {
                    Debug.Log(obj.GetComponent<LostItem>()._debugName);
                }
            }
        }

        Table.Instance.ConstraintMovement(this);

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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Enter: " + _debugName + " " + collision.gameObject.GetComponent<LostItem>()._debugName);

        GameObject item = collision.gameObject;


        if (IsMoving)
        {
            if (_objectsOnTop.Count == 0)
            {
                if (transform.position.z > item.transform.position.z - 1)
                    transform.position = new Vector3(transform.position.x, transform.position.y, item.transform.position.z - 1);
            }
            else if (item.transform.position.z == transform.position.z - 1)
            {
                Debug.Log("item  " + item.transform.position.z);
                Debug.Log("ba  " + (transform.position.z - 1));
                item.transform.position += new Vector3(0, 0, 1);
                transform.position -= new Vector3(0, 0, 1);
            }
        }

        if (item.transform.position.z < transform.position.z)
        {
            _objectsOnTop.Add(item);
            _objectsOnTop.Sort((a, b) => (int)(b.transform.position.z - a.transform.position.z) );
        }
    }



    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("Exit: " + _debugName + " " + collision.gameObject.GetComponent<LostItem>()._debugName);
        GameObject item = collision.gameObject;
        
        if (_objectsOnTop.Count > 0 && item == _objectsOnTop[0])
        {
            
        }

        _objectsOnTop.Remove(item);
    }
}
