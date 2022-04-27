using PixelDust.Audiophile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeightedObjects;

public class Test : MonoBehaviour
{
    //public string playerName;
    //public Vector3 playerUp;
    //public GameObject playerRefGo;
    //public float playerSpeed;
    //public Color playerColor;

    public SoundEvent myTestSound;
    public SoundEventCollection myTestCollection;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            myTestSound.PlayAt(this.transform);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            myTestCollection.PlayAt(this.transform);
        }
    }
}