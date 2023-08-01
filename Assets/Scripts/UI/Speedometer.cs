using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private Rigidbody playerRB;
    private TextMeshProUGUI speedText;
    private float speedLength = 10000f;
    void Awake()
    {
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
        speedText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = "Speed: " + Mathf.Round(playerRB.velocity.magnitude * speedLength) / speedLength;
    }
}
