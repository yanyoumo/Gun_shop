using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public sealed class Connector : MonoBehaviour
    {
        public delegate void ConnectorHoveredDelegate(Connector self,int idx);
        public static event ConnectorHoveredDelegate ConnectorHovered;
        //
        internal static readonly string PARENT_CONNECTOR_ROOT_NAME = "ParentConnectorRoot";
        internal static readonly string CHILDREN_CONNECTOR_ROOT_NAME = "ChildrenConnectorRoot";
        //
        [HideInInspector]
        public Connectable Host;
        [HideInInspector]
        public Connectable Device;
        [HideInInspector]
        public Connector OtherConnector;

        public int ConnectorID {internal set; get; }
        private bool _useFullConnector = false;

        public GameObject HalfConnector;
        public GameObject FullConnector;

        public SpriteRenderer ConnectorSpriteRenderer
        {
            get
            {
                if (_useFullConnector)
                {
                    Debug.Assert(FullConnector.activeSelf);
                    return FullConnector.GetComponentInChildren<SpriteRenderer>();
                }
                else
                {
                    Debug.Assert(HalfConnector.activeSelf);
                    return HalfConnector.GetComponentInChildren<SpriteRenderer>();
                }
            }
        }

        public bool EnableCollider
        {
            set
            {
                if (HalfConnector != null)
                {
                    HalfConnector.GetComponent<PolygonCollider2D>().enabled = value;
                }
                if (FullConnector != null)
                {
                    FullConnector.GetComponent<PolygonCollider2D>().enabled = value;
                }
            }
        }

        public bool UseFullConnector
        {
            set
            {
                _useFullConnector = value;
                HalfConnector.SetActive(!_useFullConnector);
                FullConnector.SetActive(_useFullConnector);
            }
            get => _useFullConnector;
        }

        internal bool ShowSprite
        {
            set => ConnectorSpriteRenderer.enabled = value;
        }

        public void Awake()
        {
            FullConnector.GetComponentInChildren<SpriteRenderer>().sortingOrder = GlobalHelper.SPRITE_LAYER_ORDER_FULL_CONNECTOR;
            HalfConnector.GetComponentInChildren<SpriteRenderer>().sortingOrder = GlobalHelper.SPRITE_LAYER_ORDER_HALF_CONNECTOR;
        }

        public void OnMouseEnter()
        {
            ConnectorHovered?.Invoke(this,ConnectorID);
        }

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
        public GameObject ConnectorPrefabTemplate;
//
        protected Transform ParentConnectorRootTrans;
        protected Transform ChildConnectorsRootTrans;
        [HideInInspector]
        public Connector ParentConnector;
        [HideInInspector]
        public Connector[] ChildConnectors;

        public bool ShowSubConnectorIdc;
        public virtual bool IsRootConnectable => false;
        public virtual bool AllowSubConnection => false;
        public Sprite ConnectorIndicatorSprite;
        public SpriteRenderer RootIndicatorSpriteRenderer;
        public Rigidbody2D ComponentRigidBody2D { get; set; }

        private Vector3 _mouseDragOffset = Vector3.zero;

        public abstract PolygonCollider2D MainCollider2D {get;}

        public bool ConnectableActive
        {
            get
            {
                if (IsRootConnectable)
                {
                    return true;
                }
                return ParentConnector.Device != null;
            }
        }


        protected bool DisplayEmptyChildConnector
        {
            set
            {
                foreach (var childConnector in ChildConnectors)
                {
                    childConnector.ShowSprite = value && (childConnector.Device == null);
                }
            }
        }

        protected bool EnableEmptyChildConnectorCollider
        {
            set
            {
                foreach (var childConnector in ChildConnectors)
                {
                    childConnector.EnableCollider = value && (childConnector.Device == null);
                }
            }
        }

        protected bool EnableEmptyParentConnectorCollider
        {
            set
            {
                if (ParentConnector != null)
                {
                    ParentConnector.EnableCollider = value && (ParentConnector.Device == null);
                }
            }
        }

        //private GameObject DisplayConnector;
        private Vector3 GetMouseWorldPos
        {
            get
            {
                var v3 = Input.mousePosition;
                v3.z = 10.0f;
                v3 = Camera.main.ScreenToWorldPoint(v3);
                v3.z = 0.0f;
                return v3;
            }
        }

        public virtual void Awake()
        {
            Destroy(RootIndicatorSpriteRenderer);
            ComponentRigidBody2D = gameObject.AddComponent<Rigidbody2D>();
            ComponentRigidBody2D.bodyType = RigidbodyType2D.Static;
        }

        public virtual void Update()
        {
            bool draggingButNotSelf = GameManager.DraggingConnectableNotNullNeitherSelf(this);
            if (ConnectableActive)
            {
                DisplayEmptyChildConnector = draggingButNotSelf;
                EnableEmptyChildConnectorCollider = draggingButNotSelf;
                EnableEmptyParentConnectorCollider = draggingButNotSelf;
                MainCollider2D.enabled = IsRootConnectable;
            }
            else
            {
                MainCollider2D.enabled = !GameManager.DraggingAnyConnectable;
            }
        }

        private bool IsMineConnector(Connector hoveredConnector, int idx)
        {
            if (idx < ChildConnectors.Length)
            {
                //Debug.Log(this.name);
                if (ChildConnectors[idx] == hoveredConnector)
                {
                    //Debug.Log(this.name);
                    return true;
                }
            }
            return false;
        }

        protected virtual void SomeChildrenConnectHovered(Connector hoveredConnector, int idx)
        {
            //Debug.Log(this.name);
            if (IsMineConnector(hoveredConnector, idx))
            {
                //Debug.Log(this.name);
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (GameManager.DraggingConnectableNotNullNeitherSelf(this))
                    {
                        if (ChildConnectors[idx].Device == null)
                        {
                            Debug.Log(this.name);
                            Connector.ConnectConnector(ChildConnectors[idx], GameManager.DraggingParentConnector);
                        }
                    }
                }
            }
        }

        protected void LiveUpdateConnectorPos(Transform[] offsetTransform)
        {
            Debug.Assert(offsetTransform.Length== ChildConnectors.Length);
            for (var i = 0; i < ChildConnectors.Length; i++)
            {
                ChildConnectors[i].transform.localPosition = offsetTransform[i].localPosition;
                ChildConnectors[i].transform.localEulerAngles = offsetTransform[i].localEulerAngles;
            }
        }

        protected void SetUpConnectable(Transform parentTransform, Transform[] offsetTransform)
        {
            if (!IsRootConnectable)
            {
                ParentConnectorRootTrans = new GameObject(Connector.PARENT_CONNECTOR_ROOT_NAME).transform;
                ParentConnectorRootTrans.transform.parent = this.transform;
                ParentConnectorRootTrans.localPosition = Vector3.zero;
                ParentConnectorRootTrans.localEulerAngles = Vector3.zero;

                ParentConnector = Instantiate(ConnectorPrefabTemplate).transform.GetComponent<Connector>();
                ParentConnector.transform.parent = ParentConnectorRootTrans;
                ParentConnector.transform.transform.localPosition = parentTransform.localPosition;
                ParentConnector.transform.transform.localEulerAngles = parentTransform.localEulerAngles;

                ParentConnector.Host = this;
                ParentConnector.UseFullConnector = true;
                ParentConnector.ShowSprite = false;
            }

            if (offsetTransform != null)
            {
                ChildConnectorsRootTrans = new GameObject(Connector.CHILDREN_CONNECTOR_ROOT_NAME).transform;
                ChildConnectorsRootTrans.transform.parent = this.transform;
                ChildConnectorsRootTrans.localPosition=Vector3.zero;
                ChildConnectorsRootTrans.localEulerAngles=Vector3.zero;

                ChildConnectors = new Connector[offsetTransform.Length];
                for (var i = 0; i < ChildConnectors.Length; i++)
                {
                    ChildConnectors[i] = Instantiate(ConnectorPrefabTemplate).GetComponent<Connector>();
                    ChildConnectors[i].transform.parent = ChildConnectorsRootTrans;

                    ChildConnectors[i].transform.localPosition = offsetTransform[i].localPosition;
                    ChildConnectors[i].transform.localEulerAngles = offsetTransform[i].localEulerAngles;

                    ChildConnectors[i].Host = this;
                    ChildConnectors[i].UseFullConnector = false;
                    ChildConnectors[i].ShowSprite = false;
                    ChildConnectors[i].ConnectorID = i;
                    //ChildConnectors[i].ConnectorHovered += SomeChildrenConnectHovered;
                }

                Connector.ConnectorHovered += SomeChildrenConnectHovered;
            }
        }

        protected void UpdateConnectableAndChildPos()
        {
            if (!IsRootConnectable)
            {
                System.Diagnostics.Debug.Assert(ParentConnector != null, nameof(ParentConnector) + " != null");

                Vector3 parentConnectionRot;
                if (ParentConnector.UseFullConnector)
                {
                    parentConnectionRot = ParentConnector.OtherConnector.transform.eulerAngles;
                }
                else
                {
                    parentConnectionRot = ParentConnector.OtherConnector.transform.eulerAngles + new Vector3(0, 0, 180);
                }

                Vector3 localRotOffset = ParentConnector.transform.eulerAngles - this.transform.localEulerAngles;
                this.transform.eulerAngles = parentConnectionRot - localRotOffset;

                Vector3 parentConnectionPos = ParentConnector.OtherConnector.transform.position;
                Vector3 localPosOffset = ParentConnector.transform.position - this.transform.position;
                this.transform.position = parentConnectionPos - localPosOffset;
            }

            if (ChildConnectors?.Length > 0 && AllowSubConnection)
            {
                foreach (var childConnector in ChildConnectors)
                {
                    childConnector.Device?.UpdateConnectableAndChildPos();
                }
            }
        }


        public virtual void OnMouseEnter()
        {
            if (IsRootConnectable)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (GameManager.DraggingConnectable != this)
                    {
                        Connector.ConnectConnector(this.ChildConnectors[0],GameManager.DraggingConnectable.ParentConnector);
                        GameManager.DraggingConnectable = null;
                    }
                }
            }
            else
            {
                //TurnOnDisplayConnector();
            }
        }

        public virtual void OnMouseDrag()
        {
            if (GameManager.DraggingConnectable == null || GameManager.DraggingConnectable == this)
            {
                if (GameManager.DraggingConnectable == null)
                {
                    _mouseDragOffset = this.transform.position - GetMouseWorldPos;
                    GameManager.DraggingConnectable = this;
                    //MainCollider2D.enabled = false;
                }
                this.transform.position = GetMouseWorldPos + _mouseDragOffset;
            }
        }

        public virtual void OnMouseUp()
        {
            if (GameManager.DraggingConnectable == this)
            {
                GameManager.DraggingConnectable = null;
                _mouseDragOffset = Vector3.zero;
                //MainCollider2D.enabled = true;
            }
        }

        public virtual void OnDisable()
        {
            Connector.ConnectorHovered -= SomeChildrenConnectHovered;
        }
    }

}