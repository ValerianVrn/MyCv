namespace MyCv.UI.Dtos
{
    internal sealed record DomainEvent(DateTime DateTime, string DomainEventType, string Intent, int? Priority)
    {
        internal const string IntentAdded = "INTENT_ADDED";
        internal const string IntentRemoved = "INTENT_REMOVED";
        internal const string PriorityChanged = "PRIORITY_CHANGED";
    }
}
