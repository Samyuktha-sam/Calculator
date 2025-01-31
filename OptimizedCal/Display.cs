
namespace Cin7.Rookies.Calculator.AdvancedCal.Core
{
    public sealed class Display : IDisposable
    {
        private static readonly Lazy<Display> _Instance = new Lazy<Display>(() => new Display());
        private List<KeyValuePair<string, char>> _Steps;

        public static Display Instance => _Instance.Value;

        private Display()
        {
            _Steps = new List<KeyValuePair<string, char>>();
        }

        public void Add(string Value, char OperatorChar)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                _Steps.Add(new KeyValuePair<string, char>(Value, OperatorChar));
            }
        }

        public bool IsEmpty() => _Steps.Count == 0;

        public void Clear() => _Steps.Clear();

        public override string ToString() => string.Join("\n\t", _Steps.Select(Step => $"{Step.Value}\t{Step.Key}")).Substring(2);

        public void Dispose()
        {
            _Steps.Clear();
            _Steps = null;
        }
    }
}
