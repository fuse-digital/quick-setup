using Microsoft.Extensions.Logging;
using Riok.Mapperly.Abstractions;
using Serilog.Events;
using Volo.Abp.Mapperly;

namespace FuseDigital.QuickSetup.Cli;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target, EnumMappingStrategy = EnumMappingStrategy.ByValue)]
public partial class LogEventLevelToLogLevelMapper : TwoWayMapperBase<LogEventLevel, LogLevel>
{
    [MapperIgnoreTargetValue(LogLevel.None)]
    public override partial LogLevel Map(LogEventLevel source);

    public override void Map(LogEventLevel source, LogLevel destination)
    {
        throw new System.NotSupportedException("LogLevel is a value type, so an in place map cannot update the caller.");
    }

    public override partial LogEventLevel ReverseMap(LogLevel source);

    public override void ReverseMap(LogLevel source, LogEventLevel destination)
    {
        throw new System.NotSupportedException("LogEventLevel is a value type, so an in place map cannot update the caller.");
    }
}
