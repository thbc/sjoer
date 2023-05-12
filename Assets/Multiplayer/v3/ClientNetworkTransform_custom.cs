using Unity.Netcode.Components;
using UnityEngine;

 namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{ 

    [DisallowMultipleComponent]
    public class ClientNetworkTransform_custom : NetworkTransform
    {
       
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
 } 