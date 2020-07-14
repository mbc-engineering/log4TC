namespace Mbc.Log4Tc.Model.Message
{
    internal class MessageTextToken : MessageTemplateToken
    {
        public MessageTextToken(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
