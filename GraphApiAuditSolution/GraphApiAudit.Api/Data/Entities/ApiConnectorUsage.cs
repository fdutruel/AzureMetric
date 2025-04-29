using System;

/// <summary>
/// Statistique d'utilisation d'un connecteur API personnalisé.
/// </summary>
public class ApiConnectorUsage
{
    /// <summary>
    /// Identifiant unique de la statistique.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Nom du connecteur.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Nombre d'appels réalisés via ce connecteur.
    /// </summary>
    public int CallCount { get; set; }
    /// <summary>
    /// Date du dernier appel.
    /// </summary>
    public DateTime LastCallDate { get; set; }
    /// <summary>
    /// Statut du connecteur (actif, inactif, etc.).
    /// </summary>
    public string Status { get; set; }
}