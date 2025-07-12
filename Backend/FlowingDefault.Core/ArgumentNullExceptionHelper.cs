namespace FlowingDefault.Core;

public static class ArgumentNullExceptionHelper
{
    public static void ThrowIfNull(object obj, string? nameOf = null)
    {
        if (obj == null)
            throw new ArgumentNullException(nameOf ?? nameof(obj));
    }
}