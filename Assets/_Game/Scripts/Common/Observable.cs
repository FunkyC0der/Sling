namespace Sling.Common
{
  public class Observable<T>
  {
    public delegate void ValueChanged(T oldValue, T newValue);
    public event ValueChanged OnValueChanged;
    
    private T _value;

    public T Value
    {
      get => _value;
      set
      {
        if (Equals(_value, value))
          return;

        T oldValue = _value;
        _value = value;
        OnValueChanged?.Invoke(oldValue, _value);
      }
    }
  }
}