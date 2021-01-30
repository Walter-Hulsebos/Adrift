using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IButton<out TValue> 
    {
        Transform Frame { get; }
        Transform Knob { get; }
        
        TValue Value { get; }
    }
}
