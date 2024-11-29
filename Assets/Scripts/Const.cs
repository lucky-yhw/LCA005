using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    public const int GoldRate = 10000;
    public const int HeadMax = 30;
    public const int NameMax = 20;
    public const int DescriptionMax = 200;
    public const int WinGiftGold = 200000;

    public static readonly IReadOnlyList<string> ProductIdList = new List<string>()
    {
        "coin_test_1", "coin_test_2", "coin_test_3"
    };

    public static readonly IReadOnlyDictionary<string, long> InAppPurchaseId2Coins = new Dictionary<string, long>()
    {
        {"coin_test_1", 20000000},
        {"coin_test_2", 40000000},
        {"coin_test_3", 80000000}
    };
    
    public const string PrivacyPolicy = "https://app.kakkoonline.com/#/privacyAgreement";
    public const string TermOfUse = "https://app.kakkoonline.com/#/usersAgreement";
}