namespace AiNotes.Models
{
    public class OpenModalMessage
    {
        public string MarkdownText { get; }

        public OpenModalMessage(string markdownText)
        {
            MarkdownText = markdownText;
        }
    }
}
