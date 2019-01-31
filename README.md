# ARTableHoloLens

Unity application for Microsoft HoloLens for use in [ARCOR](https://github.com/robofit/arcor) platform (augmented reality-based human-robot interaction). Application visualizes detected objects by robot ("to see what robot sees"), it provides visualization of robot learned programs which is fully controllable with voice. Furthemore, user is able to configure collision environment with use of gestures. The application communicates with ARCOR through a rosbridge, since the ARCOR runs under the Ubuntu 14.04 along with ROS Indigo.

I am using Unity 2017.4.7f1 version.

<img src=/images/artableHoloFinal.jpg />

### Calibration
HoloLens needs to be calibrated due to the ARCOR system (world origin is in the table's right bottom corner). For calibration, Vuforia's image tracking was used. Markers can be found in [ARCOR repo](https://github.com/robofit/arcor/tree/master/art_calibration/markers). When HoloLens detects marker, it draws virtual cube on it; when satisfied with detection, click on the cube to confirm it's position. Only three markers are used, which have to be placed as follows:

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Robot

11--------------------------------(12)

|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|

|&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|

10---------------------------------13

<img src=/images/calib1.jpg width="400" /> <img src=/images/calib2.jpg width="400" />

### Features
 - Visualization of detected objects
 - Visualization of learned robot programs
 - Manipulation with collision objects – those, into which the robot is not allowed to enter (see video linked below)

<img src=/images/visualization.jpg width="400" /> <img src=/images/CollisionEnv.png width="400" />

### Used external packages
 - [ROSBridgeLib](https://github.com/MathiasCiarlo/ROSBridgeLib) – for communication with ROS
 - [Vuforia](https://library.vuforia.com/articles/Training/Developing-Vuforia-Apps-for-HoloLens) – for marker detection and calibration
 - [MixedRealityToolkit-Unity](https://github.com/Microsoft/MixedRealityToolkit-Unity) – version 2017.4.1.0

### Videos
 - [Visualization of robotic programs using Microsoft HoloLens](https://youtu.be/awy0nxeU-4w)
 - [Collision Environment in Microsoft HoloLens](https://youtu.be/WURgPFlkluk)

### Publications
 - [Combining Spatial Augmented Reality Interface with Mixed Reality Head-mounted Display to Enhance User Understanding of Robotic Programs](http://excel.fit.vutbr.cz/submissions/2018/044/44.pdf) – paper presented on student conference Excel@FIT 2018
 - [User Interface for ARTable and Microsoft HoloLens](https://www.fit.vutbr.cz/study/DP/DP.php.en?id=20106&file=t) – My Master's Thesis
