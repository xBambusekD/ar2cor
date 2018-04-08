using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;
/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
		public class ProgramHeaderMsg : ROSBridgeMsg {
            private UInt16 _id;
            private string _name;
            private string _description;
            private bool _readonly;

			public ProgramHeaderMsg(JSONNode msg) {
                _id = UInt16.Parse(msg["id"]);
                _name = msg["name"];
                _description = msg["description"];
                _readonly = bool.Parse(msg["readonly"]);
            }
			
			public ProgramHeaderMsg(UInt16 id, string name, string description, bool readonlyP) {
                _id = id;
                _name = name;
                _description = description;
                _readonly = readonlyP;
			}

            public static string GetMessageType() {
				return "art_msgs/ProgramHeader";
			}
			
            public UInt16 GetID() {
                return _id;
            }

            public string GetName() {
                return _name;
            }

            public string GetDescription() {
                return _description;
            }

            public bool GetReadonly() {
                return _readonly;
            }

            public override string ToString() {
                return "ProgramHeader [id=" + _id +
                    ", name=" + _name +
                    ", description=" + _description +
                    ", readonly=" + _readonly.ToString().ToLower() + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"id\":" + _id +
                    ", \"name\":" + _name +
                    ", \"description\":" + _description + 
                    ", \"readonly\":" + _readonly.ToString().ToLower() + "}";
            }
		}
	}
}