using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public class CystalAdditionComponent : WeaponComponentBase
    {
        public new const int ConnectorCount = 0;
        public override bool AllowSubConnection => false;

        public override bool InitWeaponComponent()
        {
            return this.InitWeaponComponent(WeaponComponentType.Additional);
        }
    }
}