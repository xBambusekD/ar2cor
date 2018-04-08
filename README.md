# ARTableHoloLens

Unity application for Microsoft HoloLens for use in [ARTable](https://github.com/robofit/artable) platform (human-robot collaborative workspace). Application visualizes detected objects by robot ("to see what robot sees"), it provides visualization of robot learned programs which is fully controllable with voice. This application is complementary with ARTable version in my [fork](https://github.com/xBambusekD/artable/tree/bambusek).

<img src=calib.jpg width="400" /> <img src=visualization.jpg width="400" />

### Used packages:
 - [ROSBridgeLib](https://github.com/MathiasCiarlo/ROSBridgeLib) - for communication with ROS on which ARTable runs
 - [HoloLensARToolKit](https://github.com/qian256/HoloLensARToolKit) - for marker detection thus calibration
 - [MixedRealityToolkit-Unity](https://github.com/Microsoft/MixedRealityToolkit-Unity) - for general purposes to ease development
