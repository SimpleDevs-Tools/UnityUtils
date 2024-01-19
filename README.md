# UnityEditorUtils

_A collection of helpful Unity scripts and editor tools, purely for convenience._

---

## ReadOnly

_**Source:** [FuzzyLogic's answer in the Unity forums](https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/7)_

To convert any field into a ReadOnly column:

````csharp
[ReadOnly] public float example;
````

## Help Box

_**Source:** [unity-inspector-help](https://github.com/johnearnshaw/unity-inspector-help/tree/master)_

Add a help text box info into your inspector.

By default, you can just use:

````csharp
[Help("This is some help text!")]
public float example = 1000f;
````

You can add an optional parameter that changes the icon that appears with the help box:

* `UnityEditor.MessageType.Info` : The default icon.
* `UnityEditor.MessageType.Error` : A red error icon.
* `UnityEditor.MessageType.Warning` : A yellow warning icon.
* `UnityEditor.MessageType.None` : Removes the icon, leaving only the text.

````csharp
#if UNITY_EDITOR
[Help("This is some help text!", UnityEditor.MessageType.None)]
#endif
public float example = 1000f;
````