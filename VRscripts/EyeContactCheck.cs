using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeContactCheck : MonoBehaviour
{
    public GameObject infoContainer;
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 25))
        {
            if (hit.collider.tag == "Student")
            {
                hit.collider.gameObject.GetComponent<StudentActions>().SetEyeContact();
            }
            else if (hit.collider.tag == "Informational")
            {
                hit.collider.gameObject.GetComponent<InformationDisplay>().UpdateToolTip();
            }
            else
            {
                infoContainer.SetActive(false);
            }
        }
    }
}
