using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNetwork : MonoBehaviour
{
    public CharacterController characterController;
    public GameObject characterCamera;
    public GameObject headObject;

    float rotX;
    float rotY;

    [Header("Network Setup")]
    public KolhozNetworkConfiguration networkConfiguration;
    public SyncVariables syncVariables;

    private void Start()
    {
        if(networkConfiguration.IsMine(gameObject))
        {
            characterCamera.SetActive(true);
            syncVariables.enabled = false;
            characterController.enabled = true;
            headObject.SetActive(false);
            this.enabled = true;
            //Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            characterCamera.SetActive(false);
            syncVariables.enabled = true;
            characterController.enabled = false;
            headObject.SetActive(true);
            this.enabled = false;
        }
    }

    private void Update()
    {
        float strafe = Input.GetAxis("Horizontal");
        float translate = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(strafe, characterController.velocity.y, translate);
        direction = transform.TransformDirection(direction * 8);

        characterController.SimpleMove(direction);

        rotX += Input.GetAxis("Mouse X") * 4;
        rotY -= Input.GetAxis("Mouse Y") * 4;
        rotY = Mathf.Clamp(rotY, -90, 90);

        transform.rotation = Quaternion.Euler(0, rotX, 0);
        characterCamera.transform.rotation = Quaternion.Euler(rotY, rotX, 0);
    }
}
