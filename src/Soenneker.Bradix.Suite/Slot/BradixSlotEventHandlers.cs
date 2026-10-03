namespace Soenneker.Bradix;

internal struct BradixSlotEventHandlers(object childValue, object slotValue, object callback)
{
    internal readonly object ChildValue = childValue;
    internal readonly object SlotValue = slotValue;
    internal readonly object Callback = callback;
    internal int Generation;
}
