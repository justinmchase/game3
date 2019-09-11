using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public RobotBehavior SelectedRobot;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
