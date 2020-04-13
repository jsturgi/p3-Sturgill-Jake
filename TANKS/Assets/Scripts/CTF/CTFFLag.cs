using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFFLag : MonoBehaviour
{
    public GameObject Flag;
    // Start is called before the first frame update
    void Start()
    {
        CTFBroker.FlagTaken += CTFBroker_FlagTaken;
        Flag.SetActive(true);
    }

    private void CTFBroker_FlagTaken()
    {
        OnFlagTaken();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnFlagTaken()
    {
        Flag.SetActive(false);
    }
}
