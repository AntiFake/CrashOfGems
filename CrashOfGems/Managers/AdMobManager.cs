using UnityEngine;
using System.Collections;
using admob;

public class AdMobManager : MonoBehaviour
{
    //public static AdMobManager Instance { set; get; }

    private void Start()
    {
        //Instance = this;
        //DontDestroyOnLoad(gameObject);

        Admob.Instance().initAdmob("позвони мне", "позвони");
        Admob.Instance().showBannerRelative(AdSize.SmartBanner, AdPosition.BOTTOM_CENTER, 0);
    }
}
