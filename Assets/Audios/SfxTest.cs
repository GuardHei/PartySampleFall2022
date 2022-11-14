using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            SfxManager.Instance.PlaySfx("AudioTest", Vector3.zero);
        }
    }
}
