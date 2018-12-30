namespace NTMiner.Language {
    public interface ILangItem {
        string LangCode { get; }
        string ViewId { get; }
        string Key { get; }
        string Value { get; }
    }
}
