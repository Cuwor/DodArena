using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsedObject: MyTools
{
    public string tip;
    public abstract void Use();
}

public class DialogActivator : UsedObject {

    public override void Use()
    {
        Dialog d;
        if(MyGetComponent<Dialog>(out d, gameObject))
        {
            d.Show = true;
        }
    }


}
