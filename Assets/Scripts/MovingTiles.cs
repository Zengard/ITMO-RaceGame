using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTiles : MonoBehaviour
{
    public float scrollSpeed = 0.5F;
    float offset;
    public Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
         offset = Time.time * scrollSpeed;
        rend.material.mainTextureOffset = new Vector2(offset * scrollSpeed, 0);
    }
}
