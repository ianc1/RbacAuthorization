namespace RbacAuthorization.Utilities;

public abstract class ValueObject : IComparable
{
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return EqualOperator(left, right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, right) || left != null && left.Equals(right);
    }

    protected abstract IEnumerable<IComparable?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public int CompareTo(ValueObject? other)
    {
        if (other == null)
        {
            return 1;
        }

        if (ReferenceEquals(other, this))
        {
            return 0;
        }

        return GetEqualityComponents().Zip(
            other.GetEqualityComponents(),
            (left, right) => left?.CompareTo(right) ?? (right is null ? 0 : -1))
            .FirstOrDefault(comparison => comparison != 0);
    }

    public virtual int CompareTo(object? obj)
    {
        return CompareTo(obj as ValueObject);
    }

    public static bool operator <(ValueObject left, ValueObject right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(ValueObject left, ValueObject right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(ValueObject left, ValueObject right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(ValueObject left, ValueObject right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}
