using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : utility.Singleton<Table>
{
    public Bounds Bounds
    {
        get
        {
            return _collider.bounds;
        }
    }

    private BoxCollider2D _collider;


    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }


    public void ConstraintMovement(LostItem item)
    {
        if (item.IsMoving)
        {
            var itemBounds = item.GetComponent<BoxCollider2D>().bounds;
            var itemNewPosition = item.NewPosition;

            //Check if point is not within bounds
            if (itemNewPosition.x + itemBounds.extents.x > Bounds.max.x)
            {
                itemNewPosition.x = Bounds.max.x - itemBounds.extents.x;
            }
            else if (itemNewPosition.x - itemBounds.extents.x < Bounds.min.x)
            {
                itemNewPosition.x = Bounds.min.x + itemBounds.extents.x;
            }
           
            if (itemNewPosition.y + itemBounds.extents.y > Bounds.max.y && !item.IsSelected)
            {
                itemNewPosition.y = Bounds.max.y - itemBounds.extents.y;
            }
            else if (itemNewPosition.y - itemBounds.extents.y < Bounds.min.y)
            {
                itemNewPosition.y = Bounds.min.y + itemBounds.extents.y;
            }

            item.NewPosition = itemNewPosition;
        }
    }
}
