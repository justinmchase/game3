using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// set the cpu blinker to on

// mov 1 led
// wait 100
// mov 0 led
// wait 100

public class CpuBehavior : MonoBehaviour, ITickable
{

    private float Wait = 0;

    private Rogram rogram;

    private int index = 0;

    private Dictionary<string, Register> registers = new Dictionary<string, Register>();

    private void Start()
    {
        this.AddRegister(new Register
        {
            Name = "led",
            Owner = this.GetComponent<RobotPart>(),
            Value = 0
        });

        GameObject
            .FindObjectOfType<GameManager>()
            .Register(this);
    }

    // Update is called once per frame
    public void Tick()
    {
        if (this.Wait > 0)
        {
            this.Wait--;
            return;
        }

        if (this.rogram != null && this.index < rogram.ops.Length)
        {
            var op = rogram.ops[this.index];
            switch (op.Name)
            {
                case RogramOpName.noop:
                    break;
                case RogramOpName.wait:
                    var wait = (WaitOp)op;
                    this.Wait = this.GetValue(wait.Time);
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
    }

    public void Update() {
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
