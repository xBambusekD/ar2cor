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
		public class ProgramMsg : ROSBridgeMsg {
            private ProgramHeaderMsg _header;
            private List<ProgramBlockMsg> _blocks = new List<ProgramBlockMsg>();

			public ProgramMsg(JSONNode msg) {
                _header = new ProgramHeaderMsg(msg["header"]);
                foreach (JSONNode item in msg["blocks"].AsArray) {
                    _blocks.Add(new ProgramBlockMsg(item));
                }
            }
			
			public ProgramMsg(ProgramHeaderMsg header, List<ProgramBlockMsg> blocks) {
                _header = header;
                _blocks = blocks;
			}

            public static string GetMessageType() {
				return "art_msgs/Program";
			}
		
            public ProgramHeaderMsg GetHeader() {
                return _header;
            }
            
            public List<ProgramBlockMsg> GetBlocks() {
                return _blocks;
            }

            public ProgramBlockMsg GetBlockByID(UInt16 id) {
                ProgramBlockMsg searchedBlock = null;
                foreach (ProgramBlockMsg block in _blocks) {
                    if(block.GetID() == id) {
                        searchedBlock = block;
                        break;
                    }
                }
                return searchedBlock;
            }

            public override string ToString() {
                string blocksString = "[";
                for (int i = 0; i < _blocks.Count; i++) {
                    blocksString = blocksString + _blocks[i].ToString();
                    if (_blocks.Count - i > 1) blocksString += ",";
                }
                blocksString += "]";

                return "Program [header=" + _header.ToString() +
                    ", blocks=" + blocksString + "]";
			}
            
            public override string ToYAMLString() {
                string blocksString = "[";
                for (int i = 0; i < _blocks.Count; i++) {
                    blocksString = blocksString + _blocks[i].ToYAMLString();
                    if (_blocks.Count - i > 1) blocksString += ",";
                }
                blocksString += "]";

                return "{\"header\":" + _header.ToYAMLString() +
                    ", \"blocks\":" + blocksString + "}";
            }
		}
	}
}