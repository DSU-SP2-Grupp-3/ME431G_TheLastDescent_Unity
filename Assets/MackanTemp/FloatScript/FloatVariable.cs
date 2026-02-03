using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Variables/float")]
public class FloatVariable : ScriptableObject
{
    [SerializeField] private float setValue;
    [SerializeField] private float runtimeValue;

    private float previousSetValue;

    private void OnEnable()
    {
        runtimeValue = setValue;
        previousSetValue = setValue;
    }

    private void OnValidate()
    {
        if (setValue != previousSetValue)
        {
            runtimeValue = setValue;
            previousSetValue = setValue;
        }
    }

    public float Value
    {
        get => runtimeValue;
        set => runtimeValue = value;
    }

    public static implicit operator float(FloatVariable variable)
    {
        return variable.Value;
    }
}