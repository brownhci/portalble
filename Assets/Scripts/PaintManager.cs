using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble;
using UnityEngine.EventSystems;
//using GoogleARCore;
//using GoogleARCore.Examples.Common;

/* update commented session */
//#if UNITY_EDITOR
// NOTE:
// - InstantPreviewInput does not support `deltaPosition`.
// - InstantPreviewInput does not support input from
//   multiple simultaneous screen touches.
// - InstantPreviewInput might miss frames. A steady stream
//   of touch events across frames while holding your finger
//   on the screen is not guaranteed.
// - InstantPreviewInput does not generate Unity UI event system
//   events from device touches. Use mouse/keyboard in the editor
//   instead.
//using Input = GoogleARCore.InstantPreviewInput;
//#endif

public class PaintManager : MonoBehaviour {

	//hand objects declared (thumb added)
	private GameObject palm, indexfinger_tip, thumb_tip;
	private GestureControl gestureManager;

	//Painter feature global variables
	private LineRenderer new_ink;
	private bool is_painting, paint_mode, clean_trail = false;
	private bool eraser_mode = false;

	//-----JM additions------
	//inkList for erasing and highlighting
	private List<LineRenderer> inkList;

	//animation curve for changing width
	private AnimationCurve curve;
	private float curveWidth = 0.3f; //curveWidth multiplier
	private float currWidth; //stroke width mode control

	//width float
	private float thin = 0.015f;
	private float mid = 0.028f;
	private float thick = 0.062f;

	//undo/redo list
	private Stack<PaintCommand> undoStack = new Stack<PaintCommand>();
	private Stack<PaintCommand> redoStack = new Stack<PaintCommand>();

    //list of endpoints
    private List<Vector3> endpoints_list;
    public GameObject highlightSphere;

    /// <summary>
    /// dwell object
    /// </summary>
    private Dwell m_dwell;

    /// <summary>
    /// Dwell Bar
    /// </summary>
    public PaintDwellBar m_PaintDwellBar;

    /// <summary>
    /// multimodel boolean
    /// </summary>
    public bool isMultimodal;

    /// <summary>
    /// point snap boolean
    /// </summary>
    private bool isSnapping = false;
    private GameObject hsphere;

    /// <summary>
    /// straight boolean
    /// </summary>
    private bool isLine = false;

    /// <summary>
    /// force2D boolean
    /// </summary>
    private bool is2d = false;
    public GameObject paintPlane_Prefab;
    private GameObject paintPlane;
    private bool is2dPlanePlaced = false;

    //null vector
    Vector3 nullVector = new Vector3(-9999f, -9999f, -9999f);

    //---------- Xiangyu Modified -------------------------
    // whether it's semi-transparent
    private bool semiBrush = false;

    // for materials
    public List<Material> materialPrototypes;
    private int currentMaterial = 0;
    [SerializeField]
    private Color currentColor = Color.black;

    private static GameObject basic_line;
    private Dictionary<LineRenderer, Material> highlightSet;

    // some default setting
    private static int defaultHighLightMat = 1;

    // Use this for initialization
    void Start () {
		//list of inks
		inkList = new List<LineRenderer> ();
        //list of highlights
        highlightSet = new Dictionary<LineRenderer, Material>();

        //list of endpoints of lines & snap sphere
        endpoints_list = new List<Vector3>();
        hsphere = Instantiate(highlightSphere);
        hsphere.SetActive(false);

        //paintPlane for is2d
        paintPlane = Instantiate(paintPlane_Prefab);
        paintPlane.SetActive(false);

        //liat of all materials
        if (materialPrototypes == null)
            materialPrototypes = new List<Material>();

        //create basic line pattern
        if (basic_line == null) {
            basic_line = new GameObject("BasicLine");
            basic_line.AddComponent<LineRenderer>();
            basic_line.SetActive(false);
        }

		//curr width
		currWidth = 0;

		indexfinger_tip = this.transform.GetChild (1).GetChild (2).gameObject;
		//added thumb tip
		thumb_tip = this.transform.GetChild(0).GetChild(2).gameObject;
		palm = this.transform.GetChild (5).gameObject;

        //dwell
        m_dwell = new Dwell();
        m_dwell.setScaleFactor(30f);
        if (m_PaintDwellBar != null) {
            //attach it to indexfinger (using its transform here)
            m_PaintDwellBar.BindToObject(this.transform.GetChild(1).GetChild(2));
        }

        gestureManager = this.GetComponent<GestureControl> ();
	}

