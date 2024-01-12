namespace Sernager.Core.Models;

public sealed class GroupItemModel
{
    public required Guid Id { get; init; }
    private object mItem = null!;
    public object Item
    {
        get
        {
            return mItem;
        }
        init
        {
            if (value is GroupModel || value is CommandModel)
            {
                mItem = value;
            }
            else
            {
                throw new InvalidCastException("Item must be a GroupModel or CommandModel.");
            }
        }
    }
}
