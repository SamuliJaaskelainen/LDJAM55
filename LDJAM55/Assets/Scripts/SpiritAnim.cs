using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritAnim : MonoBehaviour
{
    [SerializeField] Material frame1;
    [SerializeField] Material frame2;
    public float speed = 1.0f;
    float time;
    int frame;

    GameObject hellCamera;
    MeshRenderer meshRenderer;

    void Start()
    {
        hellCamera = GameObject.FindGameObjectWithTag("HellCamera");
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        transform.LookAt(hellCamera.transform.position);
        transform.Rotate(Vector3.up, 180.0f, Space.Self);

        time += Time.deltaTime;
        if(time > speed)
        {
            time = 0.0f;
            frame++;
            meshRenderer.material = frame % 2 == 0 ? frame1 : frame2;
        }
    }
}
