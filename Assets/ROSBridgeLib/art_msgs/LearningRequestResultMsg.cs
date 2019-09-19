using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.std_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
        public class LearningRequstResultMsg : ROSBridgeMsg {
            private bool _success;
            private string _message;

			public LearningRequstResultMsg(JSONNode msg) {
                _success = bool.Parse(msg["success"]);
                _message = msg["message"];
            }
			
			public LearningRequstResultMsg(bool success, string message) {
                _success = success;
                _message = message;
			}

            public static string GetMessageType() {
				return "art_msgs/LearningRequestResult";
			}
		
            public bool GetSuccess() {
                return _success;
            }

            public string GetMessage() {
                return _message;
            }

            public override string ToString() {
                return "LearningRequestResult [success=" + _success.ToString().ToLower() +
                    ", message=" + _message + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"success\":" + _success.ToString().ToLower() +
                    ", \"message\":\"" + _message + "\"}";
            }
		}
	}
}