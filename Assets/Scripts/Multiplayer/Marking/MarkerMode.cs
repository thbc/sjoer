using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.HelperClasses;
using Assets.Resources;
using Assets.InfoItems;
using Assets.SceneManagement;

namespace Multiplayer.Marking
{

    public class MarkerMode : MonoBehaviour
    {
        /* MarkerMode explained:

        client 1:
        If markmode is enabled and a user interacts with an InfoItem, the InfoItem.cs class calls "SendMarker()",
        which send the name of the vessel to be marked to client 2.

        client 2:
        The OSC Receiver calls on MarkerMode "MarkItem(GameObject)", which:
            *invokes a click on the responding InfoItem,
            *marks it with the yellow material,
            *adds its name to a string List "recvMarkedVessels",
            *changes the gameObject's tag to "MARKED-received"

        In InfoItem, if allowMarking is enabled and an gameObject is tagged as "MARKED-received", we call the MarkerMode "UnmarkItem(GameObject)" which:
            *resets the gameObject tag to "Untagged"
            *resets its material to the default one
            *removes its name from the recvMarkedVessels List

        The recvMarkedVessels List serves primarily for the OSCReceiver which checks OnMarkReceived whether an object is already marked..
         */
        public static MarkerMode Instance { get; private set; }


        public Material mat_unmarked;
        public Material mat_markedCombined;
        public Material mat_markedSent;
        public Material mat_markedReceived;


        [HideInInspector]
        public bool allowMarking = true;

        public OSCSender sender;


        public DefaultScene defaultScene;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }



        // Method to get the assigned material
        public Material GetDefaultMaterial()
        {
            return mat_unmarked;
        }
        public Material GetMarkedReceivedMaterial()
        {
            return mat_markedReceived;
        }

        public Material GetMarkedSentMaterial()
        {
            return mat_markedSent;
        }
        public Material GetMarkedCombinedMaterial()
        {
            return mat_markedCombined;
        }


        public bool SetMarkMode()
        {
            allowMarking = !allowMarking;
            Debug.Log("allowMarking: " + allowMarking);
            StoreMarkMode();

            return allowMarking;
        }
        public void SetMarkMode(bool state)
        {
            allowMarking = state;
            Debug.Log("allowMarking: " + allowMarking);
            StoreMarkMode();
        }
        void StoreMarkMode()
        {
            ConnectionController.Instance.playerConfig.sendMarkerMultiplayer = allowMarking;
            if (allowMarking)
                PlayerPrefs.SetInt("MultiplayerMarkMode", 1);
            else if (!allowMarking)
                PlayerPrefs.SetInt("MultiplayerMarkMode", 0);

            PlayerPrefs.Save();

        }

