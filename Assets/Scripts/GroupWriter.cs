using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupWriter {

    private List<string> m_outputlist;

	// Use this for initialization
	public GroupWriter() {
        m_outputlist = new List<string>();
	}

    public void addToQueue(string s){
        m_outputlist.Add(s);
    }

    public string getQueueToString(){
        string str = "";
        if (m_outputlist.Count > 0) {
            for (int i = 0; i < m_outputlist.Count; i++) {
                str += m_outputlist[i] + "\n";
            }
            /* clear the queue */
            m_outputlist.Clear();
        }
        return str.TrimEnd('\n');
    }

    public int getCount() {
        return m_outputlist.Count;
    }
}
