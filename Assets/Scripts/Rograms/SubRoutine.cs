using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SubRoutine
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public RobotPart Owner { get; set; }

    public Action<int[], Action> Fn;
}
