using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Vector3[] _positions = { new Vector3 { x = 0, y = 0, z = 0 },
                         new Vector3 { x = 10, y = 10, z = 10},
                         new Vector3 { x = 20, y = 20, z = 20},
                         new Vector3 { x = 30, y = 30, z = 30}};
    int _timesCalledCoroutine = 0;
    int i = 0;

    void Start()
    {
        gameObject.transform.position = Vector3.zero;
        StartCoroutine(WaitToMove());
    }

    IEnumerator WaitToMove()
    {
        while (true)
        {
            _timesCalledCoroutine += 1;
            gameObject.transform.position = _positions[i];
            //Debug.Log("iter: " + i);
            i = i >= _positions.Length - 1 ? 0 : i + 1;
            //Debug.Log("Coroutine: " + _timesCalledCoroutine);
            yield return new WaitForSeconds(1);
        }
    }
}
