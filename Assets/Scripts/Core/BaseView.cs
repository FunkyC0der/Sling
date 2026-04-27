using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseView : MonoBehaviour
{
    public virtual List<Type> GetTypesToRegister() => 
        new() { GetType() };
}
