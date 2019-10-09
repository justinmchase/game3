using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{

    private bool isWet = false;
    public bool IsWet {  get { return this.isWet; } }

    public Material Default;
    public Material Wet;

    private Renderer _renderer;

    public void Water()
    {
        this.isWet = true;
        if (this.Wet != null)
        {
            this._renderer.material = this.Wet;
        }
    }

    void Start()
    {
        this._renderer = this.GetComponent<Renderer>();
    }
}
