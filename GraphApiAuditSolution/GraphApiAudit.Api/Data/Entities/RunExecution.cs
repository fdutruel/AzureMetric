using System;

/// <summary>
/// Historique d'une exécution de collecte de métriques.
/// </summary>
public class RunExecution
{
    /// <summary>
    /// Identifiant unique de l'exécution.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Date et heure d'exécution.
    /// </summary>
    public DateTime ExecutedAt { get; set; }
    /// <summary>
    /// Durée de l'exécution en millisecondes.
    /// </summary>
    public int DurationMs { get; set; }
    /// <summary>
    /// Statut de l'exécution (succès, échec, etc.).
    /// </summary>
    public string Status { get; set; }
}