    // Update is called once per frame
    void Update() {
        if (isMultimodal) {

            if(isSnapping) {
                visualizeSnapping_Update();
            }

            if(is2d) {
                setupPaintPlane();
            }
            
            if(isLine) {
                touchStraightPaint_Update();
            } else {
                touchPaint_Update();
            }
            

        }
        //else {

        //    gesturePaint_Update();
        //}
    }

    void setupPaintPlane() {
        paintPlane.SetActive(true);

        if(!is2dPlanePlaced) {
            paintPlane.transform.localPosition = new Vector3(0, 0, 0.6f);
            paintPlane.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            paintPlane.transform.parent = palm.transform;

            if(gestureManager.bufferedGesture() == "palm" || (Input.touchCount > 0 
                && !(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                && Input.GetTouch(0).phase == TouchPhase.Began)) {
                paintPlane.transform.parent = null;
                is2dPlanePlaced = true;
            }
        }
       
    }

    void visualizeSnapping_Update() {
        //if(!is_painting) {
            
        Vector3 pointSnapped = getPointSnapped(indexfinger_tip.transform.position);

        if(pointSnapped != nullVector) {
            hsphere.SetActive(true);
            hsphere.transform.position = pointSnapped;
        } else {
            hsphere.SetActive(false);
        }
    }

    private bool checkTouchValidity() {

        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
            Debug.Log("Touched UI");
            return false;
        }

        if (paintPlane.transform.parent == palm.transform) {
            return false;
        }

        return true;
    }

    void touchStraightPaint_Update() {

        if (paint_mode && !eraser_mode) {
            int numTouches = Input.touchCount;
            if (numTouches > 0) {
                Touch touch = Input.GetTouch(0);

                //switch depends on finger movements
                switch (touch.phase) {
                    case TouchPhase.Began:
                        if (!checkTouchValidity()) {
                            break;
                        }      
                        this.createNewInk();

                        new_ink.widthCurve = new AnimationCurve(new Keyframe(0f, thin), new Keyframe(0f, thin));
                        new_ink.widthMultiplier = curveWidth;
                        //currWidth = 0;

                        is_painting = true;
                        clean_trail = false;

                        new_ink.positionCount = 1;

                        if (isSnapping) {
                            Vector3 pointSnapped = getPointSnapped(indexfinger_tip.transform.position);
                            if (pointSnapped != nullVector) {
                                new_ink.SetPosition(0, pointSnapped);
                                //if there is no point to snap to
                            } else {
                                //check if is2d and get raycast point
                                if(is2d) {
                                    addRayCastPoint(0, indexfinger_tip.transform.position);
                                } else {
                                    new_ink.SetPosition(0, indexfinger_tip.transform.position);
                                }                   
                            }

                            //if not snapping
                        } else {
                            //if is2D but not snapping
                            //check if is2d and get raycast point
                            if (is2d) {
                                addRayCastPoint(0, indexfinger_tip.transform.position);
                            } else {
                                new_ink.SetPosition(0, indexfinger_tip.transform.position);
                            }  
                        }

                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (!checkTouchValidity()) {
                            break;
                        }
                        Vector3 newPoint = new Vector3();
                        is_painting = true;

                        newPoint = indexfinger_tip.transform.position;
                        new_ink.positionCount = 2;

                        if(isSnapping) {
                            Vector3 pointSnapped = getPointSnapped(indexfinger_tip.transform.position);
                            if (pointSnapped != nullVector) {
                                new_ink.SetPosition(1, pointSnapped);
                            } else {
                                //check if is2d and get raycast point
                                if (is2d) {
                                    //params are alternative points
                                    addRayCastPoint(1, newPoint);
                                } else {
                                    new_ink.SetPosition(1, newPoint);
                                }
                            }
                        } else {
                            //check if is2d and get raycast point
                            if (is2d) {
                                addRayCastPoint(1, newPoint);
                            } else {
                                new_ink.SetPosition(1, newPoint);
                            }
                        }                    
                        break;

                    case TouchPhase.Ended:
                        if (!checkTouchValidity()) {
                            break;
                        }
                        is_painting = false;

                        this.UpdateEndpoints();

                        break;
                }

            }
        }

        if (eraser_mode) {
            this.highlight(palm.transform.position);
            this.erase(palm.transform.position);
        }

    }

