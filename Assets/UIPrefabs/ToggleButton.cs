using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {
    private int m_ptr = 0;
    private Image m_image;
    public Sprite[] m_status;

	// Use this for initialization
	void Start () {
        m_image = GetComponent<Image>();
	}

    public int GetCurrentStatus() {
        return m_ptr;
    }

    public void ChangeToStatus(int status) {
        if (status >= m_status.Length) {
            status = 0;
        }
        if (status < 0) {
            status = 0;
        }
        m_ptr = status;
        if (m_image != null && m_status.Length > 0) {
            m_image.sprite = m_status[m_ptr];
        }
    }

    public void Toggle() {
        ChangeToStatus(m_ptr + 1);
    }
}
