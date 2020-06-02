﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public class Connector
    {
        public Vector2 PhysicalXyOffsetToHost = Vector2.zero;
        //public Transform LocalTransformFromHost;
        public Connectable Host;
        public Connectable Device;
        public Connector OtherConnector;
        
        public static bool ConnectConnector(Connector parent, Connector child)
        {
            parent.Device = child.Host;
            child.Device = parent.Host;
            parent.OtherConnector = child;
            child.OtherConnector = parent;
            return true;
        }   
    }

    public abstract class Connectable : MonoBehaviour
    {
        //public const int ConnectorCount = 1;    
        public Connector ParentConnector { protected set; get; }
        public Connector[] ChildConnectors { protected set; get; }
        public virtual bool IsRootConnectable => false;
        public virtual bool AllowSubConnection => false;

        protected void LiveUpdateConnectorPos(Transform[] offsetTransform)
        {
            Debug.Assert(offsetTransform.Length==ChildConnectors.Length);
            for (var i = 0; i < ChildConnectors.Length; i++)
            {
                ChildConnectors[i].PhysicalXyOffsetToHost = offsetTransform[i].localPosition;
            }
        }

        protected void SetUpConnectable(Transform parentTransform, Transform[] offsetTransform)
        {
            Vector2 parentOffset = parentTransform.localPosition;
            if (offsetTransform != null)
            {
                if (offsetTransform.Length > 0)
                {
                    Vector2[] offsetList = new Vector2[offsetTransform.Length];
                    for (var i = 0; i < offsetTransform.Length; i++)
                    {
                        offsetList[i] = offsetTransform[i].localPosition;
                    }
                    SetUpConnectable(parentOffset, offsetList);
                    return;
                }
            }
            SetUpConnectable(parentOffset, null);
        }

        protected void SetUpConnectable(Vector2 parentOffset = default, Vector2[] offsetList = null)
        {
            //Debug.Assert(ConnectorCount > 0);
            ParentConnector = new Connector
            {
                PhysicalXyOffsetToHost = parentOffset, Host = this
            };
            if (offsetList != null)
            {
                ChildConnectors = new Connector[offsetList.Length];
                for (var i = 0; i < ChildConnectors.Length; i++)
                {
                    ChildConnectors[i] = new Connector();
                    if (i < offsetList.Length)
                    {
                        ChildConnectors[i].PhysicalXyOffsetToHost = offsetList[i];
                    }

                    ChildConnectors[i].Host = this;
                }
            }
            else
            {
                ChildConnectors = null;
            }
        }

        protected void UpdateConnectableAndChildPos()
        {
            if (!IsRootConnectable)
            {
                System.Diagnostics.Debug.Assert(ParentConnector != null, nameof(ParentConnector) + " != null");
                Vector3 targetPos = ParentConnector.OtherConnector.Host.transform.position;
                Vector2 otherOffset = ParentConnector.OtherConnector.PhysicalXyOffsetToHost;
                otherOffset -= ParentConnector.PhysicalXyOffsetToHost;
                targetPos += new Vector3(otherOffset.x, otherOffset.y, 0);
                this.transform.position = targetPos;
            }

            if (AllowSubConnection)
            {
                if (ChildConnectors != null)
                {
                    if (ChildConnectors.Length > 0)
                    {
                        foreach (var childConnector in ChildConnectors)
                        {
                            childConnector.Device?.UpdateConnectableAndChildPos();
                        }
                    }
                }
            }
        }
    }

    public abstract class WeaponBase : Connectable
    {
        public Sprite CoreSprite;
        public Transform SpriteTransform;
        public Transform CoreConnectorTransform;
        protected SpriteRenderer coreSpriteRenderer;

        public sealed override bool IsRootConnectable => true;
        public sealed override bool AllowSubConnection => true;

        //public float Price;
        //public float Cost;
        //public WeaponCoreType CoreType;


        //public Dictionary<WeaponAdditionalProperties, float> WeaponAdditionalProperties { protected set; get; }
        //public HashSet<WeaponComponentType> NecessaryComponentTypes { protected set; get; }

        /*public bool AddComponent(WeaponComponentBase weaponComponentBase)
        {
            return WeaponComponents.Add(weaponComponentBase);
        }*/

        public abstract void InitWeapon();

        public virtual bool CheckWeaponValid()
        {
            return true;
            /*bool res = true;
            foreach (var necessaryComponentType in NecessaryComponentTypes)
            {
                foreach (var weaponComponentBase in WeaponComponents)
                {
                    if (weaponComponentBase.ComponentType==necessaryComponentType)
                    {
                        break;
                    }
                    res = false;
                }
            }
            return res;*/
        }

        public void Awake()
        {
            SetUpConnectable(default, new[]
            {
                new Vector2(CoreConnectorTransform.transform.localPosition.x,
                    CoreConnectorTransform.transform.localPosition.y),
            });
            InitWeapon();
            coreSpriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            coreSpriteRenderer.sprite = CoreSprite;
        }

        public void Start()
        {
            
        }

        public void Update()
        {
            this.UpdateConnectableAndChildPos();
        }
    }
}