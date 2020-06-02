using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public enum WeaponCoreType
    {
        Single,
        Spread,
        AOE,
        Support,
    }

    public enum WeaponAdditionalProperties
    {
        SingleHittingPower,
        HittingType,
        PerMagazineBulletCount,
        FiringRate,
    }

    public enum WeaponComponentType{
        Core,
        Framework,
        Additional,
    }

    public abstract class WeaponComponentBase : Connectable
    {
        public Sprite ComponentSprite;
        public Transform SpriteTransform;
        public Transform ParentConnectorTransform;
        //public Vector2[] PhysicalXyOffsetList;
        public Transform[] ChildrenConnectorTransform;
        protected SpriteRenderer ComponentSpriteRenderer;

        public sealed override bool IsRootConnectable => false;
        //public sealed override bool IsRootConnectable => false;
        public WeaponComponentType ComponentType { protected set; get; }

        public bool LiveNodeAdj = false;

        public abstract bool InitWeaponComponent();
   
        protected virtual bool InitWeaponComponent(WeaponComponentType type)
        {
            SetUpConnectable(ParentConnectorTransform, ChildrenConnectorTransform);
            ComponentType = type;
            return true;
        }

        /*public float Cost;
        public WeaponComponentType ComponentType { protected set; get; }
        public Dictionary<WeaponAdditionalProperties, float> WeaponPropertyModifier { protected set; get; }
        public HashSet<WeaponComponentBase> SubComponents { protected set; get; }

        public virtual bool AllowSubComponent => false;

        public abstract bool InitWeaponComponent();

        protected virtual bool InitWeaponComponent(WeaponComponentType type, float cost)
        {
            ComponentType = type;
            Cost = cost;
            return true;
        }*/

        public void Awake()
        {
#if !UNITY_EDITOR
            LiveNodeAdj = false;
#endif
            InitWeaponComponent();
            ComponentSpriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            ComponentSpriteRenderer.sprite = ComponentSprite;
        }

        public void Update()
        {
#if UNITY_EDITOR
            if (LiveNodeAdj)
            {
                LiveUpdateConnectorPos(ChildrenConnectorTransform);
            }
#endif
        }
    }
}