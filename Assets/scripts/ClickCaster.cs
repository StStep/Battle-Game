using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickCaster : MonoBehaviour
{
    public void Update()
    {
        // Ignore UI click
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        bool left = Input.GetMouseButtonDown(0);
        bool right = Input.GetMouseButtonDown(1);
        if (left || right)
        {
            bool hit = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D raycastHit = Physics2D.Raycast(ray.origin, ray.direction);
            if ((raycastHit != null) && (raycastHit.collider != null))
            {
                Debug.Log("We hit " + raycastHit.collider.gameObject.name);
                if (raycastHit.collider.tag == "ClickObject")
                {
                    hit = true;
                }
            }

            if (hit)
            {
                if (left)
                {
                    raycastHit.collider.gameObject.GetComponent<IClickObject>().LeftClick();
                }
                else
                {
                    raycastHit.collider.gameObject.GetComponent<IClickObject>().RightClick();
                }
            }
        }
    }
}