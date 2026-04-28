using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sling.Core
{
  public abstract class BaseView : MonoBehaviour
  {
    public virtual List<Type> GetTypesToRegister()
    {
      return new List<Type> { GetType() };
    }
  }
}