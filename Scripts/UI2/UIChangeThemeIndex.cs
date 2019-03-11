using System.Collections;
using System.Collections.Generic;
using MapEditor;
using UnityEngine;

public class UIChangeThemeIndex : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    public int changeValue;

    public void OnClick()
    {
        if (UISelectTheme.ins != null)
        {
            UISelectTheme.ins.ChangeSelectTheme(changeValue);
        }
    }
}
