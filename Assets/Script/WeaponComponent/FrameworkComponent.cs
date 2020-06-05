using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public class FrameworkComponent : WeaponComponentBase
    {
        public override bool AllowSubConnection => true;

        public override bool InitWeaponComponent()
        {
            return this.InitWeaponComponent(WeaponComponentType.Framework);
        }
    }
}