    void touchPaint_Update() {

        if (paint_mode && !eraser_mode) {
            int numTouches = Input.touchCount;
            if (numTouches > 0) {
                Touch touch = Input.GetTouch(0);

                //switch depends on finger movements
                switch(touch.phase) {
                    case TouchPhase.Began:
                        if (!checkTouchValidity()) {
                            break;
                        }
                        this.createNewInk();
                        new_ink.widthCurve = new AnimationCurve(new Keyframe(0f, thin), new Keyframe(0f, thin));
                        new_ink.widthMultiplier = curveWidth;

                        is_painting = true;
                        clean_trail = false;

                        if (isSnapping) {
                            Vector3 pointSnapped = getPointSnapped(indexfinger_tip.transform.position);
                            if (pointSnapped != nullVector) {
                                new_ink.positionCount++;
                                new_ink.SetPosition(0, pointSnapped);
                            } else {
                                if (is2d) {
                                    new_ink.positionCount++;
                                    addRayCastPoint(0, indexfinger_tip.transform.position);
                                }
                            }
                        } else if (is2d) {
                            new_ink.positionCount++;
                            addRayCastPoint(0, indexfinger_tip.transform.position);
                        }
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:

                        if (!checkTouchValidity()) {
                            break;
                        }
                        Vector3 newPoint = new Vector3();
                        is_painting = true;
                        newPoint = indexfinger_tip.transform.position;

                        //add here
                        if (new_ink.positionCount > 3) {
                            if (Vector3.Distance(newPoint, new_ink.GetPosition(new_ink.positionCount - 1)) > 0.0015f) {
                                touchPaint_helper((newPoint));
                            }
                        }
                        else {
                            touchPaint_helper((newPoint));
                        }
                        break;

                    case TouchPhase.Ended:
                        if (!checkTouchValidity()) {
                            break;
                        }
                        is_painting = false;
                        this.UpdateEndpoints();
                        break;
                }

            }
        }

        if (eraser_mode) {
            this.highlight(palm.transform.position);
            this.erase(palm.transform.position);
        }

    }

    void touchPaint_helper(Vector3 newPoint) {
        if (isSnapping) {
            Vector3 pointSnapped = getPointSnapped(indexfinger_tip.transform.position);
            if (pointSnapped != nullVector) {
                new_ink.positionCount++;
                new_ink.SetPosition(new_ink.positionCount - 1, pointSnapped);
            }
            else {
                new_ink.positionCount++;
                if (is2d) {
                    addRayCastPoint(new_ink.positionCount - 1, newPoint);
                }
                else {
                    new_ink.SetPosition(new_ink.positionCount - 1, newPoint);
                }
            }
        }
        else {
            new_ink.positionCount++;
            if (is2d) {
                addRayCastPoint(new_ink.positionCount - 1, newPoint);
            }
            else {
                new_ink.SetPosition(new_ink.positionCount - 1, newPoint);
            }
        }
    }
    
