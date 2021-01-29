using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItemSorter : MonoBehaviour
{
    private Camera _camera;

    private LostItem _item;

    private List<GameObject> _objectsOnTop = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        _item = GetComponent<LostItem>();
        _camera = MainCamera.Instance.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Enter: " + _debugName + " " + collision.gameObject.GetComponent<LostItem>()._debugName);

        GameObject item = collision.gameObject;


        if (_item.IsMoving)
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
            _objectsOnTop.Sort((a, b) => (int)(b.transform.position.z - a.transform.position.z));
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
