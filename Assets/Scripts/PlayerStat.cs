using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStat : MonoBehaviour
{
    public float damaged = 0;
    public float fuel = 100f;
    public float capacity;
    public int laps = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            damaged += 5f;

            if(damaged >= 100f)
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Blue")
        {
            capacity += 10f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Red")
        {
            fuel += 25f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Green")
        {
            damaged -= 30f;
            if(damaged <= 0)
            {
                damaged = 0;
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Yellow")
        {
            damaged = 0;
            Destroy(other.gameObject);
        }

    }
}
