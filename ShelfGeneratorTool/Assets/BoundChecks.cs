using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundChecks : MonoBehaviour
{
    public Text txt = null;
    public GameObject cover;
    public Renderer rnd = null;

    // Start is called before the first frame update
    void Start()
    {
        rnd = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        string msg = "";
        msg += this.name + " " + rnd.bounds.center.ToString() + "\n";
        msg += "X: " + rnd.bounds.min.x.ToString("0.0") + " to " + rnd.bounds.max.x.ToString("0.0") + "\n";
        msg += "Z: " + rnd.bounds.min.z.ToString("0.0") + " to " + rnd.bounds.max.z.ToString("0.0") + "\n";
        print(msg);

        cover.transform.position = rnd.bounds.center;
        cover.transform.localScale = rnd.bounds.size;
    }
}
