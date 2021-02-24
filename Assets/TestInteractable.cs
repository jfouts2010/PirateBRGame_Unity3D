using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class TestInteractable : vp_Interactable
    {
        public override bool TryInteract(vp_PlayerEventHandler player)
        {
            Debug.Log("interacted!");
            return false;
        }
    }
}
