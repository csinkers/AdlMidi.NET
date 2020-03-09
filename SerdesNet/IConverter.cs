namespace SerdesNet
{
    public interface IConverter<TPersistent, TMemory>
    {
        TPersistent ToPersistent(TMemory memory);
        TMemory ToMemory(TPersistent persistent);
    }
}
