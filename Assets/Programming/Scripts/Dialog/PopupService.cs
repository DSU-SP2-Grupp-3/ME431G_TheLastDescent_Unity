using System.Collections;
using Codice.Client.BaseCommands.Merge;
using UnityEngine;
using UnityEngine.UI;

public class PopupService : Service<PopupService>
{
    private GameObject currentPopup;
    private Animator currentAnimator;
    void Awake()
    {
        Register();
    }
    public IEnumerator Open(GameObject popup)
    {
        if (currentPopup != null) yield return Close();

        currentPopup = Instantiate(popup);
        currentPopup.transform.SetParent(transform, false);

        RectTransform rect = currentPopup.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = new Vector3(0.77f, 0.77f, 0.77f);
        currentAnimator = currentPopup.GetComponent<Animator>();

        currentAnimator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(0.01f);
        yield return new WaitUntil(() => currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
    public IEnumerator Close()
    {
        if (currentPopup == null) yield break;
        GameObject tempPop = currentPopup;
        currentPopup = null;

        Animator tempAni = currentAnimator;
        currentAnimator = null;

        tempAni.SetTrigger("End");
        yield return new WaitForSecondsRealtime(0.01f);
        yield return new WaitUntil(() => tempAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f);

        Destroy(tempPop);
    }
}
