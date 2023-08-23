using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    private bool isAlive = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Destroy(gameObject);
            Debug.Log("FLAG");
        }
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return null;
            if (!isAlive)
            {
                Debug.Log("not alive");
            }
        }
    }
}
