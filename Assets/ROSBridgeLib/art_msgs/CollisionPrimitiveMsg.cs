using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.shape_msgs;
using ROSBridgeLib.geometry_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
        public class CollisionPrimitiveMsg : ROSBridgeMsg {           

            private string _name;
            private string _setup;
            private SolidPrimitiveMsg _bbox;
            private PoseStampedMsg _pose;

            public CollisionPrimitiveMsg(JSONNode msg) {
                _name = msg["name"];
                _setup = msg["setup"];
                _bbox = new SolidPrimitiveMsg(msg["bbox"]);
                _pose = new PoseStampedMsg(msg["pose"]);
            }
			
			public CollisionPrimitiveMsg(string name, string setup, SolidPrimitiveMsg bbox, PoseStampedMsg pose) {
                _name = name;
                _setup = setup;
                _bbox = bbox;
                _pose = pose;
			}

            public static string GetMessageType() {
				return "art_msgs/CollisionPrimitive";
			}
			
            public string GetName() {
                return _name;
            }

            public string GetSetup() {
                return _setup;
            }

            public SolidPrimitiveMsg GetBBox() {
                return _bbox;
            }

            public PoseStampedMsg GetPose() {
                return _pose;
            }

            public override string ToString() {
                return "CollisionPrimitiveMsg [name=" + _name +
                    ", setup=" + _setup +
                    ", bbox=" + _bbox.ToString() +
                    ", pose=" + _pose.ToString() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"name\":\"" + _name + "\"" +
                    ", \"setup\":\"" + _setup + "\"" +
                    ", \"bbox\":" + _bbox.ToYAMLString() + 
                    ", \"pose\":" + _pose.ToYAMLString() + "}";
            }
		}
	}
}