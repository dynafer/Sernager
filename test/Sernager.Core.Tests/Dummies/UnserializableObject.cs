namespace Sernager.Core.Tests.Dummies;

public class UnserializableObject
{
    public UnserializableObject Self { get; set; }

    public UnserializableObject()
    {
        Self = this;
    }
}
