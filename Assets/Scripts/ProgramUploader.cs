using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramUploader : MonoBehaviour
{
    public GameManager Game;
    public InputField Code;

    private RogramCompiler compiler = new RogramCompiler();

    void Start()
    {
        this.Code.text = "" +
            "call move_forward 2\n" +
            "call water"
        ;
    }

    public void OnUpload()
    {
        Debug.Log("Uploading...");
        if (Code != null && Game != null && Game.SelectedRobot != null)
        {
            var code = Code.text;
            Debug.Log("Compiling..." + code);

            var rogram = compiler.Compile(code);

            var cpu = Game.SelectedRobot.GetComponentInChildren<CpuBehavior>();
            if (cpu != null)
            {
                cpu.Load(rogram);
            }
        }
    }
}
