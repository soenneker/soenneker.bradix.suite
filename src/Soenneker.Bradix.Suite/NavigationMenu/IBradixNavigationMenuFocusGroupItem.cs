using System.Threading.Tasks;

namespace Soenneker.Bradix.Suite.NavigationMenu;

internal interface IBradixNavigationMenuFocusGroupItem
{
    ValueTask FocusAsync();
}
