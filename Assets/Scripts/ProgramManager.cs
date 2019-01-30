using ROSBridgeLib.art_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.diagnostic_msgs;
using HoloToolkit.Unity;

public class ProgramManager : Singleton<ProgramManager> {
    public GameObject visualizeObjectPrefab;

    private ProgramMsg programMsg;
    private InterfaceStateMsg interfaceStateMsg;
    private visualization_state visualizationState = visualization_state.VISUALIZATION_DISABLED;
    private ProgramItemMsg currentProgramItem = null;

    private bool buildingProgram;
    private bool start_visualization;
    private bool replay_visualization;
    private bool visualization_running;
    private bool visualize_program;
    private bool visualize_block;
    private ProgramBlockMsg programBlockMsg;
    //private Dictionary<UInt16, GameObject> programToVisualize = new Dictionary<UInt16, GameObject>();
    //private Dictionary<UInt16, UInt16> programOrderHelper = new Dictionary<UInt16, UInt16>();

    //<blockID, block<itemID,instruction>>
    private Dictionary<UInt16, Dictionary<UInt16, GameObject>> program = new Dictionary<UInt16, Dictionary<UInt16, GameObject>>();
    //<blockID, block<itemID,on_successID>>
    private Dictionary<UInt16, Dictionary<UInt16, UInt16>> programHelper = new Dictionary<UInt16, Dictionary<UInt16, UInt16>>();

    private Coroutine buildProgramCoroutine;
    private Coroutine runProgramCoroutine;

    private GameObject speechManagerObj;
    private TextToSpeechManager speechManager;
       
    //indicators when user says "Next" or "Previous"
    private bool next;
    private bool previous;


    // Use this for initialization
    void Start () {
        start_visualization = false;
        replay_visualization = false;
        visualization_running = false;
        visualize_program = false;
        visualize_block = false;
        next = false;
        previous = false;

        speechManagerObj = GameObject.FindGameObjectWithTag("speech_manager");
        speechManager = speechManagerObj.GetComponent<TextToSpeechManager>();
    }
	
	// Update is called once per frame
	void Update () {
		if (SystemStarter.Instance.calibrated) {
            if (start_visualization && !visualization_running) {
                if (programMsg != null) {
                    if (interfaceStateMsg.GetProgramID() == programMsg.GetHeader().GetID()) {
                        if(visualize_program) {
                            Debug.Log("VISUALIZING PROGRAM " + interfaceStateMsg.GetProgramID());

                            buildingProgram = true;
                            StartCoroutine(BuildProgram());
                            runProgramCoroutine = StartCoroutine(RunProgram());
                            visualization_running = true;
                        }
                        else if(visualize_block) {
                            programBlockMsg = GetProgramBlock();
                            if (programBlockMsg != null) {
                                Debug.Log("VISUALIZING PROGRAM " + interfaceStateMsg.GetProgramID() + " and its block " + programBlockMsg.GetID());

                                buildingProgram = true;
                                StartCoroutine(BuildJustBlock());
                                runProgramCoroutine = StartCoroutine(RunProgram());
                                visualization_running = true;
                            }
                        }
                    }
                }
            }
            else if (replay_visualization && !visualization_running) {
                Debug.Log("REPLAYING PROGRAM " + interfaceStateMsg.GetProgramID());
                runProgramCoroutine = StartCoroutine(RunProgram());
                visualization_running = true;
            }
        }
	}

    private ProgramBlockMsg GetProgramBlock() {
        foreach (var programBlock in programMsg.GetBlocks()) {
            if (programBlock.GetID() == interfaceStateMsg.GetBlockID()) {
                return programBlock;
            }
        }
        return null;
    }

    //Builds whole program to visualize
    private IEnumerator BuildProgram() {
        Debug.Log("BULDING PROGRAM");
        buildingProgram = true;

        //build whole program
        foreach (var programBlock in programMsg.GetBlocks()) {
            yield return StartCoroutine(BuildBlock(programBlock));
        }

        buildingProgram = false;
        yield return null;
    }

    //Builds just block to visualize
    private IEnumerator BuildJustBlock() {
        Debug.Log("BULDING PROGRAM BLOCK");
        buildingProgram = true;

        yield return StartCoroutine(BuildBlock(programBlockMsg));        

        buildingProgram = false;
        yield return null;
    }

