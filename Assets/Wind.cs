using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
namespace Assets
{
    public class Wind : MonoBehaviour
    {
        public Vector3 WindDirection;
        public void Start()
        {
            WindDirection = new Vector3(0, 0, 1);
        }
    }
}
