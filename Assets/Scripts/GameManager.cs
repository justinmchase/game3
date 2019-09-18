using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public RobotBehavior SelectedRobot;

    public bool Running = true;
    public int TicksPerSecond = 10;

    private List<ITickable> tickables = new List<ITickable>();

    public void Start()
    {
        this.StartCoroutine(this.UpdateCircuits());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics
                .RaycastAll(ray)
                .Select(h => h.transform.GetComponentInParent<RobotBehavior>())
                .Where(r => r != null);

            if (hits.Any())
            {
                this.SelectedRobot = hits.First();
            }
        }

        if (TicksPerSecond > 100)
        {
            this.TicksPerSecond = 100;
        }
        if (TicksPerSecond < 1)
        {
            this.TicksPerSecond = 1;
        }
    }

    public void Register(ITickable tickable)
    {
        this.tickables.Add(tickable);
    }

    private IEnumerator UpdateCircuits()
    {
        while (true)
        {
            var s = 1.0f / this.TicksPerSecond;
            yield return new WaitForSeconds(s);

            if (this.Running)
            {
                foreach (var tickable in this.tickables)
                {
                    tickable.Tick();

                    //yield return null;
                }
            }
        }
    }
}
