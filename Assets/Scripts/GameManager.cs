using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector3 gravityVector = new Vector3(0f, -30f, 0f);
    // Start is called before the first frame update
    void Awake()
    {
        Physics.gravity = gravityVector;
    }
}
