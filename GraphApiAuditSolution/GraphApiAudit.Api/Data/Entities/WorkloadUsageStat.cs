using System;

/// <summary>
/// Statistique d'utilisation d'un workload Microsoft 365 par utilisateur.
/// </summary>
public class WorkloadUsageStat
{
    /// <summary>
    /// Identifiant unique de la statistique.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Type de workload (Exchange, SharePoint, etc.).
    /// </summary>
    public string WorkloadType { get; set; }
    /// <summary>
    /// Identifiant de l'utilisateur concerné.
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// Date de la mesure.
    /// </summary>
    public DateTime UsageDate { get; set; }
}