namespace NamelessQuizness.Renderers
{
    public interface IRenderer
    {
        void DrawMessage(string message, MessageFlags messageFlags = MessageFlags.None);
        void DrawMessage(string message, MessageFlags messageFlags, params object[] args);
        void DrawInput(string preInputMessage, Action<string?> callback);
    }
}
