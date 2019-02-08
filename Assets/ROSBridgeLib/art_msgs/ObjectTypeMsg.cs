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

            private string _name;
            private SolidPrimitiveMsg _bbox;
            private bool _container;

            public ObjectTypeMsg(JSONNode msg) {
                _name = msg["name"];
                _bbox = new SolidPrimitiveMsg(msg["bbox"]);
                _container = bool.Parse(msg["container"]);
            }
			
			public ObjectTypeMsg(string name, SolidPrimitiveMsg bbox, bool container) {
                _name = name;
                _bbox = bbox;
                _container = container;
			}

            public static string GetMessageType() {
				return "art_msgs/ObjectType";
			}
			
            public string GetName() {
                return _name;
            }

            public SolidPrimitiveMsg GetBBox() {
                return _bbox;
            }

            public bool GetContainer() {
                return _container;
            }

            public override string ToString() {
                return "ObjectType [name=" + _name +
                    ", bbox=" + _bbox.ToString() + 
                    ", container=" + _container + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"name\":\"" + _name + 
                    "\", \"bbox\":" + _bbox.ToYAMLString() +
                    ", \"container\":" + _container + "}";
            }
		}
	}
}