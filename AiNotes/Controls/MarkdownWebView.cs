namespace AiNotes.Controls
{
    public class MarkdownWebView : WebView
    {
        public static readonly BindableProperty MarkdownProperty =
            BindableProperty.Create(nameof(Markdown), typeof(string), typeof(MarkdownWebView), propertyChanged: OnMarkdownChanged);

        public string Markdown
        {
            get { return (string)GetValue(MarkdownProperty); }
            set { SetValue(MarkdownProperty, value); }
        }

        private static void OnMarkdownChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MarkdownWebView)bindable;
            control.UpdateHtml();
        }

        private void UpdateHtml()
        {
            var htmlSource = new HtmlWebViewSource
            {
                Html = $"<html><body>{Markdown}</body></html>"
            };
            Source = htmlSource;
        }
    }
}
