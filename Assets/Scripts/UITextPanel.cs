using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextPanel : MonoBehaviour {
    [SerializeField]
    private Text m_text;

	// Use this for initialization
	void Start () {
		if (m_text == null) {
            m_text = GetComponentInChildren<Text>();
        }
	}

    public void setText(string text) {
        if (m_text) {
            m_text.text = text;
            if (text == "") {
                gameObject.SetActive(false);
            }
            else {
                gameObject.SetActive(true);
            }
        }
    }
}
