namespace Serilog.Sinks.GraylogGelf.Gelf
{
    internal interface IGelfMessageSerializer
    {
        string SerializeToString(GelfMessage message);
        byte[] SerializeToStringBytes(GelfMessage message);
    }
}