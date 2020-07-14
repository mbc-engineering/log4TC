using Optional;

namespace Mbc.Log4Tc.Model.Message
{
    internal class MessageHoleToken : MessageTemplateToken
    {
        public MessageHoleToken(string label, int alignment, string format)
        {
            Label = label;
            Alignment = alignment;
            Format = format;
            if (int.TryParse(Label, out int index))
            {
                Index = index.Some();
            }
            else
            {
                Index = Option.None<int>();
            }
        }

        public string Label { get; }
        public int Alignment { get; }
        public string Format { get; }
        public Option<int> Index { get; }
    }
}
