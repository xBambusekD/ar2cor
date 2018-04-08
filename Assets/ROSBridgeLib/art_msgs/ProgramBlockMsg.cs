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
		public class ProgramBlockMsg : ROSBridgeMsg {
            private UInt16 _id;
            private string _name;
            private List<ProgramItemMsg> _items = new List<ProgramItemMsg>();
            private UInt16 _on_success;
            private UInt16 _on_failure;

			public ProgramBlockMsg(JSONNode msg) {
                _id = UInt16.Parse(msg["id"]);
                _name = msg["name"];
                foreach (JSONNode item in msg["items"].AsArray) {
                    _items.Add(new ProgramItemMsg(item));
                }
                _on_success = UInt16.Parse(msg["on_success"]);
                _on_failure = UInt16.Parse(msg["on_failure"]);
            }
			
			public ProgramBlockMsg(UInt16 id, string name, List<ProgramItemMsg> items, UInt16 on_success, UInt16 on_failure) {
                _id = id;
                _name = name;
                _items = items;
                _on_success = on_success;
                _on_failure = on_failure;
			}

            public static string GetMessageType() {
				return "art_msgs/ProgramBlock";
			}
			
			public UInt16 GetID() {
				return _id;
			}

            public string GetName() {
                return _name;
            }

            public List<ProgramItemMsg> GetProgramItems() {
                return _items;
            }

            public UInt16 GetOnSuccess() {
                return _on_success;
            }
            public UInt16 GetOnFailure() {
                return _on_failure;
            }

            public override string ToString() {
                string itemsString = "[";
                for (int i = 0; i < _items.Count; i++) {
                    itemsString = itemsString + _items[i].ToString();
                    if (_items.Count - i <= 1) itemsString += ",";
                }
                itemsString += "]";

                return "ProgramBlock [id=" + _id +
                    ", name=" + _name +
                    ", items=" + itemsString +
                    ", on_success=" + _on_success +
                    ", on_failure=" + _on_failure + "]";
			}
            
            public override string ToYAMLString() {
                string itemsString = "[";
                for (int i = 0; i < _items.Count; i++) {
                    itemsString = itemsString + _items[i].ToYAMLString();
                    if (_items.Count - i <= 1) itemsString += ",";
                }
                itemsString += "]";
                return "{\"id\":" + _id +
                    ", \"name\":" + _name +
                    ", \"items\":" + itemsString +
                    ", \"on_success\":" + _on_success +
                    ", \"on_failure\":" + _on_failure + "}";
            }
		}
	}
}