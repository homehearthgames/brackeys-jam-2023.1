using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPortal : MonoBehaviour
{
    public void StartPortalOpen()
    {
        CharacterManager.SpawnMe(transform.position);
        CharacterManager.instance.me._active = true;
    }
}
