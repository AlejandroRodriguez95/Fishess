using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    [SerializeField]
    GameObject reelRotator;

    [SerializeField]
    GameObject reel;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    [Range(0f, 10f)]
    private float friction = 2f;


    private float previousRotation;

    void Start()
    {
        previousRotation = reelRotator.transform.rotation.eulerAngles.z;
    }


    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        direction.z = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

        float currentRotation = reelRotator.transform.rotation.eulerAngles.z;
        float targetRotationZ = targetRotation.eulerAngles.z;

        if ((targetRotationZ - currentRotation + 360) % 360 <= 120)
        {
            reelRotator.transform.rotation = Quaternion.Lerp(reelRotator.transform.rotation, targetRotation, rotationSpeed/friction * Time.deltaTime);
        }
    }
}




