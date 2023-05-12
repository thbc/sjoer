using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerNetwork : NetworkBehaviour
{
    public TextMesh tm;
    private NetworkVariable<int> randomNum = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    void Update()
    {
        if (transform.position.z == 0 && !IsOwner)
            transform.position = new Vector3(0,0, 1.43f);
        if (!IsOwner) return;
       
        if (transform.position.z == 0 )
            transform.position = new Vector3(-1, 0, 1.43f);

        randomNum.Value = randomNum.Value++;
        tm.text = randomNum.Value.ToString();
        
    }
}
