using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stagecenter : MonoBehaviour
{
    private static stagecenter instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // èdï°ÇñhÇÆ
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
