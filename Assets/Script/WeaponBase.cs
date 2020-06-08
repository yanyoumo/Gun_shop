using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GunShop
{
    public abstract class WeaponBase : Connectable
    {
        public Sprite CoreSprite;
        public Transform SpriteTransform;
        public Transform CoreConnectorTransform;
        protected SpriteRenderer coreSpriteRenderer;

        public sealed override bool IsRootConnectable => true;
        public sealed override bool AllowSubConnection => true;
        public override PolygonCollider2D MainCollider2D => coreSpriteRenderer.GetComponent<PolygonCollider2D>();

        public float Price;
        public float Cost;
        public WeaponCoreType CoreType;

        public Dictionary<WeaponAdditionalProperties, float> WeaponAdditionalProperties { protected set; get; }
        public HashSet<string> WeaponAdditionalEntries { protected set; get; }
        
        public abstract void InitWeapon();

        public virtual bool CheckWeaponValid()
        {
            return true;
        }

        public new void Awake()
        {
            base.Awake();
            SetUpConnectable(default, new[]
            {
                CoreConnectorTransform,
            });
            InitWeapon();
            coreSpriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            coreSpriteRenderer.sprite = CoreSprite;
            gameObject.AddComponent<CompositeCollider2D>();
        }

        public void Start()
        {
            
        }

        public override void Update()
        {
            base.Update();
            this.UpdateConnectableAndChildPos();
        }
    }
}
