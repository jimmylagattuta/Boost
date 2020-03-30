using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class VerticalWalls : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    float movementFactor;

    Vector3 startingPos; //must be stored for absolute movement

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        //set movement factor
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period; // grows continually from 0
        const float tau = Mathf.PI * 2f; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from minus 1 to plus 1

        movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
