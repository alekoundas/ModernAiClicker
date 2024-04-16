
namespace Model.Business
{
    public class CompositeCommandParameter
    {
        public CompositeCommandParameter(EventArgs eventArgs, object parameter)
        {
            EventArgs = eventArgs;
            Parameter = parameter;
        }

        public EventArgs EventArgs { get; }

        public object Parameter { get; }
    }
}
