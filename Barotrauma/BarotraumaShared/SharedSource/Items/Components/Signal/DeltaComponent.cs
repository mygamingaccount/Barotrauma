namespace Barotrauma.Items.Components
{
    class DeltaComponent : ItemComponent
    {
        private int prevValueHash;


        public DeltaComponent(Item item, ContentXElement element)
            : base (item, element)
        {
        }

        public override void ReceiveSignal(Signal signal, Connection connection)
        {
            if (connection.Name != "signal_in") { return; }

            int valueHash = signal.value.GetHashCode();

            if (valueHash == prevValueHash) {return;}
            prevValueHash = valueHash;
            signal.power = 0.0f;
            item.SendSignal(signal, "signal_out");
        }
    }
}
