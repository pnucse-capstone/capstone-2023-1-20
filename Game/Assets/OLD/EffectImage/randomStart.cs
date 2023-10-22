using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomStart : MonoBehaviour
{
    // Start is called before the first frame update
    public float xmin = -1F;
    public float xmax = 1F;
    public float ymin = 0F;
    public float ymax = 1F;
    void Start()
    {
        reposit();
    }
    public void reposit()
    {
        gameObject.transform.position = new Vector3(Random.Range(xmin, xmax) * 2 * Camera.main.orthographicSize, 
            Random.Range(ymin, ymax) * Camera.main.orthographicSize, -0.5F);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