    private IEnumerator BuildBlock(ProgramBlockMsg programBlock) {
        ushort blockID = programBlock.GetID();
        Dictionary<UInt16, GameObject> programToVisualize = new Dictionary<UInt16, GameObject>();
        Dictionary<UInt16, UInt16> programOrderHelper = new Dictionary<UInt16, UInt16>();

        foreach (var programItem in programBlock.GetProgramItems()) {
            Debug.Log(programItem.GetType());
            switch (programItem.GetIType()) {
                //case program_type.PICK_FROM_POLYGON:
                case "PickFromPolygon":
                    //1. moznost jak zjistit rozmery aktualniho objektu .. nutno ale pockat na odpoved
                    //2. moznost bude to napevno naprat do kodu
                    ROSCommunicationManager.Instance.ros.CallService("/art/db/object_type/get", "{\"name\": \"" + programItem.GetObject()[0] + "\"}");
                    //wait until ROS sends object dimensions.. az podminka nabude true, pujde se dal
                    yield return new WaitUntil(() => ObjectsManager.Instance.ObjectIsKnown(programItem.GetObject()[0]));

                    GameObject pickFromPolygonInstr = new GameObject();
                    pickFromPolygonInstr.AddComponent<PickFromPolygon>();
                    pickFromPolygonInstr.transform.parent = gameObject.transform;
                    pickFromPolygonInstr.tag = "PICK_FROM_POLYGON";

                    PickFromPolygon pfg = pickFromPolygonInstr.GetComponent<PickFromPolygon>();
                    pfg.programItem = programItem;
                    pfg.visualizeObjectPrefab = visualizeObjectPrefab;

                    programToVisualize.Add(programItem.GetID(), pickFromPolygonInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());

                    Debug.Log("PICK_FROM_POLYGON Instruction created");

                    break;
                //case program_type.PICK_FROM_FEEDER:
                case "PickFromFeeder":
                    Debug.Log("Building PICK_FROM_FEEDER");
                    GameObject pickFromFeederInstr = new GameObject();
                    pickFromFeederInstr.AddComponent<PickFromFeeder>();
                    pickFromFeederInstr.transform.parent = gameObject.transform;
                    pickFromFeederInstr.tag = "PICK_FROM_FEEDER";

                    PickFromFeeder pff = pickFromFeederInstr.GetComponent<PickFromFeeder>();
                    pff.programItem = programItem;
                    pff.visualizeObjectPrefab = visualizeObjectPrefab;

                    //if ref id references some previously created instruction Pick From Feeder, then add aditional info to ProgramMsg (copy it from referenced instruction)
                    if (programItem.GetRefID().Count > 0) {
                        if (programToVisualize.ContainsKey(programItem.GetRefID()[0]))
                            if (programToVisualize[programItem.GetRefID()[0]].tag == "PICK_FROM_FEEDER") {
                                Debug.Log("Instantiating existing PICK_FROM_FEEDER");
                                PickFromFeeder refInstr = programToVisualize[programItem.GetRefID()[0]].GetComponent<PickFromFeeder>();
                                pff.programItem = new ProgramItemMsg(pff.programItem.GetID(), pff.programItem.GetOnSuccess(), pff.programItem.GetOnFailure(), pff.programItem.GetIType(), pff.programItem.GetName(),
                                    refInstr.programItem.GetObject(), refInstr.programItem.GetPose(), refInstr.programItem.GetPolygon(), pff.programItem.GetRefID(), pff.programItem.GetFlags(),
                                    pff.programItem.GetDoNotClear(), pff.programItem.GetLabels());

                                ROSCommunicationManager.Instance.ros.CallService("/art/db/object_type/get", "{\"name\": \"" + pff.programItem.GetObject()[0] + "\"}");
                                yield return new WaitUntil(() => ObjectsManager.Instance.ObjectIsKnown(pff.programItem.GetObject()[0]));
                            }
                    }


                    programToVisualize.Add(programItem.GetID(), pickFromFeederInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());

                    Debug.Log("PICK_FROM_FEEDER Instruction created");

                    break;
                //case program_type.PLACE_TO_POSE:
                case "PlaceToPose":
                    GameObject placeToPoseInstr = new GameObject();
                    placeToPoseInstr.AddComponent<PlaceToPose>();
                    placeToPoseInstr.transform.parent = gameObject.transform;
                    placeToPoseInstr.tag = "PLACE_TO_POSE";

                    PlaceToPose ptp = placeToPoseInstr.GetComponent<PlaceToPose>();
                    ptp.programItem = programItem;

                    if (programItem.GetRefID().Count > 0) {
                        if (programToVisualize.ContainsKey(programItem.GetRefID()[0]))
                            ptp.referenceItem = programToVisualize[programItem.GetRefID()[0]];
                    }

                    programToVisualize.Add(programItem.GetID(), placeToPoseInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());

                    Debug.Log("PLACE_TO_POSE Instruction created");
                    break;
                //case program_type.DRILL_POINTS:
                case "DrillPoints":
                    ROSCommunicationManager.Instance.ros.CallService("/art/db/object_type/get", "{\"name\": \"" + programItem.GetObject()[0] + "\"}");
                    yield return new WaitUntil(() => ObjectsManager.Instance.ObjectIsKnown(programItem.GetObject()[0]));

                    GameObject drillPointsInstr = new GameObject();
                    drillPointsInstr.AddComponent<DrillPoints>();
                    drillPointsInstr.transform.parent = gameObject.transform;
                    drillPointsInstr.tag = "DRILL_POINTS";

                    DrillPoints dp = drillPointsInstr.GetComponent<DrillPoints>();
                    dp.programItem = programItem;
                    dp.visualizeObjectPrefab = visualizeObjectPrefab;

                    programToVisualize.Add(programItem.GetID(), drillPointsInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());

                    Debug.Log("DRILL_POINTS Instruction created");
                    break;
                //case program_type.GET_READY:
                case "GetReady":
                    GameObject getReadyInstr = new GameObject();
                    getReadyInstr.AddComponent<GetReady>();
                    getReadyInstr.transform.parent = gameObject.transform;
                    getReadyInstr.tag = "GET_READY";

                    GetReady gr = getReadyInstr.GetComponent<GetReady>();
                    gr.programItem = programItem;

                    programToVisualize.Add(programItem.GetID(), getReadyInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());
                    Debug.Log("GET_READY Instruction created");
                    break;
                //case program_type.WAIT_UNTIL_USER_FINISHES:
                case "WaitUntilUserFinishes":
                    GameObject waitUntilUserFinishesInstr = new GameObject();
                    waitUntilUserFinishesInstr.AddComponent<WaitUntilUserFinishes>();
                    waitUntilUserFinishesInstr.transform.parent = gameObject.transform;
                    waitUntilUserFinishesInstr.tag = "WAIT_UNTIL_USER_FINISHES";

                    WaitUntilUserFinishes wuuf = waitUntilUserFinishesInstr.GetComponent<WaitUntilUserFinishes>();
                    wuuf.programItem = programItem;

                    programToVisualize.Add(programItem.GetID(), waitUntilUserFinishesInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());
                    Debug.Log("WAIT_UNTIL_USER_FINISHES Instruction created");
                    break;
                case "VisualInspection":
                    GameObject visualInspectionInstr = new GameObject();
                    visualInspectionInstr.AddComponent<VisualInspection>();
                    visualInspectionInstr.transform.parent = gameObject.transform;
                    visualInspectionInstr.tag = "VISUAL_INSPECTION";

                    VisualInspection vs = visualInspectionInstr.GetComponent<VisualInspection>();
                    vs.programItem = programItem;

                    if (programItem.GetRefID().Count > 0) {
                        if (programToVisualize.ContainsKey(programItem.GetRefID()[0]))
                            vs.referenceItem = programToVisualize[programItem.GetRefID()[0]];
                    }

                    programToVisualize.Add(programItem.GetID(), visualInspectionInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());

                    Debug.Log("VISUAL_INSPECTION Instruction created");
                    break;
                case "PlaceToContainer":
                    GameObject placeToContainerInstr = new GameObject();
                    placeToContainerInstr.AddComponent<PlaceToContainer>();
                    placeToContainerInstr.transform.parent = gameObject.transform;
                    placeToContainerInstr.tag = "PLACE_TO_CONTAINER";

                    PlaceToContainer ptc = placeToContainerInstr.GetComponent<PlaceToContainer>();
                    ptc.programItem = programItem;

                    if (programItem.GetRefID().Count > 0) {
                        if (programToVisualize.ContainsKey(programItem.GetRefID()[0]))
                            ptc.referenceItem = programToVisualize[programItem.GetRefID()[0]];
                    }

                    programToVisualize.Add(programItem.GetID(), placeToContainerInstr);
                    programOrderHelper.Add(programItem.GetID(), programItem.GetOnSuccess());

                    Debug.Log("PLACE_TO_CONTAINER Instruction created");
                    break;
                default:
                    break;
            }
        }

        program.Add(blockID, programToVisualize);
        programHelper.Add(blockID, programOrderHelper);

        yield return null;
    }


