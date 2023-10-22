using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
public class SubsSelectSongInfo : MonoBehaviour
{
    [SerializeField] GameObject icon;
    [SerializeField] Text title;
    [SerializeField] Text composer;
    [SerializeField] Text maker;
    [SerializeField] Text lv;
    [SerializeField] GameObject Official;
    public GameObject levelerComment;
    public IEnumerator IconChange(Sprite sprite)
    {
        float dur = 0.1F;
        float d = Time.deltaTime / dur * 90F;
        for (float i = 0; i < 90; i += d)
        {
            icon.transform.Rotate(new Vector3(0, d, 0));
            yield return null;
        }
        icon.GetComponent<Image>().sprite = sprite;
        for (float i = 0; i < 90; i += d)
        {
            icon.transform.Rotate(new Vector3(0, -d, 0));
            yield return null;
        }
        icon.transform.rotation = Quaternion.identity;

    }
    public void Set(PbrffExtracter.PreviewEntry entry)
    {
        levelerComment.GetComponent<ShowTooltip>().text = entry.description;

        if (string.IsNullOrEmpty(entry.description) || entry.description == "Unknown" || entry.description.StartsWith("Official"))
        {
            levelerComment.GetComponent<Graphic>().enabled = false;
        }
        else
        {
            levelerComment.GetComponent<Graphic>().enabled = true;
        }
        title.text = entry.title;
        composer.text = entry.composer;
        maker.text = entry.leveler;
        if(entry.level>=0 && entry.level <= 10)
        {
            lv.text = "lv." + entry.level;
        }
        else
        {
            lv.text = "lv.?" ;

        }

        Official.SetActive(entry.isOriginal);
        StartCoroutine(IconChange(entry.icon));
    }
}
