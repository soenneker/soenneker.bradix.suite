namespace Soenneker.Bradix;

public interface IBradixIdGenerator
{
    string New(string prefix);
}