    private IEnumerator RunProgram() {
        //wait while program is building
        yield return new WaitWhile(() => buildingProgram);

        Dictionary<UInt16, GameObject> programToVisualize = new Dictionary<UInt16, GameObject>();
        Dictionary<UInt16, UInt16> programOrderHelper = new Dictionary<UInt16, UInt16>();

        foreach (var block in program) {
            programToVisualize = block.Value;
            //load programOrderHelper dictionary
            if(!programHelper.TryGetValue(block.Key, out programOrderHelper)) {
                break;
            }

            UInt16 currentID = programOrderHelper.First().Key;
            UInt16 on_successID = programOrderHelper.First().Value;

            while (true) {
                GameObject programItem;
                //load programItem by currentID key            
                if (!programToVisualize.TryGetValue(currentID, out programItem)) {
                    break;
                }

                Debug.Log("Visualizing: " + currentID.ToString() + ".. On success will be: " + on_successID.ToString());

                //execute programItem
                switch (programItem.gameObject.tag) {
                    case "PICK_FROM_POLYGON":
                        Debug.Log("Starting pick from polygon");
                        PickFromPolygon pfg = programItem.GetComponent<PickFromPolygon>();
                        //sends change of interface to ARTable - for states switching during visualization
                        SendInterfaceStateChangeToROS(pfg.programItem, block.Key);
                        currentProgramItem = pfg.programItem;
                        pfg.Run();
                        //wait until instruction finishes or until user says "Next" or "Previous"
                        yield return new WaitWhile(() => pfg.IsRunning() && !next && !previous);
                        //if Next is said, instruction skips to its end state
                        if (next) {
                            pfg.OnNextInstruction();
                            next = false;
                        }
                        ////if Previous is said, instruction skips to its initial state and decrements iterator twice to play previous instruction
                        //if (previous) {
                        //    pfg.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending pick from polygon");
                        break;
                    case "PICK_FROM_FEEDER":
                        Debug.Log("Starting pick from feeder");
                        PickFromFeeder pff = programItem.GetComponent<PickFromFeeder>();
                        SendInterfaceStateChangeToROS(pff.programItem, block.Key);
                        currentProgramItem = pff.programItem;
                        pff.Run();
                        yield return new WaitWhile(() => pff.IsRunning() && !next && !previous);
                        if (next) {
                            pff.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    pff.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending pick from feeder");
                        break;
                    case "PLACE_TO_POSE":
                        Debug.Log("Starting place to pose");
                        PlaceToPose ptp = programItem.GetComponent<PlaceToPose>();
                        SendInterfaceStateChangeToROS(ptp.programItem, block.Key);
                        currentProgramItem = ptp.programItem;
                        ptp.Run();
                        yield return new WaitWhile(() => ptp.IsRunning() && !next && !previous);
                        if (next) {
                            ptp.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    ptp.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending place to pose");
                        break;
                    case "DRILL_POINTS":
                        Debug.Log("Starting drill points");
                        DrillPoints dp = programItem.GetComponent<DrillPoints>();
                        SendInterfaceStateChangeToROS(dp.programItem, block.Key);
                        currentProgramItem = dp.programItem;
                        dp.Run();
                        yield return new WaitWhile(() => dp.IsRunning() && !next && !previous);
                        if (next) {
                            dp.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    dp.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending drill points");
                        break;
                    case "GET_READY":
                        Debug.Log("Starting get ready");
                        GetReady gr = programItem.GetComponent<GetReady>();
                        SendInterfaceStateChangeToROS(gr.programItem, block.Key);
                        currentProgramItem = gr.programItem;
                        gr.Run();
                        yield return new WaitWhile(() => gr.IsRunning() && !next && !previous);
                        if (next) {
                            gr.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    gr.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending get ready");
                        break;
                    case "WAIT_UNTIL_USER_FINISHES":
                        Debug.Log("Starting wait until user finishes");
                        WaitUntilUserFinishes wuuf = programItem.GetComponent<WaitUntilUserFinishes>();
                        SendInterfaceStateChangeToROS(wuuf.programItem, block.Key);
                        currentProgramItem = wuuf.programItem;
                        wuuf.Run();
                        yield return new WaitWhile(() => wuuf.IsRunning() && !next && !previous);
                        if (next) {
                            wuuf.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    wuuf.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending wait until user finishes");
                        break;
                    case "VISUAL_INSPECTION":
                        Debug.Log("Starting visual inspection");
                        VisualInspection vs = programItem.GetComponent<VisualInspection>();
                        SendInterfaceStateChangeToROS(vs.programItem, block.Key);
                        currentProgramItem = vs.programItem;
                        vs.Run();
                        yield return new WaitWhile(() => vs.IsRunning() && !next && !previous);
                        if (next) {
                            vs.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    vs.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending visual inspection");
                        break;
                    case "PLACE_TO_CONTAINER":
                        Debug.Log("Starting place to container");
                        PlaceToContainer ptc = programItem.GetComponent<PlaceToContainer>();
                        SendInterfaceStateChangeToROS(ptc.programItem, block.Key);
                        currentProgramItem = ptc.programItem;
                        ptc.Run();
                        yield return new WaitWhile(() => ptc.IsRunning() && !next && !previous);
                        if (next) {
                            ptc.OnNextInstruction();
                            next = false;
                        }
                        //if (previous) {
                        //    ptc.OnPreviousInstruction();
                        //    i = GetCorrectIndexOfPreviousInstruction(i);
                        //    previous = false;
                        //}
                        Debug.Log("Ending place to container");
                        break;
                    default:
                        break;
                }

                //end program execution
                if (on_successID == 0) {
                    break;
                }

                //load next ID into currentID
                currentID = on_successID;

                //try to load nex on_successID
                if (!programOrderHelper.TryGetValue(currentID, out on_successID)) {
                    break;
                }
            }
        }
        ////foreach(var programItem in programToVisualize.Values) {
        //for (int i=0; i < programToVisualize.Count; i++) {
        //    GameObject programItem = programToVisualize.ElementAt(i).Value;
        //    switch (programItem.gameObject.tag) {
        //        case "PICK_FROM_POLYGON":
        //            Debug.Log("Starting pick from polygon");
        //            PickFromPolygon pfg = programItem.GetComponent<PickFromPolygon>();
        //            //sends change of interface to ARTable - for states switching during visualization
        //            SendInterfaceStateChangeToROS(pfg.programItem);
        //            currentProgramItem = pfg.programItem;
        //            pfg.Run();
        //            //wait until instruction finishes or until user says "Next" or "Previous"
        //            yield return new WaitWhile(() => pfg.IsRunning() && !next && !previous);
        //            //if Next is said, instruction skips to its end state
        //            if(next) {
        //                pfg.OnNextInstruction();
        //                next = false;
        //            }
        //            //if Previous is said, instruction skips to its initial state and decrements iterator twice to play previous instruction
        //            if(previous) {
        //                pfg.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending pick from polygon");
        //            break;
        //        case "PICK_FROM_FEEDER":
        //            Debug.Log("Starting pick from feeder");
        //            PickFromFeeder pff = programItem.GetComponent<PickFromFeeder>();
        //            SendInterfaceStateChangeToROS(pff.programItem);
        //            currentProgramItem = pff.programItem;
        //            pff.Run();
        //            yield return new WaitWhile(() => pff.IsRunning() && !next && !previous);
        //            if (next) {
        //                pff.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                pff.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending pick from feeder");
        //            break;
        //        case "PLACE_TO_POSE":
        //            Debug.Log("Starting place to pose");
        //            PlaceToPose ptp = programItem.GetComponent<PlaceToPose>();
        //            SendInterfaceStateChangeToROS(ptp.programItem);
        //            currentProgramItem = ptp.programItem;
        //            ptp.Run();
        //            yield return new WaitWhile(() => ptp.IsRunning() && !next && !previous);
        //            if (next) {
        //                ptp.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                ptp.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending place to pose");
        //            break;
        //        case "DRILL_POINTS":
        //            Debug.Log("Starting drill points");
        //            DrillPoints dp = programItem.GetComponent<DrillPoints>();
        //            SendInterfaceStateChangeToROS(dp.programItem);
        //            currentProgramItem = dp.programItem;
        //            dp.Run();
        //            yield return new WaitWhile(() => dp.IsRunning() && !next && !previous);
        //            if (next) {
        //                dp.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                dp.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending drill points");
        //            break;
        //        case "GET_READY":
        //            Debug.Log("Starting get ready");
        //            GetReady gr = programItem.GetComponent<GetReady>();
        //            SendInterfaceStateChangeToROS(gr.programItem);
        //            currentProgramItem = gr.programItem;
        //            gr.Run();
        //            yield return new WaitWhile(() => gr.IsRunning() && !next && !previous);
        //            if (next) {
        //                gr.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                gr.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending get ready");
        //            break;
        //        case "WAIT_UNTIL_USER_FINISHES":
        //            Debug.Log("Starting wait until user finishes");
        //            WaitUntilUserFinishes wuuf = programItem.GetComponent<WaitUntilUserFinishes>();
        //            SendInterfaceStateChangeToROS(wuuf.programItem);
        //            currentProgramItem = wuuf.programItem;
        //            wuuf.Run();
        //            yield return new WaitWhile(() => wuuf.IsRunning() && !next && !previous);
        //            if (next) {
        //                wuuf.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                wuuf.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending wait until user finishes");
        //            break;
        //        case "VISUAL_INSPECTION":
        //            Debug.Log("Starting visual inspection");
        //            VisualInspection vs = programItem.GetComponent<VisualInspection>();
        //            SendInterfaceStateChangeToROS(vs.programItem);
        //            currentProgramItem = vs.programItem;
        //            vs.Run();
        //            yield return new WaitWhile(() => vs.IsRunning() && !next && !previous);
        //            if (next) {
        //                vs.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                vs.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending visual inspection");
        //            break;
        //        case "PLACE_TO_CONTAINER":
        //            Debug.Log("Starting place to container");
        //            PlaceToContainer ptc = programItem.GetComponent<PlaceToContainer>();
        //            SendInterfaceStateChangeToROS(ptc.programItem);
        //            currentProgramItem = ptc.programItem;
        //            ptc.Run();
        //            yield return new WaitWhile(() => ptc.IsRunning() && !next && !previous);
        //            if (next) {
        //                ptc.OnNextInstruction();
        //                next = false;
        //            }
        //            if (previous) {
        //                ptc.OnPreviousInstruction();
        //                i = GetCorrectIndexOfPreviousInstruction(i);
        //                previous = false;
        //            }
        //            Debug.Log("Ending place to container");
        //            break;
        //        default:
        //            break;
        //    }
        //}

        speechManager.Say("Visualization of block " + interfaceStateMsg.GetBlockID().ToString() + " in program " + interfaceStateMsg.GetProgramID().ToString() + " ended.");
        visualization_running = false;
        start_visualization = false;
        replay_visualization = false;
        visualize_program = false;
        visualize_block = false;
        VisualizationManager.Instance.VisualizationInProcess(false);
        //ClearProgram();
        Debug.Log("Visualization of program " + interfaceStateMsg.GetProgramID().ToString() + " ended.");
    }

    private void SendInterfaceStateChangeToROS(ProgramItemMsg programItemMsg, UInt16 blockID) {
        InterfaceStateMsg msg = new InterfaceStateMsg("PROJECTED UI", interfaceStateMsg.GetSystemState(), interfaceStateMsg.GetTimestamp(),
            interfaceStateMsg.GetProgramID(), blockID, programItemMsg, interfaceStateMsg.GetFlags(), interfaceStateMsg.GetEditEnabled(),
            interfaceStateMsg.GetErrorSeverity(), interfaceStateMsg.GetErrorCode());
        ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), msg);
    }

    //private int GetCorrectIndexOfPreviousInstruction(int i) {
    //    //1st decrement - replay current instruction
    //    //2nd decrement - replay previous instruction
    //    //3rd decrement - only if replayed instruction was place to pose

    //    //skip first instruction
    //    if (i > 0) {
    //        i--;
    //        //if instruction before should be PLACE_TO_POSE than go one more instruction backwards to have something to place (PickFromFeeder or PickFromPolygon)
    //        //it would be nonsense to replay place to pose when robot has nothing in gripper to place
    //        if (programToVisualize.ElementAt(i).Value.gameObject.tag.Equals("PLACE_TO_POSE")) {
    //            i--;
    //        }
    //    }

    //    //if it was first instruction - replay it
    //    //if it was any other instruction - replay previous instruction
    //    i--;

    //    return i;
    //}

    //clears all variables and destroyes program instructions.. should be called only when "Back to blocks" pressed
    public void ClearProgram() {
        foreach(var block in program.Values) {
            foreach (var programItem in block.Values) {
                Destroy(programItem);
            }
        }
        program.Clear();
        programHelper.Clear();
        programMsg = null;
        programBlockMsg = null;
    }

    public void StartVisualization(InterfaceStateMsg msg, bool visualizeProgram) {
        interfaceStateMsg = msg;
        start_visualization = true;

        visualize_program = visualizeProgram;
        visualize_block = !visualizeProgram;
    }

    //only activate instructions and they will rerun .. (OnEnable() is called in each ProgramInstruction)
    public void ReplayVisualization() {
        replay_visualization = true;
        foreach(var block in program.Values) {
            foreach (var programItem in block.Values) {
                programItem.SetActive(true);
            }
        }
    }

    //visualization is stopped by disabling all program instructions (not by deleting them, in case of replaying program, so the whole program doesn't have to load again)
    public void StopVisualization() {
        StopCoroutine(runProgramCoroutine);

        visualization_running = false;
        start_visualization = false;
        replay_visualization = false;
        visualize_program = false;
        visualize_block = false;
        VisualizationManager.Instance.VisualizationInProcess(false);

        foreach (var block in program.Values) {
            foreach (var programItem in block.Values) {
                programItem.SetActive(false);
            }
        }

        //ClearProgram();
        Debug.Log("Program visualization ended");
    }

    public void SetProgramMsgFromROS(ProgramMsg msg) {
        //receive only if program is really visualizing (conflicts when in edit mode)
        if(start_visualization) {
            programMsg = msg;
        }
        //Debug.Log(programMsg.ToYAMLString());
    }


    public void SetVisualizationState(visualization_state vState) {
        visualizationState = vState;
    }

    //called when user says "Next".. used to skip current instruction
    public void OnNextInstruction() {
        //reacting only when visualization running
        if (visualizationState == visualization_state.VISUALIZATION_RUN || visualizationState == visualization_state.VISUALIZATION_RESUME ||
            visualizationState == visualization_state.VISUALIZATION_REPLAY) {
            next = true;
        }
    }

    //called when user says "Previous".. used to go back to previous instruction
    public void OnPreviousInstruction() {
        //reacting only when visualization running
        if (visualizationState == visualization_state.VISUALIZATION_RUN || visualizationState == visualization_state.VISUALIZATION_RESUME ||
            visualizationState == visualization_state.VISUALIZATION_REPLAY) {
            previous = true;
        }
    }

    //called when user says "Stop"
    public void OnStop() {
        //stop can be called from all states except STOP state
        if (visualizationState == visualization_state.VISUALIZATION_RUN || visualizationState == visualization_state.VISUALIZATION_RESUME ||
            visualizationState == visualization_state.VISUALIZATION_REPLAY || visualizationState == visualization_state.VISUALIZATION_PAUSE) {
            ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), new InterfaceStateMsg("PROJECTED UI", interfaceStateMsg.GetSystemState(), interfaceStateMsg.GetTimestamp(),
                interfaceStateMsg.GetProgramID(), interfaceStateMsg.GetBlockID(), currentProgramItem, new List<KeyValueMsg>() { new KeyValueMsg("HOLOLENS_VISUALIZATION", "STOP") },
                interfaceStateMsg.GetEditEnabled(), interfaceStateMsg.GetErrorSeverity(), interfaceStateMsg.GetErrorCode()));
        }
        else if (visualizationState == visualization_state.VISUALIZATION_STOP) {
            speechManager.Say("Visualization is already stopped!");
        }
        else {
            speechManager.Say("There is nothing to stop!");
        }
    }

    //called when user says "Replay"
    public void OnReplay() {
        //replay can be called only from STOP state
        if (visualizationState == visualization_state.VISUALIZATION_STOP) {
            ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), new InterfaceStateMsg("PROJECTED UI", interfaceStateMsg.GetSystemState(), interfaceStateMsg.GetTimestamp(),
                interfaceStateMsg.GetProgramID(), interfaceStateMsg.GetBlockID(), currentProgramItem, new List<KeyValueMsg>() { new KeyValueMsg("HOLOLENS_VISUALIZATION", "REPLAY") },
                interfaceStateMsg.GetEditEnabled(), interfaceStateMsg.GetErrorSeverity(), interfaceStateMsg.GetErrorCode()));
        }
        else if(visualizationState == visualization_state.VISUALIZATION_RUN || visualizationState == visualization_state.VISUALIZATION_RESUME ||
            visualizationState == visualization_state.VISUALIZATION_REPLAY || visualizationState == visualization_state.VISUALIZATION_PAUSE) {
            speechManager.Say("You have to stop the visualization first!");
        }
        else {
            speechManager.Say("There is nothing to replay!");
        }
    }

