using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public class Connector
    {
        //public Vector2 PhysicalXyOffsetToHost = Vector2.zero;
        public Transform LocalTransformFromHost;
        public Connectable Host;
        public Connectable Device;
        public Connector OtherConnector;

        public GameObject NodeGO => LocalTransformFromHost.gameObject;

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
        public bool ShowSubConnectorIdc;
        public Connector ParentConnector { protected set; get; }
        public Connector[] ChildConnectors { protected set; get; }
        public virtual bool IsRootConnectable => false;
        public virtual bool AllowSubConnection => false;
        public Sprite ConnectorIndicatorSprite;

        //private GameObject DisplayConnector;

        protected void TurnOnDisplayConnector()
        {
            foreach (var childConnector in ChildConnectors)
            {
                SpriteRenderer var = childConnector.NodeGO.AddComponent<SpriteRenderer>();
                var.sprite = ConnectorIndicatorSprite;
                var.sortingOrder = -1;
            }
        }


        protected void TurnOffDisplayConnector()
        {
            foreach (var childConnector in ChildConnectors)
            {
                SpriteRenderer var=childConnector.NodeGO.GetComponent<SpriteRenderer>();
                Destroy(var);
            }
        }

        protected void LiveUpdateConnectorPos(Transform[] offsetTransform)
        {
            Debug.Assert(offsetTransform.Length==ChildConnectors.Length);
            for (var i = 0; i < ChildConnectors.Length; i++)
            {
                ChildConnectors[i].LocalTransformFromHost = offsetTransform[i];
            }
        }

        protected void SetUpConnectable(Transform parentTransform, Transform[] offsetTransform)
        {
            ParentConnector = new Connector
            {
                LocalTransformFromHost = parentTransform,
                Host = this
            };
            if (offsetTransform != null)
            {
                ChildConnectors = new Connector[offsetTransform.Length];
                for (var i = 0; i < ChildConnectors.Length; i++)
                {
                    ChildConnectors[i] = new Connector();
                    if (i < offsetTransform.Length)
                    {
                        ChildConnectors[i].LocalTransformFromHost = offsetTransform[i];
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

                Vector3 parentConnectionRot = ParentConnector.OtherConnector.LocalTransformFromHost.eulerAngles;
                Vector3 localRotOffset = ParentConnector.LocalTransformFromHost.eulerAngles - this.transform.localEulerAngles;
                this.transform.eulerAngles = parentConnectionRot - localRotOffset;
                Vector3 parentConnectionPos = ParentConnector.OtherConnector.LocalTransformFromHost.position;
                Vector3 localPosOffset = ParentConnector.LocalTransformFromHost.position - this.transform.position;
                this.transform.position = parentConnectionPos - localPosOffset;
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
                CoreConnectorTransform,
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
