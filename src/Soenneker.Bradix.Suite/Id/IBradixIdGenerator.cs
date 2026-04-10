namespace Soenneker.Bradix.Suite.Id;

public interface IBradixIdGenerator
{
    string New(string prefix);
}