    //called when user says "Pause"
    public void OnPause() {
        if (visualizationState == visualization_state.VISUALIZATION_RUN || visualizationState == visualization_state.VISUALIZATION_RESUME || 
            visualizationState == visualization_state.VISUALIZATION_REPLAY) {
            ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), new InterfaceStateMsg("PROJECTED UI", interfaceStateMsg.GetSystemState(), interfaceStateMsg.GetTimestamp(),
                interfaceStateMsg.GetProgramID(), interfaceStateMsg.GetBlockID(), currentProgramItem, new List<KeyValueMsg>() { new KeyValueMsg("HOLOLENS_VISUALIZATION", "PAUSE") },
                interfaceStateMsg.GetEditEnabled(), interfaceStateMsg.GetErrorSeverity(), interfaceStateMsg.GetErrorCode()));
        }
        else if (visualizationState == visualization_state.VISUALIZATION_PAUSE) {
            speechManager.Say("Visualization is already paused!");
        }
        else if (visualizationState == visualization_state.VISUALIZATION_STOP) {
            speechManager.Say("Visualization is not even running!");
        }
        else {
            speechManager.Say("There is nothing to pause!");
        }
    }

    //called when user says "Resume"
    public void OnResume() {
        //resume can be called only from PAUSE state
        if (visualizationState == visualization_state.VISUALIZATION_PAUSE) {
            ROSCommunicationManager.Instance.ros.Publish(InterfaceStatePublisher.GetMessageTopic(), new InterfaceStateMsg("PROJECTED UI", interfaceStateMsg.GetSystemState(), interfaceStateMsg.GetTimestamp(),
                interfaceStateMsg.GetProgramID(), interfaceStateMsg.GetBlockID(), currentProgramItem, new List<KeyValueMsg>() { new KeyValueMsg("HOLOLENS_VISUALIZATION", "PAUSE") },
                interfaceStateMsg.GetEditEnabled(), interfaceStateMsg.GetErrorSeverity(), interfaceStateMsg.GetErrorCode()));
        }
        else if (visualizationState == visualization_state.VISUALIZATION_RUN || visualizationState == visualization_state.VISUALIZATION_RESUME ||
            visualizationState == visualization_state.VISUALIZATION_REPLAY) {
            speechManager.Say("You have to pause the visualization first!");
        }
        else if (visualizationState == visualization_state.VISUALIZATION_STOP) {
            speechManager.Say("Visualization is not even running!");
        }
        else {
            speechManager.Say("There is nothing to resume!");
        }
    }
}