        /* 
            #region This handling the incoming marker, and detecting when the recipient closes it again
            public void OnMarkItemReceived(string s)//(GameObject vesselGO)
            {
                // return if vessel is already marked
                if (InMarkedReceivedList(s)) return;
                s = s + " (HorizonPlane)";
                Debug.Log("received mark: " + s);
                GameObject vesselGO;
                if (GameObject.Find(s))
                {
                    vesselGO = GameObject.Find(s);
                    if (vesselGO.tag == "MARKED-received") return;

                    MarkItemReceived(vesselGO);
                }
                else
                {   // this logic makes sure both expanded and collapses markers are found
                    s = "[T] " + s;
                    if (GameObject.Find(s))
                    {
                        vesselGO = GameObject.Find(s);
                        if (vesselGO.tag == "MARKED-received") return;

                        MarkItemReceived(vesselGO);
                    }
                }
            }
            void MarkItemReceived(GameObject vesselGO)
            {
                Debug.LogWarning("found: " + vesselGO.name);
                vesselGO.gameObject.tag = "MARKED-received";
                vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function
                vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedMaterial();
                AddVesselMarkedReceivedList(vesselGO.name);
            }

            #endregion

            #region Interaction with unmarking items





            /// <summary>
            /// Triggered from interaction with InfoItem. Called if object was a sent marker.
            /// </summary>
            /// 
            public void UnmarkSentItem(GameObject vesselGO)
            {
                Debug.LogWarning("UNMARK: " + vesselGO.name);
                vesselGO.gameObject.tag = "Untagged";
                vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetAssignedMaterial();
                RemoveGETVesselMarkedSentList(vesselGO.name);

                //TODO: send and notify Unmark to other player
                sender.SendUnmarked(vesselGO.name);
                //TODO2: trigger collapse --> is done already in InfoItem, but still needs to be done on-received

            }
            /// <summary>
            /// Triggered from interaction with InfoItem. Called if object was a received marker.
            /// </summary>
            /// 
            public void UnmarkReceivedItem(GameObject vesselGO)
            {
                Debug.LogWarning("UNMARK: " + vesselGO.name);
                vesselGO.gameObject.tag = "Untagged";
                RemoveVesselMarkedReceivedList(vesselGO.name);

                //TODO: send and notify Unmark to other player
                sender.SendUnmarked(vesselGO.name);
                //TODO2: trigger collapse --> is done already in InfoItem, but still needs to be done on-received
            }
            #endregion

            #region Sending
            /// <summary>
            /// Handling the sending of marker to other player and also marking it for the sender-client and adds to list for keeping track.
            /// Called from InfoItem if allowMarking is true.
            /// </summary>
            public void SendMarker(string _vesselName)
            {
                if (!sender.osc.isInitialized) //Should send Marker but OSC is not initialized yet..
                    return;

                //Debug.LogWarning("Send MARKER: " + _vesselName);
                sender.SendMarked(_vesselName);

                OnMarkItemSent(_vesselName);    // mark as well for user who is sending the mark
            }
            public void OnMarkItemSent(string s)
            {
                // return if vessel is already marked
                if (InMarkedSentList(s)) return;
                s = s + " (HorizonPlane)";
                //Debug.Log("sending and marking mark: " + s);
                GameObject vesselGO;
                if (GameObject.Find(s))
                {
                    vesselGO = GameObject.Find(s);
                    if (vesselGO.tag == "MARKED-sent") return;

                    MarkItemSent(vesselGO);
                }
                else
                {   // this logic makes sure both expanded and collapses markers are found
                    s = "[T] " + s;
                    if (GameObject.Find(s))
                    {
                        vesselGO = GameObject.Find(s);
                        if (vesselGO.tag == "MARKED-sent") return;

                        MarkItemSent(vesselGO);
                    }
                }
            }

            public void MarkItemSent(GameObject vesselGO)
            {
                Debug.LogWarning("found: " + vesselGO.name);
                vesselGO.gameObject.tag = "MARKED-sent";
                //    vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function
                vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedSentMaterial();
                AddVesselMarkdSentList(vesselGO);
            }

            public void OnUnmarkItemReceived(string s)
            {
                // !!! if unmarking via OSC is not working, try uncommenting this line
                if (!InMarkedSentList(s) && !InMarkedReceivedList(s)) return;
                // if (!InMarkedSentList(s)) return;
                s = s + " (HorizonPlane)";
                //Debug.Log("sending and marking mark: " + s);
                GameObject vesselGO;
                if (GameObject.Find(s))
                {
                    vesselGO = GameObject.Find(s);
                    if (vesselGO.tag == "MARKED-sent")
                        ItemSent_receivedUnmark(vesselGO);
                    else if (vesselGO.tag == "MARKED-received")
                        ItemReceived_receivedUnmark(vesselGO);
                }
                else
                {   // this logic makes sure both expanded and collapses markers are found
                    s = "[T] " + s;
                    if (GameObject.Find(s))
                    {
                        vesselGO = GameObject.Find(s);
                        if (vesselGO.tag == "MARKED-sent")
                            ItemSent_receivedUnmark(vesselGO);
                        else if (vesselGO.tag == "MARKED-received")
                            ItemReceived_receivedUnmark(vesselGO);
                    }
                }
            }
            /// <summary>
            /// This is called when osc receives "unmarking" from other player.
            /// Only called if object was type of "received". Although when this player unmarks this object, that is handled in InfoItem,
            /// we still need this function for the case that th other player sent this marker earlier and unmarked it now.
            /// </summary>
            /// <param name="vesselGO"></param>
            void ItemReceived_receivedUnmark(GameObject vesselGO)
            {
                Debug.LogWarning("UNMARK: " + vesselGO.name);
                vesselGO.gameObject.tag = "Untagged";
                vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetAssignedMaterial();
                RemoveVesselMarkedReceivedList(vesselGO);
            }
            /// <summary>
            /// This is called when osc receives "unmarking" from other player.
            /// Only called if object was type of "received".
            /// </summary>
            /// <param name="vesselGO"></param>
            void ItemSent_receivedUnmark(GameObject vesselGO)
            {
                Debug.LogWarning("UNMARK: " + vesselGO.name);
                vesselGO.gameObject.tag = "Untagged";
                vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetAssignedMaterial();
                RemoveGETVesselMarkedSentList(vesselGO);
            }
            #endregion
           */
        // called from OSCReceiver
        /*     void MarkItem(GameObject vesselGO)
            {
                Debug.LogWarning("found: " + vesselGO.name);
                vesselGO.gameObject.tag = "MARKED-received";
                vesselGO.GetComponent<TargettableInfoItem>().OnClick(); // trying to open item through OnClick function

                vesselGO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = GetMarkedMaterial();

                AddVesselMarkedReceivedList(vesselGO.name);
            } */



