namespace NTMiner {
    // 所有的实体都可以基于反射赋值。注意：IMinerData, ISpeedData不继成IEntity是被特别看待的。
    public interface IEntity : ICanUpdateByReflection { }
}
