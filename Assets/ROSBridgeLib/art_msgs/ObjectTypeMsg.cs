using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.shape_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
        public class ObjectTypeMsg : ROSBridgeMsg {           

            private String _name;
            private SolidPrimitiveMsg _bbox;

            public ObjectTypeMsg(JSONNode msg) {
                _name = msg["name"];
                _bbox = new SolidPrimitiveMsg(msg["bbox"]);
            }
			
			public ObjectTypeMsg(String name, SolidPrimitiveMsg bbox) {
                _name = name;
                _bbox = bbox;
			}

            public static string GetMessageType() {
				return "art_msgs/ObjectType";
			}
			
            public String GetName() {
                return _name;
            }

            public SolidPrimitiveMsg GetBBox() {
                return _bbox;
            }

            public override string ToString() {
                return "ObjectType [name=" + _name +
                    ", bbox=" + _bbox.ToString() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"name\":" + _name +
                    ", \"bbox\":" + _bbox.ToYAMLString() + "}";
            }
		}
	}
}