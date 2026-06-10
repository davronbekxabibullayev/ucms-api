namespace Ucms.Application.Handlers;

/// <summary>
/// Marker interface for MassTransit consumer discovery in the Application assembly.
/// Usage: cfg.AddConsumers(typeof(IApplicationAssemblyMarker).Assembly)
/// </summary>
public interface IApplicationAssemblyMarker { }
