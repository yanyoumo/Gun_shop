using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public sealed class WeaponSimple : WeaponBase
    {
        public new const int ConnectorCount = 2;

        public new void Awake()
        {
            base.Awake();
        }

        public override void InitWeapon()
        {
            /*NecessaryComponentTypes=new HashSet<WeaponComponentType>()
            {
                WeaponComponentType.Core,
                WeaponComponentType.Framework,
                WeaponComponentType.Additional
            };*/
        }
    }
}
