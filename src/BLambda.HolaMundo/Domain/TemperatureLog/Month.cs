namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public enum Month
    {
        NotSet = 0,
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }



    //public sealed class Month
    //{

    //    private readonly string name;
    //    private readonly int value;
    //    private static readonly Dictionary<string, Month> instance = new();

    //    public static readonly Month NotSet = new(0, "NotSet");
    //    public static readonly Month January = new(1, "January");
    //    public static readonly Month February = new(2, "February");
    //    public static readonly Month March = new(3, "March");
    //    public static readonly Month April = new(4, "April");
    //    public static readonly Month May = new(5, "May");
    //    public static readonly Month June = new(6, "June");
    //    public static readonly Month July = new(7, "July");
    //    public static readonly Month August = new(8, "August");
    //    public static readonly Month September = new(9, "September");
    //    public static readonly Month October = new(10, "October");
    //    public static readonly Month November = new(11, "November");
    //    public static readonly Month December = new(12, "December");

    //    private Month(int value, string name)
    //    {
    //        this.name = name;
    //        this.value = value;
    //        instance[name] = this;
    //    }

    //    public override string ToString()
    //    {
    //        return name;
    //    }

    //    public virtual int AsInt()
    //    {
    //        return value;
    //    }

    //    public static explicit operator Month(string str)
    //    {
    //        if (instance.TryGetValue(str, out Month result))
    //            return result;
    //        else
    //            throw new InvalidCastException();
    //    }

    //    public static explicit operator Month(int value)
    //    {
    //        return instance.Values.Single(v => v.value == value);
    //    }
    //}
}
