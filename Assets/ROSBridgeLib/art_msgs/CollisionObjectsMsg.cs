using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;


/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
		public class CollisionObjectsMsg : ROSBridgeMsg {

            private List<CollisionPrimitiveMsg> _primitives = new List<CollisionPrimitiveMsg>();

			public CollisionObjectsMsg(JSONNode msg) {
                foreach (JSONNode item in msg["primitives"].AsArray) {
                    _primitives.Add(new CollisionPrimitiveMsg(item));
                }
            }
			
			public CollisionObjectsMsg(List<CollisionPrimitiveMsg> primitives) {
                _primitives = primitives;
			}

            public static string GetMessageType() {
				return "art_msgs/CollisionPrimitiveMsg";
			}			

            public List<CollisionPrimitiveMsg> GetPrimitives() {
                return _primitives;
            }

            public CollisionPrimitiveMsg GetPrimitiveByName(string name) {
                CollisionPrimitiveMsg searchedPrimitive = null;
                foreach (CollisionPrimitiveMsg primitive in _primitives) {
                    if (primitive.GetName() == name) {
                        searchedPrimitive = primitive;
                        break;
                    }
                }
                return searchedPrimitive;
            }

            public void UpdatePrimitiveMsg(CollisionPrimitiveMsg msg) {
                for (int i = 0; i < _primitives.Count; i++) {
                    if (_primitives[i].GetName() == msg.GetName()) {
                        _primitives[i] = msg;
                        break;
                    }
                }
            }

            public override string ToString() {
                string primitivesString = "[";
                for (int i = 0; i < _primitives.Count; i++) {
                    primitivesString = primitivesString + _primitives[i].ToString();
                    if (_primitives.Count - i > 1) primitivesString += ",";
                }
                primitivesString += "]";

                return "CollisionObjects [primitives=" + primitivesString + "]";
			}
            
            public override string ToYAMLString() {
                string primitivesString = "[";
                for (int i = 0; i < _primitives.Count; i++) {
                    primitivesString = primitivesString + _primitives[i].ToYAMLString();
                    if (_primitives.Count - i > 1) primitivesString += ",";
                }
                primitivesString += "]";

                return "{\"primitives\":" + primitivesString + "}";
            }
		}
	}
}