using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateHellWall : MonoBehaviour
{
    [SerializeField] AnimationCurve parallaxAnim;
    [SerializeField] float speed = 1.0f;
    MeshRenderer hellWall;
    float time;
    MaterialPropertyBlock propertyBlock;

    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        hellWall = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        time += speed * Time.deltaTime;
        if(time > 1.0f)
        {
            time -= 1.0f;
        }
        hellWall.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_Parallax", parallaxAnim.Evaluate(time));
        hellWall.SetPropertyBlock(propertyBlock);
    }
}
