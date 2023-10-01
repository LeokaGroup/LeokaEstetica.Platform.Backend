namespace LeokaEstetica.Platform.Base.Models.Dto;

public class HandlerInfo
{
    /// <summary>
    /// Gets true if data is dynamic; otherwise false.
    /// </summary>
    public bool IsDynamic { get; }

    /// <summary>
    /// Gets handler type.
    /// </summary>
    public Type HandlerType { get; }

    private HandlerInfo(bool isDynamic, Type handlerType)
    {
        IsDynamic = isDynamic;
        HandlerType = handlerType;
    }

    public static HandlerInfo Dynamic(Type handlerType)
    {
        return new HandlerInfo(true, handlerType);
    }

    public static HandlerInfo Typed(Type handlerType)
    {
        return new HandlerInfo(false, handlerType);
    }
}