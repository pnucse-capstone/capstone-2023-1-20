using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeRipple : MonoBehaviour
{
    LineRenderer line;
    public float radius= 3F;
    // Start is called before the first frame update
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.loop = true;
    }
    void drawCircle(float r,float width)
    {
        line.SetWidth(width,width);
        int l=50;
        line.positionCount = l+1;
        Vector3[] points = new Vector3[l+1];
        for(int i=0;i<l+1; i++)
        {
            float angle= (float)i /l * Mathf.PI * 2;
            points[i] = new Vector3(Mathf.Cos(angle)*r,Mathf.Sin(angle)*r);
        }
        line.SetPositions(points);
    }
    public void playRipple()
    {
        StartCoroutine(play());
    }
    IEnumerator play()
    {
        float t = 0.8F;
        for (float i=0;i<=t; i+=Time.deltaTime)
        {
            radius = i/t*5;
            line.material.SetColor("_TintColor" ,new Color(1, 1, 1, (t - i) / t*0.5F));
            
            drawCircle(radius, (t - i) / 5F);
            yield return null;
        }
        line.SetWidth(0F, 0F);
    }
}
