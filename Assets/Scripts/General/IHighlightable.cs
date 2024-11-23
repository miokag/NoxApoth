public interface IHighlightable
{
    void Highlight();
    void Unhighlight();
    bool CanHighlight { get; } 
}