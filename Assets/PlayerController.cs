using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationRate = 90f;
    Vector3 controlRotation;

    public Vector3Int targetBlock;
    public Vector3Int placeBlock;
    public Transform cursor;
    //get players target block
    void GetTargetBlock()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out hit, 10f))
        {
            Vector3 point = hit.point - (hit.normal * 0.1f);
            targetBlock = new Vector3Int(
                Mathf.RoundToInt(point.x),
                Mathf.RoundToInt(point.y),
                Mathf.RoundToInt(point.z)
            );
            Vector3 p = targetBlock + hit.normal;
            if (cursor) cursor.position = targetBlock;
            placeBlock = new Vector3Int(
                Mathf.RoundToInt(p.x),
                Mathf.RoundToInt(p.y),
                Mathf.RoundToInt(p.z)
            );
        }
    }
    //Look around the world
    void Aim()
    {
        float pitch = Input.GetAxisRaw("Mouse Y");
        float yaw = Input.GetAxisRaw("Mouse X");
        controlRotation.y += yaw * rotationRate * Time.deltaTime;
        controlRotation.x = Mathf.Clamp(controlRotation.x - pitch * rotationRate * Time.deltaTime, -89f, 89f);
        Camera.main.transform.rotation = Quaternion.Euler(controlRotation);
    }
    //Move around the world
    void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = Vector3.zero;
        moveDirection += Camera.main.transform.forward * yInput;
        moveDirection += Camera.main.transform.right * xInput;
        moveDirection.Normalize();
        Camera.main.transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
   

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        Move();
        GetTargetBlock();
        // set block based on input
        if (Input.GetMouseButtonDown(0)) Chunk.SetVoxelType(placeBlock.x, placeBlock.y, placeBlock.z, Voxel.Type.Grass);
    }
}
