using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
	[SerializeField]
	Player player;
	[SerializeField]
	string propName;
	string maxPropName;
	Scrollbar sb;

    // Start is called before the first frame update
    void Start()
    {
		sb = GetComponent<Scrollbar>();
		maxPropName = "max" + propName;
    }

    // Update is called once per frame
    void Update()
    {
		sb.size = (float)player.GetProp(propName) / (float)player.GetProp(maxPropName);
    }
}
