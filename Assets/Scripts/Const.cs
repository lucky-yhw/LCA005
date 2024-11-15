using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    public const int GoldRate = 10000;
    public const int HeadMax = 10;
    public const int NameMax = 20;
    public const int DescriptionMax = 200;
    public const int WinGiftGold = 200000;

    public static readonly IReadOnlyList<string> ProductIdList = new List<string>()
    {
        "test1", "test2", "test3"
    };

    public static readonly IReadOnlyDictionary<string, long> InAppPurchaseId2Coins = new Dictionary<string, long>()
    {
        {"test1", 20000},
        {"test2", 40000},
        {"test3", 80000}
    };
}