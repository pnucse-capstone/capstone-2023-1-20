using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KeyGrid : MonoBehaviour // 키음 팔레트를 보여주고 선택하고, 선택된 키음의 id를 now_id로 노출
{
    List<GameObject> list;
    [SerializeField]GameObject prefab;
    [SerializeField] InputField input_name;
    [SerializeField] InputField input_start;
    [SerializeField] InputField input_finish;
    [SerializeField] Toggle toggle_loop;

    [SerializeField] Button default_button;
    // Start is called before the first frame update
    void Start()
    {
        list = new List<GameObject>();

    }
    public int now_id = 0;
    public void Select(SoundInfo info)
    {
        if (info == null)
        {
            now_id = -1;
            input_name.text = "";
            input_start.text = "";
            input_finish.text = "";
            toggle_loop.isOn = false;
            input_name.interactable = false;
            input_start.interactable = false;
            input_finish.interactable = false;
            toggle_loop.interactable = false;
            return;
        }
        now_id = info.id;
        KeySoundPallete.FMODPlay(info.id);
        input_name.text = info.name;
        input_name.interactable = now_id != 0;
        toggle_loop.interactable = now_id != 0;
    }
    public void SelectDefaultButton() 
    {
        Select(KeySoundPallete.default_info);
    }
    // Update is called once per frame
    void Update()
    {

        while (list.Count < KeySoundPallete.Count)
        {
            var temp = Instantiate(prefab, transform);
            temp.GetComponent<Button>().onClick.AddListener(() => {

                Select(temp.GetComponent<KeySoundEntry>().getInfo());

            });
            list.Add(temp);
        }
        var pairs = KeySoundPallete.dic;
        for (int i = 0; i < list.Count; i++)
        {
            if (i >= KeySoundPallete.Count)
            {
                list[i].SetActive(false);
                continue;
            }
            else
            {
                list[i].SetActive(true);
            }
            list[i].GetComponent<KeySoundEntry>().Set(pairs[i]);
            if (now_id == -1)
            {
                list[i].GetComponent<Button>().interactable = true;
            }
            else 
            {
                list[i].GetComponent<Button>().interactable = now_id != pairs[i].id;
            } 
        }
        default_button.GetComponent<Button>().interactable = now_id != 0;

    }
}
