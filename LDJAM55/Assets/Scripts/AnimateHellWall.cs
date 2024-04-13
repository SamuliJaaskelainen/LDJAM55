using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateHellWall : MonoBehaviour
{
    [SerializeField] AnimationCurve parallaxAnim;
    [SerializeField] float speed = 1.0f;
    MeshRenderer hellWall;
    float time;
    
    void Start()
    {
        hellWall = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        time += speed * Time.deltaTime;
        if(time > 1.0f)
        {
            time -= 1.0f;
        }    
        hellWall.sharedMaterial.SetFloat("_Parallax", parallaxAnim.Evaluate(time));
    }
}
