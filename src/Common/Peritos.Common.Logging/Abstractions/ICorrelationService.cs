namespace Peritos.Common.Logging.Abstractions
{
    public interface ICorrelationService
    {
        string CorrelationId { get; }
    }
}
