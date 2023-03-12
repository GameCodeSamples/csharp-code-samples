This camera system shows the implementation of a third person free look camera in C# using cinemachine that allows for:
- Following a target: The camera smoothly follows a target object, which can be set in the Unity editor or programmatically.
- Rotating: The camera can rotate around the target object based on user input, such as mouse movement or touch gestures.
- Zooming: The camera distance can be adjusted to zoom in or out from the target object.
- It follows a skeleton and keeps the skeleton head centered.
- It also takes collision into account
- It has a layer of abstraction to be able to control it with multiple devices(in this case I have only implemented the events for mobile, but it is ready for other input sources).

![CinemachineFreelook](https://user-images.githubusercontent.com/127549378/224436645-2152cbb3-368c-46f1-b370-4219724fcd5d.png)
