using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using GoogleARCore.Examples.Common;

public class UIController : MonoBehaviour {
    public Portalble.PortalbleGeneralController m_PortalbleController;

    public GameObject m_startButton;
    public GameObject m_endButton;

    public GameObject m_GameButton;
    public GameObject m_GameLayer;

    //bools
    private bool m_game_show = false;
    private bool m_start_show = true;

    //planes
    private GameObject[] m_planes;

    void Start() {
        if (m_PortalbleController == null) {
            m_PortalbleController = FindObjectOfType<Portalble.PortalbleGeneralController>();
        }
    }

    public void toggleGame()
    {
        m_game_show = !m_game_show;

        m_GameLayer.SetActive(m_game_show);
    }

    public void toggleStartEnd() {
        m_start_show = !m_start_show;
        m_endButton.SetActive(!m_start_show);
        m_startButton.SetActive(m_start_show);
    }

    public void togglePlaneMesh() {
        GlobalStates.isGridVisible = !GlobalStates.isGridVisible;

        m_planes = GameObject.FindGameObjectsWithTag("PlaneGeneratedByARCore");
    }


    public void SwitchMultimodal(bool check) {
        GlobalStates.isShift = check;
    }

    public void simpleSwitchButton(Image button) {
        if (button.color == Color.white) {
            button.color = Color.grey;
        }
        else {
            button.color = Color.white;
        }
    }

    public void ToggleARPlaneVisibility() {
        if (m_PortalbleController != null) {
            m_PortalbleController.planeVisibility = !m_PortalbleController.planeVisibility;
        }
    }

    public void ToggleVibration() {
        if (m_PortalbleController != null) {
            m_PortalbleController.UseVibration = !m_PortalbleController.UseVibration;
        }
    }

    public void ToggleGrabHighLight() {
        if (m_PortalbleController != null) {
            m_PortalbleController.GrabHighLight = !m_PortalbleController.GrabHighLight;
        }
    }

    public void ToggleHandAction() {
        if (m_PortalbleController != null) {
            m_PortalbleController.HandActionRecogEnabled = !m_PortalbleController.HandActionRecogEnabled;
        }
    }
}
