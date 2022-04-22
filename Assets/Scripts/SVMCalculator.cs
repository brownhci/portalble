using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Linq;
using System;
using UnityEngine.UI;

/* Struct to store decision function parameters*/
public struct svm_param
{
    public int sv_count;
    public double rho;
    public double[] alpha;
    public int[] index;
}

public class SVMCalculator : MonoBehaviour
{
    // Start is called before the first frame update
    public List<double[]> support_vectors = new List<double[]>();
    public List<svm_param> decision_functions_info = new List<svm_param>();
    public float gamma = 1;
    /* decision function winner */
    private int[] positive_winner = new int[] { 0, 2, 0, 1, 0, 1, 0, 1, 2, 3 };
    /* decision function loser */
    private int[] negative_winner = new int[] { 1, 3, 2, 3, 3, 2, 4, 4, 4, 4 }; 

    private bool svm_model_ready = false;

    void Start()
    {
        StartCoroutine(readSVM());
    }

    /* To read in file on Android asynchronously */
    public IEnumerator readSVM() {
        using (WWW svm_reader = new WWW(System.IO.Path.Combine(Application.streamingAssetsPath, "svm.xml")))
        {
            yield return svm_reader;
            System.IO.File.WriteAllBytes(System.IO.Path.Combine(Application.persistentDataPath, "svm.xml"), svm_reader.bytes);
            string m_text = System.IO.File.ReadAllText(System.IO.Path.Combine(Application.persistentDataPath, "svm.xml"));
            parseFile(m_text);
        }
    }

    /* This is for reading parameters from SVM.xml file
     * For loop 1: read all support vectors
     * For loop 2: read all decision functions */
    public void parseFile(string m_text)
    {
        XDocument doc = XDocument.Parse(m_text);
        var alldict = doc.Element("opencv_storage").Element("opencv_ml_svm");
        var sv = alldict.Element("support_vectors").Elements("_");
        var df = alldict.Element("decision_functions").Elements("_");

        foreach (var onedict in sv)
        {
            string temp = onedict.ToString().Replace("<_>", "").Replace("</_>", "").Replace("\r\n", "").Replace("\n", "");
            double[] temp_arr = new double[30];
            int i = 0;
            foreach (string t in temp.Split(' '))
            {
                if (t.Length == 14)
                {
                    temp_arr[i] = Convert.ToDouble(float.Parse(t, System.Globalization.NumberStyles.Float));
                    i++;
                }
            }
            support_vectors.Add(temp_arr);
        }
        
        foreach(var onedict in df)
        {
            int sv_count = int.Parse(onedict.Element("sv_count").ToString().Replace("<sv_count>", "").Replace("</sv_count>", ""), System.Globalization.NumberStyles.Integer);
            double rho = Convert.ToDouble(float.Parse(onedict.Element("rho").ToString().Replace("<rho>", "").Replace("</rho>", ""), System.Globalization.NumberStyles.Float));
            string temp = onedict.Element("alpha").ToString().Replace("<alpha>", "").Replace("</alpha>", "").Replace("\r\n", "").Replace("\n", "");
            double[] alpha = new double[sv_count];
            int i = 0;
            foreach(string t in temp.Split(' '))
            {
                if (t.Length != 0)
                {
                    alpha[i] = Convert.ToDouble(float.Parse(t, System.Globalization.NumberStyles.Float));
                    i++;
                }
            }
            int[] index = new int[sv_count];
            temp = onedict.Element("index").ToString().Replace("<index>", "").Replace("</index>", "").Replace("\r\n", "").Replace("\n", "");
            i = 0;
            foreach (string t in temp.Split(' '))
            {
                if (t.Length != 0)
                {
                    index[i] = int.Parse(t, System.Globalization.NumberStyles.Integer);
                    i++;
                }
            }
            svm_param e = new svm_param();
            e.sv_count = sv_count;
            e.rho = rho;
            e.alpha = alpha;
            e.index = index;
            decision_functions_info.Add(e);
        }
        svm_model_ready = true;
    }
    
    /* This is the histogram intersection function between two samples */
    public double GeneralizedHistogramIntersection(double[] h1, double[] h2)
    {
        double sum = 0;
        for(int i = 0; i < h1.Length; i++)
        {
            double min = Math.Min(h1[i], h2[i]);
            sum += min;
        }
        return sum;

    }

    /* This is the linear kernel implementation */
    public double Linear(double[] h1, double[] h2)
    {
        double sum = 0;
        for (int i = 0; i < h1.Length; i++)
        {
 
            sum += h1[i] * h2[i];
        }
        return sum;

    }

    /* This is the histogram intersection kernel implementation */
    public int kernel(double[] x, svm_param p)
    {
        double sum = 0;
        for (int i = 0; i < p.sv_count; i++)
        {
            double alpha = p.alpha[i];
            double histInter = GeneralizedHistogramIntersection(x, support_vectors[p.index[i]]);
            sum += alpha * histInter; // times y_l if we know what is y_l
        }
        sum += p.rho;
        return sum > 0 ? 1 : -1;
    }

    /* This is the logic of decision function of one-vs-one multi-class SVM for selecting winner class.
     * 0-1 0-2 0-3 0-4 1-2 1-3 1-4 2-3 2-4 3-4
     * 0   1   2   3   4   5   6   7   8   9 */
    public int decide(double[] x)
    {
        
        int[] votes = new int[5];
        int maxIndex = 0;
        for (int i = 0; i < decision_functions_info.Count; i++)
        {
            int res = kernel(x, decision_functions_info[i]);
            int voteIndex = res > 0 ? positive_winner[i] : negative_winner[i];
            votes[voteIndex] += 1;
            if (votes[voteIndex] >= votes[maxIndex])
            {
                maxIndex = voteIndex;
            }
        }
        return maxIndex;
    }

    /* bruteforce decides whether to use svm deciding function or merely finger point distance */
    public int bruteforce_distance(Vector3 position1, Vector3 position2, float threshold = 0.08f)
    {
        float distance = Vector3.Distance(position1, position2);
        if (distance < threshold)
            return 1;
        else
            return 0;
            
    }

    public bool isSVMReady()
    {
        return svm_model_ready;
    }
}
