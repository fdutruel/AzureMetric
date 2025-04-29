using System;

/// <summary>
/// Métrique d'appel API Graph (volume, durée, endpoint, date).
/// </summary>
public class ApiCallMetric
{
    /// <summary>
    /// Identifiant unique de la métrique.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Endpoint Graph API appelé.
    /// </summary>
    public string Endpoint { get; set; }
    /// <summary>
    /// Nombre d'appels sur cet endpoint.
    /// </summary>
    public int CallCount { get; set; }
    /// <summary>
    /// Durée moyenne des appels (ms).
    /// </summary>
    public double AvgDuration { get; set; }
    /// <summary>
    /// Date de la mesure.
    /// </summary>
    public DateTime Date { get; set; }
}