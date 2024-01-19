# UnityEditorUtils

_A collection of helpful Unity scripts and editor tools, purely for convenience._

---

## ReadOnly

_**Source:** [FuzzyLogic's answer in the Unity forums](https://discussions.unity.com/t/how-to-make-a-readonly-property-in-inspector/75448/7)_

To convert any field into a ReadOnly column:

````csharp
[ReadOnly] public float example;
````