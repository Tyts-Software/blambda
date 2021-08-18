using Ddd.Abstructions.Domain;

namespace BLambda.HolaMundo.Domain.TemperatureLog
{
    public interface IStat : IAggregateRoot
    {
        public string Location { get; set; }
        public Stat Stat { get; set; }
    }
}
