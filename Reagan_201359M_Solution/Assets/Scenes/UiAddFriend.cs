using System;
using UnityEngine;

public class UiAddFriend : MonoBehaviour
{
    public string displayName;

    public static Action<string> OnAddFriend = delegate { };

    public void SetAddFriendName(string name)
    {
        displayName = name;
    }

    public void AddFriend()
    {
        if(string.IsNullOrEmpty(displayName))
        {
            return;
        }
        OnAddFriend?.Invoke(displayName);
    }
}