    void gesturePaint_Update() { 
        //Paint feature
        if (paint_mode) {
			//if paint mode and not erasing
			if (!eraser_mode) {
				if (gestureManager.bufferedGesture () == "pinch" || gestureManager.bufferedGesture() == "paint") {
                        /* user just started painting */
                    if (!is_painting) {
					    
						this.createNewInk ();

						new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, thin), new Keyframe (0f, thin));
						new_ink.widthMultiplier = curveWidth;
						currWidth = 0;

						is_painting = true;
						clean_trail = false;

                        /* user is painting */
					} else {
						
						Vector3 newPoint = new Vector3 ();

						newPoint = (indexfinger_tip.transform.position + thumb_tip.transform.position) / 2;

						this.adjustWidth (new_ink, Vector3.Distance (indexfinger_tip.transform.position, thumb_tip.transform.position));

						//add here
						if (new_ink.positionCount > 3) {
							if (Vector3.Distance (newPoint, new_ink.GetPosition (new_ink.positionCount - 1)) > 0.0015f) {
								new_ink.positionCount++;
								new_ink.SetPosition (new_ink.positionCount - 1, newPoint);
							}
						} else {
							new_ink.positionCount++;
							new_ink.SetPosition (new_ink.positionCount - 1, newPoint);
						}

                        //check if the finger is moving (dwell) to end a line
                        m_dwell.doUpdate(indexfinger_tip.transform.position);
                        m_PaintDwellBar.SetPercent(m_dwell.getProcess());

                        if (m_dwell.isReady()) {
                            is_painting = false;
                            m_dwell.SetCooldown(2.0f);
                        }

                    }
				} else if (gestureManager.bufferedGesture () == "palm") {
                    // It's because when we have no lines but palm, it leads to NULL pointer error.
                    if (new_ink != null) {
                        if (new_ink.positionCount > 10 && clean_trail == false) {
                            new_ink.positionCount -= 10;
                            clean_trail = true;
                        }
                    }

					is_painting = false;
				}
			}
		}


        if (eraser_mode) {

            this.highlight(palm.transform.position);
            this.erase(palm.transform.position);

        }
        
	}

    /**
     * clean all lines and the list of lines
     */
	public void cleanInk(){
		foreach (LineRenderer lr in inkList) {
            GameObject.Destroy(lr.gameObject);
        }

        inkList.Clear();
        highlightSet.Clear();
	}

	public void turnOnPaint(){
		paint_mode = true;
		//added JM for eraser
	}

	public void turnOffPaint(){
		//cleanInk ();
		paint_mode = false;
	}

	// for eraser icon button
	public void turnOnEraser(){
		eraser_mode = true;
	}

	public void turnOffEraser() {
		eraser_mode = false;
	}

	private void createNewInk(){
        if (currentMaterial < 0 || currentMaterial >= materialPrototypes.Count) {
            return;
        }
        GameObject gobj = Instantiate(basic_line);
        new_ink = gobj.GetComponent<LineRenderer>();
        if (new_ink == null) {
            gobj.AddComponent<LineRenderer>();
        }
        Renderer render = new_ink.GetComponent<Renderer>();
        render.material = Instantiate(materialPrototypes[currentMaterial]) as Material;
        render.material.SetColor("_Color", currentColor);
        render.material.SetColor("_EmissionColor", currentColor);

        bool hasTintColor = render.material.HasProperty("_TintColor");
        if (hasTintColor)
            render.material.SetColor("_TintColor", currentColor);
        

        // Set Line color
        LineRenderer lr = new_ink.GetComponent<LineRenderer>();
        if (lr != null) {
            lr.startColor = currentColor;
            lr.endColor = currentColor;
        }
		new_ink.positionCount = 0;
		new_ink.numCornerVertices = 15;
		new_ink.numCapVertices = 10;
        new_ink.gameObject.SetActive(true);

		inkList.Add (new_ink);

		//add to undo stack
		PaintCommand strokePaint = new StrokePaint (new_ink);
		this.performNewCommand (strokePaint);
	}

	private void erase(Vector3 eraserPos){

		if (inkList.Count > 0) {

			for (int j = 0; j < inkList.Count; j++) {
				//check if eraserPos overlaps with a vertice in a ink object
				if (inkList [j] != null) {
					for (int i = 0; i < inkList [j].positionCount; i++) {
						if (inkList [j].gameObject.activeInHierarchy) {
							
							if (Mathf.Abs (inkList [j].GetPosition (i).x - eraserPos.x) < 0.08f && Mathf.Abs (inkList [j].GetPosition (i).y - eraserPos.y) < 0.08f && Mathf.Abs (inkList [j].GetPosition (i).z - eraserPos.z) < 0.08f) {

								inkList [j].gameObject.SetActive (false);

								//add to undo stack
								PaintCommand strokeErase = new StrokeErase (inkList[j]);
								this.performNewCommand (strokeErase);
							}
						}
					}
				}
			}
		}
	}

	private void highlight(Vector3 pos){
        // Check if highlight mat is available
        if (defaultHighLightMat < 0 || defaultHighLightMat >= materialPrototypes.Count)
            return;

		if (inkList.Count > 0) {

			for (int j = 0; j < inkList.Count; j++) {
				//check if eraserPos overlaps with a vertice in a ink object
				if (inkList [j] != null) {
					for (int i = 0; i < inkList [j].positionCount; i++) {
						if (inkList [j].gameObject.activeInHierarchy) {
							if (Mathf.Abs (inkList [j].GetPosition (i).x - pos.x) < 0.16 && Mathf.Abs (inkList [j].GetPosition (i).y - pos.y) < 0.16f && Mathf.Abs (inkList [j].GetPosition (i).z - pos.z) < 0.16f) {
                                // High light is to swap materials
                                if (highlightSet.ContainsKey(inkList[j]) == false) {
                                    highlightSet.Add(inkList[j], inkList[j].material);
                                    inkList[j].material = materialPrototypes[defaultHighLightMat];
                                }
							} else {
								//change back to normal mat if too far way and highlighted
								if (highlightSet.ContainsKey(inkList[j])) {
                                    inkList[j].material = highlightSet[inkList[j]];
                                    highlightSet.Remove(inkList[j]);
                                }
							}
						}
					}
				}
			}
		}
	}
		
	private void adjustWidth(LineRenderer curInk, float tipsDist){
		//smallest dist and thin
		if (tipsDist < 0.04f) {

			if (currWidth != 0) {
                LineRenderer prevInk = inkList[inkList.Count - 1];
                this.createNewInk ();
                if (prevInk.positionCount > 0) {
                    new_ink.positionCount = 1;
                    new_ink.SetPosition(0, prevInk.GetPosition(prevInk.positionCount - 1));
                }

                //transition curve
                if (currWidth == thick) {
					new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, thick), new Keyframe (0.1f, thin), new Keyframe (0.9f, thin), new Keyframe (1.0f, thin));
				} else {
					new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, mid), new Keyframe (0.1f, thin), new Keyframe (0.9f, thin), new Keyframe (1.0f, thin));
				}

				new_ink.widthMultiplier = curveWidth;
				currWidth = 0;
			}
		
		//mid dist and mid
		} else if (tipsDist >= 0.04f && tipsDist < 0.06f) {
			
			if (currWidth != 1) {
                LineRenderer prevInk = inkList[inkList.Count - 1];
                this.createNewInk ();
                if (prevInk.positionCount > 0) {
                    new_ink.positionCount = 1;
                    new_ink.SetPosition(0, prevInk.GetPosition(prevInk.positionCount - 1));
                }

				//transition curve
				if (currWidth == thin) {
					new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, thin), new Keyframe (0.1f, mid), new Keyframe (0.9f, mid), new Keyframe (1.0f, mid));
				} else {
					new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, thick), new Keyframe (0.1f, mid), new Keyframe (0.9f, mid), new Keyframe (1.0f, mid));
				}

				new_ink.widthMultiplier = curveWidth;
				currWidth = 1;
			}

		//greatest dist and thick
		} else {

			if (currWidth != 2) {
                LineRenderer prevInk = inkList[inkList.Count - 1];
                this.createNewInk ();
                if (prevInk.positionCount > 0) {
                    new_ink.positionCount = 1;
                    new_ink.SetPosition(0, prevInk.GetPosition(prevInk.positionCount - 1));
                }


                //transition curve
                if (currWidth == thin) {
					new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, thin), new Keyframe (0.1f, thick), new Keyframe (0.9f, thick), new Keyframe (1.0f, thick));
				} else {
					new_ink.widthCurve = new AnimationCurve (new Keyframe (0f, mid), new Keyframe (0.1f, thick), new Keyframe (0.9f, thick), new Keyframe (1.0f, thick));
				}

				new_ink.widthMultiplier = curveWidth;
				currWidth = 2;
			}
		}
	}

	private void performNewCommand(PaintCommand command) {
		if (!undoStack.Contains (command)) {
			redoStack.Clear ();
			undoStack.Push (command);
		}
	}

	public void undoCommand() {
		if (undoStack.Count > 0) {
			PaintCommand command = undoStack.Pop ();
			command.undo ();
			redoStack.Push (command);
		}
	}

	public void redoCommand() {
		if (redoStack.Count > 0) {
			PaintCommand command = redoStack.Pop ();
			command.redo ();
			undoStack.Push (command);
		}
	}

    public bool isSemiTransparentBrush() {
        return semiBrush;
    }

    public Color CurrentColor {
        get {
            return currentColor;
        }
        set {
            currentColor = value;
        }
    }

    /**
     * Current Material Property
     */
    public int CurrentMaterial {
        get {
            return currentMaterial;
        }
        set {
            SetCurrentMaterial(value);
        }
    }

    /**
     * Set current material with material ID
     */
    public void SetCurrentMaterial(int index) {
        if (index < materialPrototypes.Count && index >= 0) {
            currentMaterial = index;
        }
    }

    /**
     * Add a new type of material and return its id
     */
    public int AddALineMaterial(Material mat) {
        materialPrototypes.Add(mat);
        return (materialPrototypes.Count - 1);
    }

    public void UpdateEndpoints() {

        endpoints_list = new List<Vector3> ();

        //loop through all exisiting lines and add their endpoints into endpoint_list
        if (inkList.Count > 0) {
            for (int j = 0; j < inkList.Count; j++) {
                
                if ((inkList[j] != null) && inkList[j].gameObject.activeInHierarchy) {
                    endpoints_list.Add(inkList[j].GetPosition(0));
                    endpoints_list.Add(inkList[j].GetPosition(inkList[j].positionCount - 1));
                }
            }
        }
    }

    public void highlightEndpoints(Vector3 pos) {
        if(endpoints_list.Count > 0) {
            for (int i = 0; i < endpoints_list.Count; i++) {
                bool snapped = is_snapped(endpoints_list[i]);
                if (Vector3.Distance(endpoints_list[i], pos) < 0.08f) {
                    if(!snapped) {
                        GameObject hsphere = Instantiate(highlightSphere);
                        hsphere.transform.position = endpoints_list[i];
                    }
                } else {
                    if(snapped) {
                        GameObject.Destroy(unhighlightSnap(endpoints_list[i]));
                    }
                }
            }
        }
    }

    public bool is_snapped(Vector3 end_pos) {
        GameObject[] hspheres = GameObject.FindGameObjectsWithTag("endpoint");
        for (int i = 0; i < hspheres.Length; i++) {
            if(hspheres[i].transform.position == end_pos) {
                return true;
            }
        }
        return false;
    }

    public GameObject unhighlightSnap(Vector3 end_pos) {
        GameObject[] hspheres = GameObject.FindGameObjectsWithTag("endpoint");
        for (int i = 0; i < hspheres.Length; i++) {
            if (hspheres[i].transform.position == end_pos) {
                return hspheres[i];
            }
        }
        return null;
    }

    private Vector3 getPointSnapped(Vector3 palm_pos) {
        if(endpoints_list.Count != 0) {
            float[] distances = new float[endpoints_list.Count];
            for (int i = 0; i < endpoints_list.Count; i++) {
                distances[i] = Vector3.Distance(endpoints_list[i], palm_pos);
            }

            float minDist = Mathf.Min(distances);

            if (minDist < 0.08f) {
                int minIndex = System.Array.IndexOf(distances, minDist);
                return endpoints_list[minIndex];
            }
        }

        return nullVector;
    }

    private Vector3 getRayCastPlanePoint(Vector3 pos, Vector3 dir) {
        Ray ray = new Ray(pos, dir);
        RaycastHit hit;
        if(paintPlane.GetComponent<BoxCollider>().Raycast(ray, out hit, 9999f)) {
            return hit.point;
        }

        return nullVector;
    }

    private void addRayCastPoint(int posIndex, Vector3 alterPoint) {
        Vector3 pointCast = getRayCastPlanePoint(indexfinger_tip.transform.position, -indexfinger_tip.transform.up);
        if (pointCast != nullVector) {
            new_ink.SetPosition(posIndex, pointCast);
        }
        else {
            new_ink.SetPosition(posIndex, alterPoint);
        }
    }

    public void toggleSnap() {
        isSnapping = !isSnapping;
        if(!isSnapping) {
            hsphere.SetActive(false);
        }
    }

    public void toggle2D() {
        is2d = !is2d;
        if(!is2d) {
            paintPlane.SetActive(false);
            is2dPlanePlaced = false;
        }
    }

    public void toggleLine() {
        isLine = !isLine;
    }


}
