using System.Collections;
using System.Text;
using SimpleJSON;
using System;
using System.Collections.Generic;
using ROSBridgeLib.std_msgs;
using ROSBridgeLib.diagnostic_msgs;

/* 
 * @brief ARTable - VUT FIT
 * @author Daniel Bambušek
 */

namespace ROSBridgeLib {
	namespace art_msgs {
		public class GuiNotificationMsg : ROSBridgeMsg {

            public enum MessageType:UInt16 {
                INFO = 0,
                WARN = 1,
                ERROR = 2
            }

            private string _msg;
            private float _min_duration;
            private bool _temp;
            private MessageType _message_type;

			public GuiNotificationMsg(JSONNode msg) {
                _msg = msg["msg"];
                _min_duration = float.Parse(msg["min_duration"]);
                _temp = bool.Parse(msg["temp"]);
                _message_type = (MessageType) UInt16.Parse(msg["message_type"]);
            }
			
			public GuiNotificationMsg(string msg, float min_duration, bool temp, MessageType message_type) {
                _msg = msg;
                _min_duration = min_duration;
                _temp = temp;
                _message_type = message_type;
            }

            public static string GetMessageType() {
				return "art_msgs/GuiNotification";
			}
			
			public string GetMsg() {
				return _msg;
			}
			
            public float GetMinDuration() {
                return _min_duration;
            }

            public bool GetTemp() {
                return _temp;
            }
            
            public MessageType GetMessageTypeProperty() {
                return _message_type;
            }
            
            public override string ToString() {
                return "GuiNotification [msg=" + _msg +
                    ", min_duration=" + _min_duration +
                    ", temp=" + _temp.ToString().ToLower() +
                    ", message_type=" + (UInt16) _message_type + "]";
			}
            
            public override string ToYAMLString() {
                return "{\"msg\":\"" + _msg + "\"" +
                    ", \"min_duration\":" + _min_duration +
                    ", \"temp\":" + _temp.ToString().ToLower() +
                    ", \"message_type\":" + (UInt16)_message_type + "}";
            }
		}
	}
}