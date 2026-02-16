namespace NilArea.Common;

public static partial class Extensions
{
    extension<TS, T>(TS)
    {
        public static T operator >> (TS x, Func<TS, T> fun) => fun(x);
    }

    extension<TS, TM, T>(Func<TS, TM>)
    {
        public static Func<TS, T> operator <<(Func<TS, TM> p, Func<TM, T> n) => x => n(p(x));
    }
}
