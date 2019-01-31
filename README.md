# ARTableHoloLens

Unity application for Microsoft HoloLens for use in [ARCOR](https://github.com/robofit/arcor) platform (augmented reality-based human-robot interaction). Application visualizes detected objects by robot ("to see what robot sees"), it provides visualization of robot learned programs which is fully controllable with voice. Furthemore, user is able to configure collision environment with use of gestures.

<img src=calib.jpg width="400" /> <img src=visualization.jpg width="400" />

### Used packages:
 - [ROSBridgeLib](https://github.com/MathiasCiarlo/ROSBridgeLib) - for communication with ROS
 - [Vuforia](https://library.vuforia.com/articles/Training/Developing-Vuforia-Apps-for-HoloLens) - for marker detection and calibration
 - [MixedRealityToolkit-Unity](https://github.com/Microsoft/MixedRealityToolkit-Unity) - for general purposes to ease development
