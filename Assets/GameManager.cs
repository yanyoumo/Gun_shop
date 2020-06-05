using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GunShop
{
    public class GameManager : MonoBehaviour
    {
        public Connectable host;
        public Connectable device;
        public Connectable device_sub;
        public Connectable device_sub1;
        // Start is called before the first frame update

        public static Connectable DraggingConnectable;

        public static Connector DraggingParentConnector
        {
            get
            {
                Debug.Assert(DraggingConnectable!=null);
                return DraggingConnectable?.ParentConnector;
            }
        }

        public static bool DraggingAnyConnectable => (DraggingConnectable != null);

        public static bool DraggingConnectableNotNullNeitherSelf(Connectable self)
        {
            if (DraggingConnectable != null)
            {
                if (DraggingConnectable != self)
                {
                    return true;
                }
            }
            return false;
        }

        void Start()
        {
            Connector.ConnectConnector(host.ChildConnectors[0], device.ParentConnector);
//            Connector.ConnectConnector(device.ChildConnectors[0], device_sub.ParentConnector);
//            Connector.ConnectConnector(device.ChildConnectors[1], device_sub1.ParentConnector);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}