        internal void MarkSend(InfoItem _info)
        {
            Debug.LogWarning("MarkSend called..");

            GameObject _vesselGO = _info.Shape;
            _info.SetState_MarkedSent(true);

            //defaultScene.AddVesselMarkedSentList(_info);


            if (_info.IsMarked_Received())
                SetMaterial_markedCombined(_vesselGO);
            else
                SetMaterial_markedSent(_vesselGO);

            sender.Send_Marked(_info.Key);
        }

        internal void UnmarkSent(InfoItem _info) //wasReceived is true when "unmarking sent items" is received from network via OSC
        {
            Debug.LogWarning("UnmarkSent called..");

            GameObject _vesselGO = _info.Shape;
            _info.SetState_MarkedSent(false);

            if (_info.IsMarked_Received())
                SetMaterial_markedReceived(_vesselGO);
            else
                SetMaterial_unmarked(_vesselGO);
            //send “UnmarkSent” to client --> trigger

            sender.Send_UnmarkedSent(_info.Key);
        }

        internal void UnmarkReceived(InfoItem _info) //wasReceived is true when "unmarking received items" is received from network via OSC
        {
            Debug.LogWarning("UnmarkReceived called..");

            GameObject _vesselGO = _info.Shape;

            _info.SetState_MarkedReceived(false);


            if (_info.IsMarked_Sent())
                SetMaterial_markedSent(_vesselGO);
            else
                SetMaterial_unmarked(_vesselGO);

            sender.Send_UnmarkedReceived(_info.Key);
        }

        // these are the OSC received commands
        public void OnReceivedUnmark_receivedMarkers(string _vesselName)
        {
            InfoItem _info = defaultScene.GetVesselInfoItem(_vesselName);
            GameObject _vesselGO = _info.Shape;

            if (!_info.IsMarked_Received())
                Debug.LogWarning("getting unmark received markers, but element was not marked as received... investigate?");


            _info.SetState_MarkedReceived(false);


            if (_info.IsMarked_Sent())
                SetMaterial_markedSent(_vesselGO);
            else
                SetMaterial_unmarked(_vesselGO);


        }

        // HERE IS A BUG!! --> _GO is null when receiveing unmark from client
        public void OnReceivedUnmark_sentMarkers(string _vesselName)
        {
            InfoItem _info = defaultScene.GetVesselInfoItem(_vesselName);
            if(_info == null)
            {
                Debug.LogWarning("_info item for: "+ _vesselName + " is null");
            return;
            }
            GameObject _vesselGO = _info.Shape;
            if (!_info.IsMarked_Sent())
                Debug.LogWarning("getting unmark sent markers, but element was not marked as sent... investigate?");

            _info.SetState_MarkedSent(false);

            if (_info.IsMarked_Received())
                SetMaterial_markedReceived(_vesselGO);
            else
                SetMaterial_unmarked(_vesselGO);
        }

        public void OnReceivedMark_receivedMarkers(string _vesselName)
        {
            InfoItem _info = defaultScene.GetVesselInfoItem(_vesselName);
            GameObject _vesselGO = _info.Shape;
            if (_info.IsMarked_Received())
                Debug.LogWarning("getting mark received markers, but element was already marked as received... investigate?");


            // here we have to find the gameObject inside the scene
            // let s make sure we find a "HorizonPlane" instead of SkyArea obj
            // ...
            _info.SetState_MarkedReceived(true);
            if (_info.IsMarked_Sent())
                SetMaterial_markedCombined(_vesselGO);
            else
                SetMaterial_markedReceived(_vesselGO);
        }


        #region Handle TargettableInfoItem's material based on marked-status
        public void SetMaterial_unmarked(GameObject _gO)
        {
            _gO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = mat_unmarked;
        }
        public void SetMaterial_markedReceived(GameObject _gO)
        {
            _gO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = mat_markedReceived;
        }
        public void SetMaterial_markedSent(GameObject _gO)
        {
            _gO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = mat_markedSent;
        }
        public void SetMaterial_markedCombined(GameObject _gO)
        {
            _gO.transform.Find($"StickAnchor/Stick/PinAnchor/AISPinTarget/default").gameObject.GetComponent<MeshRenderer>().material = mat_markedCombined;
        }
        #endregion

