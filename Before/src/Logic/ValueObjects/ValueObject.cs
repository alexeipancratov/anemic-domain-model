namespace Logic.ValueObjects;

// Could use struct, but since it doesn't support inheritance, we would need to 
// overload operators in each struct. Also they don't work quite well with ORMs.
public abstract class ValueObject<T>
    where T: ValueObject<T>
{
    public override bool Equals(object obj)
    {
        var valueObject = obj as T;

        if (valueObject == null)
            return false;

        return EqualsCore(valueObject);
    }

    protected abstract bool EqualsCore(T other);

    public override int GetHashCode()
    {
        return GetHashCodeCore();
    }

    protected abstract int GetHashCodeCore();

    public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
    {
        // Can accept null for both operands (unlike Equals).
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
    {
        return !(a == b);
    }
}