using UnityEngine;
using System.Collections;

public class HexEditor : MonoBehaviour {

	private bool markDelete;
	private Color originalColor;

	void Awake () 
	{
		if(!Editor.instance.m_EnabledEditor) 
			this.enabled = false;
		else
			originalColor = this.renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseOver()
	{
		if(this.enabled)
		{
			if(Input.GetMouseButtonDown(0))
			{
				DeleteChild();
				if(!markDelete)
					Editor.instance.LeftClickedHex(this.gameObject);
			}
			if(Input.GetMouseButtonDown(1))
			{
				DeleteChild();
				if(!markDelete)
				{
					markDelete = true;
					Color deleteColor = this.renderer.material.color;
					deleteColor = Color.red;
					deleteColor.a = 0.1f;
					this.renderer.material.color = deleteColor;
					Editor.instance.RightClickHex(this.gameObject);
				}
				else
				{
					markDelete = false;
					this.renderer.material.color = originalColor;
				}
			}
		}

	}

	void DeleteChild()
	{
		if(this.transform.childCount > 0)
		{
			Transform old = this.transform.GetChild(0);
			Destroy(old.gameObject);
		}
	}
}
