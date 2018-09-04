using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib.art_msgs;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.diagnostic_msgs;

public class FakeROS : MonoBehaviour {

    InterfaceStateMsg intefaceStateMsg;
    HololensStateMsg hololensStateMsg;
    ProgramMsg programMsg;

	// Use this for initialization
	void Start () {
        CreateFakeInterfaceStateMsg();
        CreateFakeHololensStateMsg();
        CreateFakeProgramMsg();
        VisualizationManager.Instance.SetHololensStateMsgFromROS(hololensStateMsg);
        VisualizationManager.Instance.SetInterfaceStateMsgFromROS(intefaceStateMsg);
        ProgramManager.Instance.SetProgramMsgFromROS(programMsg);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateFakeInterfaceStateMsg() {
        intefaceStateMsg = new InterfaceStateMsg("ART_BRAIN", 6, new TimeMsg(1520366291, 111136913), 13, 1,
                                new ProgramItemMsg(1, 2, 0, "PickFromPolygon", new List<string>(), new List<PoseStampedMsg>(),
                                    new List<PolygonStampedMsg>(), new List<ushort>(), new List<KeyValueMsg>(), new List<string>(), new List<SceneLabelMsg>()),
                                new List<KeyValueMsg>(), false, 0, 0);
    }

    private void CreateFakeHololensStateMsg() {
        hololensStateMsg = new HololensStateMsg(hololens_state.STATE_VISUALIZING, visualization_state.VISUALIZATION_RUN);
    }

    private void CreateFakeProgramMsg() {
        programMsg = new ProgramMsg(new ProgramHeaderMsg(13, "Training - polygon", "", false), new List<ProgramBlockMsg>() {
                        new ProgramBlockMsg(1, "Zvedni ze stolu a poloz", new List<ProgramItemMsg> {
                            new ProgramItemMsg(1, 2, 0, "PickFromPolygon", new List<string>(){ "ShortLeg" }, new List<PoseStampedMsg>(),
                                new List<PolygonStampedMsg>() { new PolygonStampedMsg(new HeaderMsg(0, new TimeMsg(0, 0), "marker"), new PolygonMsg(new PointMsg[] {
                                    new PointMsg(0.9060779f, 0.2258368f, 0f), new PointMsg(1.17369f, 0.0966467f, 0f), new PointMsg(1.308086f, 0.5375652f, 0f),
                                    new PointMsg(1.020977f, 0.5331101f, 0f)}))}, 
                                new List<ushort>(), new List<KeyValueMsg>(), new List<string>(), new List<SceneLabelMsg>()),

                            new ProgramItemMsg(2, 3, 0, "PlaceToPose", new List<string>(), new List<PoseStampedMsg>() {
                                new PoseStampedMsg( new HeaderMsg(0, new TimeMsg(0,0), "marker"), 
                                new ROSBridgeLib.geometry_msgs.PoseMsg(new PointMsg(0.4221045f, 0.378173f, 0.023f), 
                                new QuaternionMsg(0.6944976f, 0.132371f, 0.132371f, 0.6944976f)))},
                                new List<PolygonStampedMsg>(), new List<ushort>() { 1 }, new List<KeyValueMsg>(), new List<string>(), new List<SceneLabelMsg>()),

                            new ProgramItemMsg(3, 4, 0, "GetReady", new List<string>(), new List<PoseStampedMsg>(),
                                new List<PolygonStampedMsg>(), new List<ushort>(), new List<KeyValueMsg>(), new List<string>(), new List<SceneLabelMsg>()),

                            new ProgramItemMsg(4, 1, 0, "WaitUntilUserFinishes", new List<string>(), new List<PoseStampedMsg>(),
                                new List<PolygonStampedMsg>(), new List<ushort>() { 2 }, new List<KeyValueMsg>(), new List<string>(), new List<SceneLabelMsg>())
                        }, 1, 0)});
    }

}