        /*  void AddVessel(List<GameObject> _vesselList, GameObject _vesselGO)
         {
             if (!_vesselList.Contains(_vesselGO))
                 _vesselList.Add(_vesselGO);
         }
         void RemoveVessel(List<GameObject> _vesselList, GameObject _vesselGO)
         {
             if (!_vesselList.Contains(_vesselGO))
                 _vesselList.Remove(_vesselGO);
         } */
        //public List<GameObject> recvMarkedVessels = new List<GameObject>();
        //public List<GameObject> sentMarkedVessels = new List<GameObject>();
        #region Dictionaries for received and sent Vessels



        /*  private Dictionary<string, GameObject> recvMarkedVessels = new Dictionary<string, GameObject>();
         private Dictionary<string, GameObject> sentMarkedVessels = new Dictionary<string, GameObject>();

         public bool InMarkedReceivedList(string key)
         {
             return recvMarkedVessels.ContainsKey(key);
         }

         public GameObject GetVesselMarkedReceivedList(string key)
         {
             recvMarkedVessels.TryGetValue(key, out var vesselGO);
             return vesselGO;
         }

         void AddVesselMarkedReceivedList(string key, GameObject _vesselGO)
         {
             if (!recvMarkedVessels.ContainsKey(key))
             {
                 recvMarkedVessels.Add(key, _vesselGO);
             }
         }
         public void RemoveVesselMarkedReceivedList(string key)
         {
             recvMarkedVessels.Remove(key);
         } 
         GameObject RemoveGETVesselMarkedReceivedList(string key)
         {
             var _GO = GetVesselMarkedReceivedList(key);

             recvMarkedVessels.Remove(key);
             return _GO;
         }

         public bool InMarkedSentList(string key)
         {
             return sentMarkedVessels.ContainsKey(key);
         }
         public GameObject GetVesselMarkedSentList(string key)
         {
             foreach (var item in sentMarkedVessels)
             {
                 Debug.LogWarning(item.Key + ": "+item.Value.name);
             }
             sentMarkedVessels.TryGetValue(key, out var vesselGO);
             return vesselGO;
         }
         void AddVesselMarkdSentList(string key, GameObject _vesselGO)
         {
             if (!sentMarkedVessels.ContainsKey(key))
             {
                 sentMarkedVessels.Add(key, _vesselGO);
             }
         }

         GameObject RemoveGETVesselMarkedSentList(string key)
         {

             var _GO = GetVesselMarkedSentList(key);
             sentMarkedVessels.Remove(key);
             return _GO;
         }
  */


        #endregion

        #region Finding / retrieving the correct gameObject
        TargettableInfoItem FindTargettableInfoItem(string s)
        {
            s = s + " (HorizonPlane)";
            GameObject vesselGO;
            if (GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);

                // MarkItemReceived(vesselGO);
                return vesselGO.GetComponent<TargettableInfoItem>();
            }
            else
            {   // this logic makes sure both expanded and collapses markers are found
                s = "[T] " + s; // T ==> target
                if (GameObject.Find(s))
                {
                    vesselGO = GameObject.Find(s);

                    //   MarkItemReceived(vesselGO);
                    return vesselGO.GetComponent<TargettableInfoItem>();
                }
                else
                {
                    Debug.LogWarning("could not find the Marked/Unmarked GameObject with name: " + s);
                    return null;
                }
            }
        }

        TargettableInfoItem FindTargettableInfoItem_old(string s)
        {
            var t = FindGameObject(s)?.GetComponent<TargettableInfoItem>();
            if (t != null)
                return t;
            else
            {
                Debug.LogWarning("could not find the TargettableInfoItem for: " + s);
                return null;
            }
        }

        // this is a helper, but we will try another approach
        /// <summary>
        /// This tries to find the GameObject with TargettableInfoItem that the user has interacted with. Queries via name for finding received name from OSC.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        GameObject FindGameObject(string s)
        {
            s = s + " (HorizonPlane)";
            GameObject vesselGO;
            if (GameObject.Find(s))
            {
                vesselGO = GameObject.Find(s);

                // MarkItemReceived(vesselGO);
                return vesselGO;
            }
            else
            {   // this logic makes sure both expanded and collapses markers are found
                s = "[T] " + s; // T ==> target
                if (GameObject.Find(s))
                {
                    vesselGO = GameObject.Find(s);

                    //   MarkItemReceived(vesselGO);
                    return vesselGO;
                }
                else
                {
                    Debug.LogWarning("could not find the Marked/Unmarked GameObject with name: " + s);
                    return null;
                }
            }
        }

        #endregion





    }
}