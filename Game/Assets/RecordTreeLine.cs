using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RecordTreeLine : MonoBehaviour
{

    [SerializeField]
    GameObject Entry;
    List<RecordTreeEntry> entries = new List<RecordTreeEntry>();
    public void SetList(List<ItemWrapper> items)
    {

        foreach (var song in items)
        {
            var e = Instantiate(Entry, transform);
            var packEntry = e.GetComponentInChildren<RecordTreeEntry>();

            var zip = new ZipUtility(song.Directory);
            byte[] bytes = zip.ReadEntry("meta.json");
            var table = JsonUtility.FromJson<TableMetaData>(Encoding.Default.GetString(bytes));

            packEntry.Set(song, table);

            entries.Add(packEntry);

        }

    }

}
