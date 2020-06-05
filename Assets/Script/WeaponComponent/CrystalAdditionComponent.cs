using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public class CrystalAdditionComponent : WeaponComponentBase
    {
        public override bool AllowSubConnection => false;

        public override bool InitWeaponComponent()
        {
            bool res=this.InitWeaponComponent(WeaponComponentType.Additional);
            ParentConnector.UseFullConnector = false;
            return res;
        }
    }
}