This is a C# script for a Unity game that defines a component called NetworkPlayerAnimationController. This component is intended to be attached to game objects that represent players and are networked using the Mirror networking library.

The script provides functionality for managing player animations, using an animator component attached to a SkeletonComponent component. The component also requires a PlayerMovement component to be attached to the same game object.

The script provides several methods for playing animations with different priorities, and ensures that higher priority animations interrupt lower priority ones. The script also defines an event OnAnimationCompletedEvent that can be subscribed to by other scripts, which is triggered when an animation completes.

The script uses AddressableAssets to load animations asynchronously, and uses a Priority enum to represent the different priorities of animations. The script also defines an AnimationInfo struct to hold information about a currently playing animation, including its ID and priority.

The NetworkBehaviour base class is used to enable networking functionality, allowing the animation state to be synchronized across the network to other players. The script uses SyncVar attributes to mark variables that should be synchronized, and Command attributes to mark functions that should be invoked on the server.

The NetworkPlayerAnimationController component is initialized when the SkeletonComponent is changed, and on initialization, the default animation and any custom animations are loaded and added to a sorted dictionary of animations, sorted by priority. The PlayTopPriorityAnimation method is then called to play the highest priority animation.

The component also provides functionality for removing all animations of a certain priority, and for handling the completion of gesture animations. The TryPlayAnimation method is used to play an animation with a specified priority and stop the player movement if required.

Finally, the component also defines methods for playing animations on a remote player, and includes client-side and server-side code to ensure that animations are synchronized across the network.
