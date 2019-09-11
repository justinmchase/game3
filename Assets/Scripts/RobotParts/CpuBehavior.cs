using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// set the cpu blinker to on

// mov 1 led
// wait 100
// mov 0 led
// wait 100

public class CpuBehavior : MonoBehaviour
{

    private float Wait = 0.0f;

    private Rogram rogram;

    private int index = 0;

    private Dictionary<string, Register> registers = new Dictionary<string, Register>();

    private Material ledMaterial;

    private void Start()
    {
        //this.ledMaterial = GameObject.Instantiate(this.GetComponent<MeshRenderer>().material);
        
        this.AddRegister(new Register
        {
            Name = "led",
            Owner = this.GetComponent<RobotPart>(),
            Value = 0
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (this.rogram != null && this.index < rogram.ops.Length && this.Wait <= 0.0f)
        {
            var op = rogram.ops[this.index];
            switch (op.Name)
            {
                case RogramOpName.noop:
                    break;
                case RogramOpName.wait:
                    var wait = (WaitOp)op;
                    this.Wait = this.GetValue(wait.Time) / 1000.0f;
                    break;
                case RogramOpName.mov:
                    var mov = (MovOp)op;
                    var src = this.GetValue(mov.Src);
                    var dest = this.registers[mov.Dest.Register];
                    dest.Value = src;
                    break;
                default:
                    Debug.Log($"Invalid Op {op}");
                    break;
            }

            this.index = (this.index + 1) % rogram.ops.Length;
        }

        if (this.Wait >= 0.0f)
        {
            this.Wait -= Time.deltaTime;
        }

        var ledOff = this.registers["led"].Value == 0;
        var ledColor = ledOff ? Color.gray : Color.red;
        this.GetComponent<MeshRenderer>().material.color = ledColor;
    }

    private int GetValue(IRogramOpArg arg)
    {
        if (arg.Kind == RogramArgKind.Literal)
        {
            return ((RogramValueArg)arg).Value;
        }
        else
        {
            return this.registers[((RogramRegisterArg)arg).Register].Value;
        }
    }

    public void Load(Rogram rogram)
    {
        this.index = 0;
        this.rogram = rogram;

        Debug.Log("Rogram loaded!");
    }

    public void AddRegister(Register register)
    {
        this.registers.Add(register.Name, register);
    }
}
