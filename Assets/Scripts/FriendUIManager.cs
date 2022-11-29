using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendUIManager : MonoBehaviour
{
    public GameObject textUI;
    public Animator animator;

    public void UpdateFriendCount(int totalFriendCount)
    {
        textUI.GetComponent<TextMeshProUGUI>().text = totalFriendCount.ToString();
        StartCoroutine("UIShow");
    }

    private IEnumerator UIShow()
    {
        animator.SetBool("Show", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Show", false);
    }

}
