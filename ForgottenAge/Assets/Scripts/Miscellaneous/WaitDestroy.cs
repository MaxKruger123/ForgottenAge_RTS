using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitDestroy : MonoBehaviour
{
    public float waitTime = 1f;

    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
