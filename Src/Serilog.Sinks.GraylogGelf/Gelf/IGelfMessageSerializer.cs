namespace Serilog.Sinks.Graylog.Extended.Gelf
{
    internal interface IGelfMessageSerializer
    {
        string SerializeToString(GelfMessage message);
        byte[] SerializeToStringBytes(GelfMessage message);
    }
}