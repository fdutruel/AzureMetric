using System;

namespace GraphApiAudit.Api.Data.Entities
{
    /// <summary>
    /// Représente la consommation d'une application enregistrée (App Registration) sur le tenant.
    /// </summary>
    public class AppRegistrationUsage
    {
        /// <summary>
        /// Identifiant unique de la métrique.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identifiant de l'application (AppId Azure AD).
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// Nom affiché de l'application.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Nombre d'appels API effectués par cette application.
        /// </summary>
        public int ApiCallCount { get; set; }
        /// <summary>
        /// Date de la mesure.
        /// </summary>
        public DateTime Date { get; set; }
    }
}
