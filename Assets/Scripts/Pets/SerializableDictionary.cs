using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public struct NamedPriceLabels
{
    public string name;
    public UILabel label;
}

[Serializable]
public struct PriceToKey
{
    public string name;
    public int price;
}

[Serializable]
public struct BoolToKey
{
    public string name;
    public bool status;
}
