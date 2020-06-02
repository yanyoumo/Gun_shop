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
        void Start()
        {
            Connector.ConnectConnector(host.ChildConnectors[0], device.ParentConnector);
            Connector.ConnectConnector(device.ChildConnectors[0], device_sub.ParentConnector);
            Connector.ConnectConnector(device.ChildConnectors[1], device_sub1.ParentConnector);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}