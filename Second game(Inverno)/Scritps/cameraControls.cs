using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControls : MonoBehaviour
{
    [Header("----- Camera Fields -----")]
    [SerializeField] int sensHorz;
    [SerializeField] int sensVert;
    [SerializeField] int lockVertMin;
    [SerializeField] int lockVertMax;
    [SerializeField] float shoulderOffset;
    [SerializeField] bool inverty;

    [Header("----- Camera Collision Fields -----")]
    [SerializeField] Transform refTransform;
    [SerializeField] float collisionOffset;
    [SerializeField] float cameraSnapSpeed;

    //private collision fields
    Vector3 startPos;
    Vector3 normalizedDir;
    Transform parentPos;
    float origDistance;
    

    float xRotation;
    bool dirRight;
    bool freeLook;
    Vector3 currVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //getting initial info for collision detection
        startPos = transform.localPosition;
        normalizedDir = startPos.normalized;
        parentPos = transform.parent;
        origDistance = Vector3.Distance(startPos, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensHorz;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensVert;

        if (inverty)
            xRotation += mouseY;
        else
            xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        if(Input.GetButton("FreeLook"))
        {
            freeLook = true;
            transform.LookAt(parentPos.position + new Vector3(0, 1.25f, 0));
            transform.RotateAround(parentPos.position, Vector3.up, mouseX);
        }
        else
        {
            freeLook = false;
            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }

    void LateUpdate()
    {
        Vector3 currPos = startPos;
        RaycastHit hit;
        //Vector3 shoulderPos;
        Vector3 dirTemp = parentPos.TransformPoint(startPos) - refTransform.position;
        if(!freeLook)
        {
        if(Physics.SphereCast(refTransform.position, collisionOffset, dirTemp, out hit, origDistance))
        {
            currPos =  (normalizedDir * (hit.distance - collisionOffset));

            transform.localPosition = currPos;
        }
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, currPos, cameraSnapSpeed * Time.deltaTime);
        }

        /*if(Input.GetAxisRaw("Horizontal") > 0 && !dirRight)
            startPos += transform.right * shoulderOffset;
        else if(Input.GetAxisRaw("Horizontal") < 0 && dirRight)
            startPos -= transform.right * shoulderOffset;*/
         
    }
}
