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

        public virtual bool ConnectedToParent => ParentConnector.Device != null;

        public sealed override bool IsRootConnectable => false;
        //public sealed override bool IsRootConnectable => false;
        public WeaponComponentType ComponentType { protected set; get; }

        public bool LiveNodeAdj = false;

        public abstract bool InitWeaponComponent();

        public override PolygonCollider2D MainCollider2D => ComponentSpriteRenderer.GetComponent<PolygonCollider2D>();


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

        /*public void OnMouseExit()
        {
            TurnOffDisplayConnector();
        }*/

        public new void Awake()
        {
            base.Awake();
#if !UNITY_EDITOR
            LiveNodeAdj = false;

#endif
            ComponentSpriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            ComponentSpriteRenderer.sprite = ComponentSprite;
            InitWeaponComponent();
        }

        public void Start()
        {
            //TurnOnDisplayConnector();
            //TurnOffDisplayConnector();
        }

        public override void Update()
        {
            base.Update();
#if UNITY_EDITOR
            if (LiveNodeAdj)
            {
                LiveUpdateConnectorPos(ChildrenConnectorTransform);
            }
#endif
        }
    }
}