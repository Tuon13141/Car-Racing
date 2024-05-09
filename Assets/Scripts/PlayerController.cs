using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private List<Transform> listPosition;

    private int currentIndex;
    private bool reachPosition;
    public float moveSpeed = 0;
    public float rotationSpeed;
    public Vector3 moveDirection;
    private bool moveOpposite = false;

    [SerializeField]
    private float slowDownSpeed;
    [SerializeField]
    private float speedUpSpeed;
    [SerializeField]
    private float slowMoveSpeed; 
    [SerializeField]
    private float normalMoveSpeed;
    [SerializeField]
    private float automaticRotationSpeed;
    [SerializeField]
    private float manualMaxRotationSpeed;
    [SerializeField]
    private float manualMinRotationSpeed;
    [SerializeField]
    private float manualRotationIncreaseOrDecreaseSpeed;
    [SerializeField]
    private DriveMode driveMode = DriveMode.automatic;
    
    void Start()
    {
        StartCoroutine(ChangePosition());
        //moveSpeed = normalMoveSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeDriveMode();
        }

        switch (driveMode)
        {
            case DriveMode.automatic:
                AutomaticMode();
                break;
            case DriveMode.manual:
                ManualMode();
                break;
        }
        
    }

    float Increase(float a, float b, float c)
    {
        a += c * Time.deltaTime;
        if (a > b)
        {
            a = b;
        }
        return a;
    }

    float Decrease(float a, float b, float c)
    {
        a -= c * Time.deltaTime;
        if (a < b)
        {
            a = b;
        }
        return a;
    }

    void RotateObjectToPoint(Vector3 targetPoint)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
    }

    void ChangeDriveMode()
    {
        if(driveMode == DriveMode.manual)
        {
            driveMode = DriveMode.automatic;
        }
        else
        {
            driveMode = DriveMode.manual;
        }
    }

    IEnumerator ChangePosition()
    {
        yield return new WaitUntil(() => reachPosition);

        currentIndex++;
        if(currentIndex >= listPosition.Count)
        {
            currentIndex = 0;
        }
        reachPosition = false;

        StartCoroutine(ChangePosition());
    }

    void AutomaticMode()
    {
        rotationSpeed = automaticRotationSpeed;
        transform.position = Vector3.MoveTowards(transform.position, listPosition[currentIndex].position, moveSpeed * Time.deltaTime);

        float distance = Vector3.Distance(this.transform.position, listPosition[currentIndex].position);
        if (distance == 0f)
        {
            reachPosition = true;
        }
        else
        {
            
            if (distance <= 24f)
            {
                moveSpeed -= slowDownSpeed * Time.deltaTime;
                if (moveSpeed < slowMoveSpeed)
                {
                    moveSpeed = slowMoveSpeed;
                }
            }
            else
            {
                moveSpeed += speedUpSpeed * Time.deltaTime;
                if (moveSpeed > normalMoveSpeed)
                {
                    moveSpeed = normalMoveSpeed;
                }
            }

            RotateObjectToPoint(listPosition[currentIndex].position);
        }
    }

    void ManualMode()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(0f, 0f, verticalInput).normalized;
        
        if(inputDirection != Vector3.zero)
        {
            if(moveDirection.z == inputDirection.z || moveDirection == Vector3.zero)
            {
                moveDirection = inputDirection;
                if(moveSpeed < normalMoveSpeed)
                {
                    moveSpeed = Increase(moveSpeed, normalMoveSpeed, speedUpSpeed);
                }             
                if(rotationSpeed < manualMaxRotationSpeed)
                {
                    rotationSpeed = Increase(rotationSpeed, manualMaxRotationSpeed, manualRotationIncreaseOrDecreaseSpeed);
                }
            }
            else
            {
                if (!moveOpposite)
                {
                    moveOpposite = true;
                }
                if (moveSpeed > 0 && moveOpposite)
                {
                    moveSpeed = Decrease(moveSpeed, 0, slowDownSpeed);
                    if (rotationSpeed > manualMinRotationSpeed)
                    {
                        rotationSpeed = Decrease(rotationSpeed, manualMinRotationSpeed, manualRotationIncreaseOrDecreaseSpeed);
                    }
                    if (moveSpeed == 0)
                    {
                        moveOpposite = false;
                        moveDirection = inputDirection;
                    }
                }
                else if (moveSpeed < normalMoveSpeed && !moveOpposite)
                {
                    moveDirection = inputDirection;
                    moveSpeed = Increase(moveSpeed, normalMoveSpeed, speedUpSpeed);
                    if (rotationSpeed < manualMaxRotationSpeed)
                    {
                        rotationSpeed = Increase(rotationSpeed, manualMaxRotationSpeed, manualRotationIncreaseOrDecreaseSpeed);
                    }
                }
            }
        }
        else
        {
            if(moveSpeed > 0)
            {
                moveSpeed = Decrease(moveSpeed, 0, slowDownSpeed);
                if (rotationSpeed > manualMinRotationSpeed)
                {
                    rotationSpeed = Decrease(rotationSpeed, manualMinRotationSpeed, manualRotationIncreaseOrDecreaseSpeed);
                }
            }
            else
            {
                moveDirection = Vector3.zero;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            moveSpeed = Decrease(moveSpeed, slowMoveSpeed, slowDownSpeed * 1.2f);
        }

        if (moveSpeed == 0)
        {
            rotationSpeed = 0;
        }

        if (moveDirection != Vector3.zero)
        {
            if (horizontalInput < 0)
            {
                transform.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
            }
            else if (horizontalInput > 0)
            {
                transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }


        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}

public enum DriveMode
{
    manual,
    automatic,
}
