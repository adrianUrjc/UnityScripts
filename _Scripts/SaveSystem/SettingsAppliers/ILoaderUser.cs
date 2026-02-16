using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoaderUser
{
    
    public void SubscribeToValuesChange(); 
    public void OnValuesChange();
}