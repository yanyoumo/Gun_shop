using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public sealed class WeaponSimple : WeaponBase
    {
        public new void Awake()
        {
            base.Awake();
        }

        public override void InitWeapon()
        {
            ChildConnectors[0].UseFullConnector = true;
        }
    }
}
