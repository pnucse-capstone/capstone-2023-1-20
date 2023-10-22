using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public struct TimeSignature
{
    public int numerator;
    public int denominator;
    public static TimeSignature[] GetAll()
    {
        TimeSignature[] arr = new TimeSignature[]
        {
            new TimeSignature(4, 4),
            new TimeSignature(3, 4),
            new TimeSignature(2, 4),
        };
        return arr;
    }
    public TimeSignature(int numerator, int denominator) 
    { 
        this.numerator = numerator;
        this.denominator = denominator;
    }

    public TimeSignature(string signature)
    {
        numerator = int.Parse(signature.Trim().Split('/')[0]);
        denominator = int.Parse(signature.Trim().Split('/')[1]);
    }
    public override string ToString()
    {
        return $"{numerator}/{denominator}";
    }
}
public class BPMSetting : MonoBehaviour
{
    [SerializeField]
    InputField field;
    [SerializeField]
    Dropdown dropdown;

    void Start()
    {
        var str = TimeSignature.GetAll().First((x) => x.numerator == Game.table.timeSignatureNumerator).ToString();
        dropdown.value =  dropdown.options.FindIndex((x)=>x.text==str);
    }
    public void Abs(string str)
    {
        if (str.StartsWith("-")) { field.text = ""; }
    }
    public void OnTimeSignatureChanged(int value)
    {
        string str = dropdown.options[value].text;
        var signature = new TimeSignature(str);
        Game.table.SetTimeSignature(signature);

    }
}
