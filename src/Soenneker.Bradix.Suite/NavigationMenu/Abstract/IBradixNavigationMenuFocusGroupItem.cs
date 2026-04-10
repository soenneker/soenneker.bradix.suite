using System.Threading.Tasks;

namespace Soenneker.Bradix;

internal interface IBradixNavigationMenuFocusGroupItem
{
    ValueTask FocusAsync();
}
