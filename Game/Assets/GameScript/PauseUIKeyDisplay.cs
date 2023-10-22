using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIKeyDisplay : MonoBehaviour
{
    [SerializeField]
    Text txt;
    // Start is called before the first frame update
    void Start()
    {
        StringBuilder builder= new StringBuilder();
        builder.AppendLine($"Speed: {KeySetting.keySpeeddown},{KeySetting.keySpeedup}");
        builder.AppendLine($"Restart:{KeySetting.keyRestart}");
        builder.AppendLine($"Judge Point:{KeySetting.keyJudgeDown},{KeySetting.keyJudgeUp}");
        builder.AppendLine($"UI:{KeySetting.keyJudgeUIDown},{KeySetting.keyJudgeUIUp}");
        txt.text = builder.ToString();
//            $"Lane Cover:{KeySetting.keyCoverup},{KeySetting.keyCoverdown}\n";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
